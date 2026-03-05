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
            byte[] response = new GetWifiStatusResponseRecord()
            {
                Connected = Wireless80211.Connected,
                SSID = Wireless80211.GetConfiguration().Ssid
            }.Serialize();
            event_args.Context.Response.StatusCode = (int)HttpStatusCode.OK;
            event_args.Context.Response.StatusDescription = "OK";
            event_args.Context.Response.ContentType = "application/octet-stream";
            event_args.Context.Response.ContentLength64 = response.Length;
            event_args.Context.Response.OutputStream.Write(response, 0, response.Length);
            Debug.WriteLine("GET response on path 'wifi'");
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
                data = new PostWifiResponseRecord()
                {
                    Success = status is WifiConnectionStatus.Success,
                    Description = status switch
                    {
                        WifiConnectionStatus.AccessRevoked => "Access revoked",
                        WifiConnectionStatus.InvalidCredential => "Invalid password",
                        WifiConnectionStatus.NetworkNotAvailable => "Network not found",
                        WifiConnectionStatus.Timeout => "Timeout",
                        WifiConnectionStatus.UnsupportedAuthenticationProtocol => "Unsupported authentication protocol",
                        _ => "Unexpected error"
                    }
                }.Serialize();
                event_args.Context.Response.StatusCode = (int)HttpStatusCode.OK;
                event_args.Context.Response.StatusDescription = "OK";
                event_args.Context.Response.ContentType = "application/octet-stream";
                event_args.Context.Response.ContentLength64 = data.Length;
                event_args.Context.Response.OutputStream.Write(data, 0, data.Length);
                Debug.WriteLine("POST response on path 'wifi'");
            }
            catch
            {
                Debug.WriteLine("POST response on path 'wifi': StatusCode = 400; Description = 'Invalid body'");
                event_args.Context.Response.StatusDescription = "Invalid body";
                WebServer.OutputHttpCode(event_args.Context.Response, HttpStatusCode.BadRequest);
            }
        }
    }
}