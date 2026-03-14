#nullable enable

using System;
using System.Threading;
using System.Device.Wifi;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace AWPS.IoT
{
    public static class Wireless80211
    {
        public static WifiAdapter GetAdapter()
        {
            return WifiAdapter.FindAllAdapters()[0];
        }
        public static NetworkInterface GetInterface()
        {
            foreach(NetworkInterface network_interface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if(network_interface.NetworkInterfaceType is NetworkInterfaceType.Wireless80211)
                {
                    return network_interface;
                }
            }
            throw new PlatformNotSupportedException();
        }
        public static Wireless80211Configuration GetConfiguration()
        {
            return Wireless80211Configuration.GetAllWireless80211Configurations()[GetInterface().SpecificConfigId];
        }

        public static bool Connected
        {
            get => GetInterface().IPv4Address.StartsWith("0") is false;
        }

        public static WifiConnectionStatus TryConnect(string ssid, string password)
        {
            WifiAdapter adapter = GetAdapter();
            adapter.Disconnect();
            bool status_got = false;
            WifiConnectionStatus result = default;
            adapter.AvailableNetworksChanged += ConnectToWifi;
            adapter.ScanAsync();
            while(status_got is false)
            {
                Thread.Sleep(200);
            }
            Wireless80211Configuration configuration = GetConfiguration();
            if(result is not WifiConnectionStatus.Success)
            {
                Debug.WriteLine($"Failed to connect to wifi network: {ssid}");
                TryConnect(configuration.Ssid, configuration.Password);
            }
            else
            {
                Debug.WriteLine($"Connected to wifi network: {ssid}");
                configuration.Ssid = ssid;
                configuration.Password = password;
                configuration.SaveConfiguration();
            }
            return result;

            void ConnectToWifi(WifiAdapter sender, object event_args)
            {
                try
                {
                    result = sender.Connect(ssid, WifiReconnectionKind.Automatic, password).ConnectionStatus;
                }
                catch(Exception ex)
                {
                    result = WifiConnectionStatus.UnspecifiedFailure;
                    Debug.WriteLine($"Failed to connect to wifi network: SSID = {ssid}; Exception = {ex}");
                }
                finally
                {
                    adapter.AvailableNetworksChanged -= ConnectToWifi;
                    status_got = true;
                }
            }
        }
    }
}