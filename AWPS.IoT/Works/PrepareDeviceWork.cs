using System;
using System.Net;
using AWPS.IoT.MsgPack;
using System.Threading;
using AWPS.IoT.Services;
using Iot.Device.Button;
using System.Diagnostics;
using Iot.Device.DhcpServer;
using AWPS.IoT.WebControllers;
using nanoFramework.WebServer;
using nanoFramework.Networking;
using nanoFramework.Runtime.Native;
using System.Net.NetworkInformation;

namespace AWPS.IoT.Works
{
    public static class PrepareDeviceWork
    {
        [Conditional("DEBUG")] private static void AddDebugLogs()
        {
            Power.OnRebootEvent += delegate()
            {
                Debug.WriteLine("Rebooting...");
            };
            NetworkChange.NetworkAddressChanged += delegate(object sender, EventArgs event_args)
            {
                if(Wireless80211.Connected is true)
                {
                    Debug.WriteLine("Wifi connected");
                }
                else
                {
                    Debug.WriteLine("Wifi disconnected");
                }
            };
            NetworkChange.NetworkAPStationChanged += delegate(int station_index, NetworkAPStationEventArgs event_args)
            {
                if (event_args.IsConnected is true)
                {
                    Debug.WriteLine("External device connected to AP");
                }
                else
                {
                    Debug.WriteLine("External device disconnected from AP");
                }
            };
            if (Wireless80211.Connected is true)
            {
                Debug.WriteLine("Wifi connected");
            }
        }
        private static void EnableButton()
        {
            GpioButton button = new(21);
            button.Press += static delegate(object sender, EventArgs event_args)
            {
                if(WirelessAP.Enabled is false)
                {
                    WirelessAP.Enable();
                }
                else
                {
                    WirelessAP.Disable();
                }
            };
            Debug.Write("Button enabled");
        }
        private static void StartDhcpServerIfWirelessAPEnabled()
        {
            if(WirelessAP.Enabled is true)
            {
                if(new DhcpServer().Start(IPAddress.Parse(WirelessAP.IP), IPAddress.Parse(WirelessAP.Mask)) is false)
                {
                    Debug.WriteLine("DHCP-server start failed");
                    Power.RebootDevice();
                }
                Debug.WriteLine("DHCP-server started");
            }
        }
        private static void EnableTimeoutForWirelessAP()
        {
            if(WirelessAP.Enabled is true)
            {
                Timer timer = new(DisableWirelessAP, null, 60000, Timeout.Infinite);
                NetworkChange.NetworkAPStationChanged += delegate(int station_index, NetworkAPStationEventArgs event_args)
                {
                    if (event_args.IsConnected is true)
                    {
                        timer?.Dispose();
                        Debug.WriteLine("WirelessAP timeout disabled");
                    }
                    else
                    {
                        Debug.WriteLine("WirelessAP timeout enabled");
                        timer?.Dispose();
                        timer = new Timer(DisableWirelessAP, null, 60000, Timeout.Infinite);
                    }
                };
                Debug.WriteLine("WirelessAP timeout enabled");

                static void DisableWirelessAP(object? state)
                {
                    Debug.WriteLine("WirelessAP timeout");
                    WirelessAP.Disable();
                }
            }
        }
        private static void ToogleWebServerOnNetworkAPStationChanged()
        {
            WebServer web_server = new(80, HttpProtocol.Http, IPAddress.Parse(WirelessAP.IP), new Type[]
            {
                typeof(RootWebController),
                typeof(WifiWebController)
            });
            NetworkChange.NetworkAPStationChanged += delegate(int station_index, NetworkAPStationEventArgs event_args)
            {
                if(event_args.IsConnected is true)
                {
                    if(web_server.IsRunning is false && web_server.Start() is true)
                    {
                        Debug.WriteLine($"WebServer started. Listening on: 'http://{WirelessAP.IP}:80'");
                    }
                    else
                    {
                        Debug.WriteLine("WebServer start failed");
                        Power.RebootDevice();
                    }
                }
                else
                {
                    if(web_server.IsRunning is true)
                    {
                        web_server.Stop();
                        Debug.WriteLine("WebServer stopped");
                    }
                }
            };
            if(WirelessAP.GetConfiguration().GetConnectedStations().Length > 0)
            {
                if(web_server.IsRunning is false && web_server.Start() is true)
                {
                    Debug.WriteLine($"WebServer started. Listening on: 'http://{WirelessAP.IP}:80'");
                }
                else
                {
                    Debug.WriteLine("WebServer start failed");
                    Power.RebootDevice();
                }
            }
        }
        private static void EnsureUtcNowIsValid()
        {
            if(DateTime.UtcNow.Year >= 2026)
            {
                Debug.WriteLine("System time is UTC");
                return;
            }
            Debug.WriteLine("System time is not UTC. Wait 10s for wifi reconnection");
            CancellationTokenSource timeout = new(10000);
            WifiNetworkHelper.Reconnect(requiresDateTime: true, token: timeout.Token);
            if(DateTime.UtcNow.Year >= 2026)
            {
                Debug.WriteLine("System time is UTC");
                return;
            }
            Debug.Write("System time is not UTC. ");
            Helper.EnterDeepSleep(TimeSpan.FromMinutes(5));
        }
        public static void Start()
        {
            try
            {
                Debug.WriteLine("PrepareDeviceWork started");
                AddDebugLogs();
                EnableButton();
                MsgPackContextConfigurator.Setup();
                StartDhcpServerIfWirelessAPEnabled();
                EnableTimeoutForWirelessAP();
                ToogleWebServerOnNetworkAPStationChanged();
                if (WirelessAP.Enabled is true)
                {
                    Debug.WriteLine("Device mode: Configuration");
                    Thread.Sleep(Timeout.Infinite);
                }
                Debug.WriteLine("Device mode: Work");
                EnsureUtcNowIsValid();
                Debug.WriteLine("PrepareDeviceWork finished");
            }
            catch(Exception exc)
            {
                Debug.WriteLine($"PrepareDeviceWork failed: {exc}");
            }
        }
    }
}