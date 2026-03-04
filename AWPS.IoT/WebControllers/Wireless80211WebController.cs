#nullable enable

using System.Net;
using System.Text;
using System.Device.Wifi;
using System.Diagnostics;
using nanoFramework.Json;
using nanoFramework.WebServer;
using AWPS.IoT.WebControllers.Models;

namespace AWPS.IoT.WebControllers
{
    public sealed class Wireless80211WebController
    {
        [Route("wifi")]
        [Method("GET")]
        public void GetWifiStatus(WebServerEventArgs event_args)
        {
            Debug.WriteLine("GET Request on path 'wifi'");
            GetWifiStatusResponseModel response = new()
            {
                SSID = Wireless80211.GetConfiguration().Ssid,
                Connected = Wireless80211.Connected
            };
            string content = JsonConvert.SerializeObject(response);
            Debug.WriteLine($"GET response on path 'wifi': {content}");
            event_args.Context.Response.StatusCode = (int)HttpStatusCode.OK;
            event_args.Context.Response.StatusDescription = "OK";
            event_args.Context.Response.ContentType = "application/json";
            WebServer.OutputAsStream(event_args.Context.Response, content);
        }

        [Route("wifi")]
        [Method("POST")]
        public void PostWifi(WebServerEventArgs event_args)
        {
            byte[] data = event_args.Context.Request.ReadBody();
            string content = Encoding.UTF8.GetString(data, 0, data.Length);
            Debug.WriteLine($"POST Request on path 'wifi': {content}");
            try
            {
                var request = (PostWifiRequestModel)JsonConvert.DeserializeObject(content, typeof(PostWifiRequestModel));
                WifiConnectionStatus status = Wireless80211.TryConnect(request.SSID, request.Password);
                PostWifiResponseModel response = new()
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
                };
                content = JsonConvert.SerializeObject(response);
                Debug.WriteLine($"POST response on path 'wifi': {content}");
                event_args.Context.Response.StatusCode = (int)HttpStatusCode.OK;
                event_args.Context.Response.StatusDescription = "OK";
                event_args.Context.Response.ContentType = "application/json";
                WebServer.OutputAsStream(event_args.Context.Response, content);
            }
            catch
            {
                Debug.WriteLine($"POST response on path 'wifi': StatusCode = 400; Description = 'Invalid body'");
                event_args.Context.Response.StatusDescription = "Invalid body";
                WebServer.OutputHttpCode(event_args.Context.Response, HttpStatusCode.BadRequest);
            }
        }
    }
}