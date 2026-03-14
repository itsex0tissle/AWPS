#nullable enable

using System.Net;
using System.Device.Wifi;
using System.Diagnostics;
using nanoFramework.WebServer;
using AWPS.IoT.WebControllers.Records;

namespace AWPS.IoT.WebControllers
{
    public sealed class Wireless80211WebController
    {
        [Route("wifi")]
        [Method("GET")]
        public void GetWifiStatus(WebServerEventArgs event_args)
        {
            Debug.WriteLine("GET request on path 'wifi'");
            ResponseHelper.SendStream(event_args.Context.Response, "application/octet-stream", new GetWifiStatusResponseRecord()
            {
                Connected = Wireless80211.Connected,
                SSID = Wireless80211.GetConfiguration().Ssid
            }.Serialize());
            Debug.WriteLine($"GET response on path 'wifi'");
        }

        [Route("wifi")]
        [Method("POST")]
        public void PostWifi(WebServerEventArgs event_args)
        {
            Debug.WriteLine("POST request on path 'wifi'");
            byte[] data = event_args.Context.Request.ReadBody();
            try
            {
                PostWifiRequestRecord request = new();
                request.Deserialize(data);
                WifiConnectionStatus status = Wireless80211.TryConnect(request.SSID, request.Password);
                ResponseHelper.SendStream(event_args.Context.Response, "application/octet-stream", new PostWifiResponseRecord()
                {
                    Success = status is WifiConnectionStatus.Success,
                    Description = status switch
                    {
                        WifiConnectionStatus.AccessRevoked => "Access revoked",
                        WifiConnectionStatus.InvalidCredential => "Invalid password",
                        WifiConnectionStatus.NetworkNotAvailable => "Network not found",
                        WifiConnectionStatus.Success => "Success",
                        WifiConnectionStatus.Timeout => "Timeout",
                        WifiConnectionStatus.UnsupportedAuthenticationProtocol => "Unsupported authentication protocol",
                        _ => "Unexpected error"
                    }
                }.Serialize());
            }
            catch
            {
                ResponseHelper.SendStatusCode(event_args.Context.Response, HttpStatusCode.BadRequest);
            }
            Debug.WriteLine("POST response on path 'wifi'");
        }

        [Route("wifi")]
        [Method("OPTIONS")]
        public void OptionsWifi(WebServerEventArgs event_args)
        {
            Debug.WriteLine("OPTIONS request on path 'wifi'");
            ResponseHelper.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            Debug.WriteLine("OPTIONS response on path 'wifi'");
        }
    }
}