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

#nullable enable

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
            if(Sleep.GetWakeupCause() is Sleep.WakeupCause.Timer)
            {
                Debug.WriteLine("Wakeup cause is timer. No sense to enable button at all");
                return;
            }
            GpioButton button = new(21, TimeSpan.FromTicks(15000000L), TimeSpan.FromSeconds(3))
            {
                IsHoldingEnabled = true
            };
            button.Press += ToogleWirelessAP;
            button.Holding += ResetMeasuringData;
            Debug.Write("Button enabled");
            if(WirelessAP.Enabled is false)
            {
                Debug.WriteLine(". Disabling in 10s");
                Thread.Sleep(10000);
                button.Press -= ToogleWirelessAP;
                button.Holding -= ResetMeasuringData;
                button.Dispose();
                Debug.WriteLine("Button disabled");
                return;
            }
            Debug.WriteLine();

            static void ToogleWirelessAP(object sender, EventArgs event_args)
            {
                if(WirelessAP.Enabled is false)
                {
                    WirelessAP.Enable();
                }
                else
                {
                    WirelessAP.Disable();
                }
            }
            static void ResetMeasuringData(object sender, ButtonHoldingEventArgs event_args)
            {
                if(event_args.HoldingState is ButtonHoldingState.Started)
                {
                    MeasuringDataFile.Reset();
                }
            }
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
                    if(WebServer.IsRunning is true)
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