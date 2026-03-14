using System;
using System.Diagnostics;
using nanoFramework.Runtime.Native;
using System.Net.NetworkInformation;

namespace AWPS.IoT
{
    public static class WirelessAP
    {
        public const string IP = "192.168.4.1";
        public const string Mask = "255.255.255.0";
        public const string SSID = "AutoWateringDevice";

        public static NetworkInterface GetInterface()
        {
            foreach(NetworkInterface network_interface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if(network_interface.NetworkInterfaceType is NetworkInterfaceType.WirelessAP)
                {
                    return network_interface;
                }
            }
            throw new PlatformNotSupportedException();
        }
        public static WirelessAPConfiguration GetConfiguration()
        {
            return WirelessAPConfiguration.GetAllWirelessAPConfigurations()[GetInterface().SpecificConfigId];
        }
        
        public static bool Enabled
        {
            get => (GetConfiguration().Options & WirelessAPConfiguration.ConfigurationOptions.Enable) != 0;
        }

        public static void Enable()
        {
            if(Enabled is true)
            {
                return;
            }
            GetInterface().EnableStaticIPv4(IP, Mask, IP);
            WirelessAPConfiguration configuration = GetConfiguration();
            configuration.Ssid = SSID;
            configuration.Password = "";
            configuration.MaxConnections = 1;
            configuration.Authentication = AuthenticationType.Open;
            configuration.Encryption = EncryptionType.None;
            configuration.Options = WirelessAPConfiguration.ConfigurationOptions.AutoStart;
            configuration.SaveConfiguration();
            Debug.WriteLine("WirelessAP enabled");
            Power.RebootDevice();
        }
        public static void Disable()
        {
            if(Enabled is false)
            {
                return;
            }
            WirelessAPConfiguration configuration = GetConfiguration();
            configuration.Options = WirelessAPConfiguration.ConfigurationOptions.Disable;
            configuration.SaveConfiguration();
            Debug.WriteLine("WirelessAP disabled");
            Power.RebootDevice();
        }
    }
}