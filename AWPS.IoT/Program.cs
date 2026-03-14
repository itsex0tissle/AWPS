/* Top-Level packages:
 * nanoFramework.System.Net;
 * nanoFramework.Runtime.Native;
 * nanoFramework.IoT.Device.DhcpServer;
 * nanoFramework.IoT.Device.Button;
 * nanoFramework.WebServer;
 * nanoFramework.System.Device.Wifi;
 * nanoFramework.System.Adc;
 * nanoFramework.IoT.Device.Dhtxx.Esp32;
 * nanoFramework.System.IO.FileSystem;
 * nanoFramework.M2Mqtt;
 * nanoFramework.ResourceManager;
 * nanoFramework.Hardware.Esp32;
 */

using System;
using System.Net;
using System.Threading;
using Iot.Device.Button;
using System.Diagnostics;
using AWPS.IoT.Measuring;
using Iot.Device.DhcpServer;
using nanoFramework.WebServer;
using AWPS.IoT.WebControllers;
using AWPS.IoT.MqttInteraction;
using nanoFramework.Networking;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Runtime.Native;
using System.Net.NetworkInformation;
using System.Diagnostics.CodeAnalysis;

namespace AWPS.IoT
{
    public static class Program
    {
        private static WebServer? WebServer { get; set; }
        private static Timer? WirelessAPTimer { get; set; }
        private static GpioButton? Button { get; set; }

        [Conditional("DEBUG")] private static void SetupLogs()
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
                if(event_args.IsConnected is true)
                {
                    Debug.WriteLine("External device connected to AP");
                }
                else
                {
                    Debug.WriteLine("External device disconnected from AP");
                }
            };
            if(Wireless80211.Connected is true)
            {
                Debug.WriteLine("Wifi connected");
            }
        }
        private static void SetupButton()
        {
            Button = new GpioButton(21);
            Button.Press += static delegate(object sender, EventArgs event_args)
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
                WirelessAPTimer = new Timer(DisableWirelessAP, null, 60000, Timeout.Infinite);
                NetworkChange.NetworkAPStationChanged += delegate (int station_index, NetworkAPStationEventArgs event_args)
                {
                    if (event_args.IsConnected is true)
                    {
                        WirelessAPTimer?.Dispose();
                        Debug.WriteLine("WirelessAP timeout disabled");
                    }
                    else
                    {
                        Debug.WriteLine("WirelessAP timeout enabled");
                        WirelessAPTimer?.Dispose();
                        WirelessAPTimer = new Timer(DisableWirelessAP, null, 60000, Timeout.Infinite);
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
        private static void SetupTooglingWebServerBasedOnAP()
        {
            NetworkChange.NetworkAPStationChanged += delegate(int station_index, NetworkAPStationEventArgs event_args)
            {
                if(event_args.IsConnected is true)
                {
                    WebServer ??= new WebServer(80, HttpProtocol.Http, IPAddress.Parse(WirelessAP.IP), new Type[]
                    {
                        typeof(RootWebController),
                        typeof(Wireless80211WebController)
                    });
                    if(WebServer.Start() is true)
                    {
                        Debug.WriteLine($"WebServer started. Listening on: 'http://{WirelessAP.IP}:80'");
                    }
                    else
                    {
                        Debug.WriteLine("WebServer start failed");
                    }
                }
                else
                {
                    if(WebServer is not null && WebServer.IsRunning is true)
                    {
                        WebServer.Stop();
                        Debug.WriteLine("WebServer stopped");
                    }
                }
            };
        }
        private static void TryReconnectToWifi()
        {
            WifiNetworkHelper.Reconnect(requiresDateTime: true);
        }
        [DoesNotReturn] private static void EnterDeepSleep(TimeSpan restart_in)
        {
            Debug.WriteLine($"Enter deep sleep. Restart in: {restart_in}");
            Sleep.EnableWakeupByTimer(restart_in);
            Sleep.StartDeepSleep();
            throw new Exception("Impossible exception");
        }
        private static void EnsureUtcNowIsValid()
        {
            if(DateTime.UtcNow.Year > 2025)
            {
                Debug.WriteLine("System time is UTC");
                return;
            }
            Debug.WriteLine("System time is not UTC");
            EnterDeepSleep(TimeSpan.FromMinutes(5));
        }
        private static void StartMeasuringData()
        {
            MeasuringWork.Start();
        }
        private static void StartMqttInteractor()
        {
            MqttInteractor.Start();
        }
        public static void Main()
        {
            Debug.WriteLine("Start of program");
            SetupLogs();
            SetupButton();
            StartDhcpServerIfWirelessAPEnabled();
            EnableTimeoutForWirelessAP();
            SetupTooglingWebServerBasedOnAP();
            if(WirelessAP.Enabled is true)
            {
                Debug.WriteLine("Device mode: Configuration");
                Thread.Sleep(Timeout.Infinite);
            }
            Debug.WriteLine("Device mode: Normal");
            TryReconnectToWifi();
            EnsureUtcNowIsValid();
            StartMeasuringData();
            StartMqttInteractor();
            EnterDeepSleep(TimeSpan.FromSeconds(30));
        }
    }
}