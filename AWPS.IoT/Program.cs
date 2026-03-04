/* Top-Level packages:
 * nanoFramework.System.Net;
 * nanoFramework.Runtime.Native;
 * nanoFramework.IoT.Device.DhcpServer;
 * nanoFramework.IoT.Device.Button;
 * nanoFramework.WebServer;
 * nanoFramework.System.Device.Wifi;
 * nanoFramework.Json;
 */

#nullable enable

using System;
using System.Net;
using System.Threading;
using Iot.Device.Button;
using System.Diagnostics;
using Iot.Device.DhcpServer;
using nanoFramework.WebServer;
using AWPS.IoT.WebControllers;
using nanoFramework.Runtime.Native;
using System.Net.NetworkInformation;

namespace AWPS.IoT
{
    public static class Program
    {
        private static WebServer? WebServer { get; set; }

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
            GpioButton button = new(21, TimeSpan.FromTicks(15000000L), TimeSpan.FromSeconds(3))
            {
                IsHoldingEnabled = true
            };
            button.Press += delegate(object sender, EventArgs event_args)
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
            Debug.WriteLine("Button enabled");
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

        public static void Main()
        {
            Debug.WriteLine("Start of program");
            SetupLogs();
            SetupButton();
            StartDhcpServerIfWirelessAPEnabled();
            SetupTooglingWebServerBasedOnAP();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}