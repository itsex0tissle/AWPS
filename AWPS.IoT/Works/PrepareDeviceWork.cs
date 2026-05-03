using System;
using System.Net;
using AWPS.IoT.Helpers;
using AWPS.IoT.MsgPack;
using System.Threading;
using AWPS.IoT.Services;
using Iot.Device.Button;
using System.Diagnostics;
using nanoFramework.Json;
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
                Logger.LogInfo("Rebooting...");
            };
            NetworkChange.NetworkAddressChanged += delegate(object sender, EventArgs event_args)
            {
                if(Wireless80211.Connected is true)
                {
                    Logger.LogInfo("Wifi connected");
                }
                else
                {
                    Logger.LogInfo("Wifi disconnected");
                }   
            };
            NetworkChange.NetworkAPStationChanged += delegate(int station_index, NetworkAPStationEventArgs event_args)
            {
                if(event_args.IsConnected is true)
                {
                    Logger.LogInfo("External device connected to AP");
                }
                else
                {
                    Logger.LogInfo("External device disconnected from AP");
                }
            };
            if(Wireless80211.Connected is true)
            {
                Logger.LogInfo("Wifi connected");
            }
        }
        private static void GlobalSetup()
        {
            JsonSerializerOptions.Default.PropertyNameCaseInsensitive = true;
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
            Logger.LogInfo("Button enabled");
        }
        private static void StartDhcpServerIfWirelessAPEnabled()
        {
            if (WirelessAP.Enabled is true)
            {
                if (new DhcpServer().Start(IPAddress.Parse(WirelessAP.IP), IPAddress.Parse(WirelessAP.Mask)) is false)
                {
                    Logger.LogError("DHCP server start failed");
                    Power.RebootDevice();
                }
                Logger.LogInfo("DHCP server started");
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
                        Logger.LogInfo($"{nameof(WirelessAP)} timeout timer disabled");
                    }
                    else
                    {
                        Logger.LogInfo($"{nameof(WirelessAP)} timeout timer enabled");
                        timer?.Dispose();
                        timer = new Timer(DisableWirelessAP, null, 60000, Timeout.Infinite);
                    }
                };
                Logger.LogInfo($"{nameof(WirelessAP)} timeout timer enabled");

                static void DisableWirelessAP(object? state)
                {
                    Logger.LogWarning($"{nameof(WirelessAP)} timeout occurred");
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
                    StartWebServer();
                }
                else
                {
                    StopWebServer();
                }
            };
            if(WirelessAP.GetConfiguration().GetConnectedStations().Length > 0)
            {
                StartWebServer();
            }

            void StartWebServer()
            {
                if(web_server.IsRunning is false && web_server.Start() is true)
                {
                    Logger.LogInfo($"Web server started. Listening on: 'http://{WirelessAP.IP}:80'");
                }
                else
                {
                    Logger.LogError("Web server start failed");
                    Power.RebootDevice();
                }
            }
            void StopWebServer()
            {
                if(web_server.IsRunning is true)
                {
                    web_server.Stop();
                    Logger.LogInfo("Web server stopped");
                }
            }
        }
        private static void EnsureUtcNowIsValid()
        {
            if(DateTimeHelper.UtcNowValid is true)
            {
                Logger.LogInfo("System time is UTC");
                return;
            }
            Logger.LogWarning("System time is not UTC. Wait 10s for wifi reconnection");
            CancellationTokenSource timeout = new(10000);
            WifiNetworkHelper.Reconnect(requiresDateTime: true, token: timeout.Token);
            if(DateTimeHelper.UtcNowValid is true)
            {
                Logger.LogInfo("System time is UTC");
                return;
            }
            Logger.LogError("System time still not UTC");
            Helper.EnterDeepSleep(TimeSpan.FromMinutes(5));
        }
        public static void Start()
        {
            Logger.LogInfo($"{nameof(PrepareDeviceWork)} started");
            try
            {
                AddDebugLogs();
                GlobalSetup();
                EnableButton();
                MsgPackContextConfigurator.Setup();
                StartDhcpServerIfWirelessAPEnabled();
                EnableTimeoutForWirelessAP();
                ToogleWebServerOnNetworkAPStationChanged();
                if(WirelessAP.Enabled is true)
                {
                    Thread.Sleep(Timeout.Infinite);
                }
                EnsureUtcNowIsValid();
            }
            catch(Exception exc)
            {
                Logger.LogError(exc.ToString());
            }
            Logger.LogInfo($"{nameof(PrepareDeviceWork)} finished");
        }
    }
}