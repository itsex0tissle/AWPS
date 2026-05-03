using System;
using System.Net;
using System.Text;
using AWPS.IoT.Models;
using AWPS.IoT.Services;
using System.Collections;
using System.Device.Wifi;
using nanoFramework.Json;
using nanoFramework.WebServer;

namespace AWPS.IoT.WebControllers
{
    public static class WifiWebController
    {
        [Route("wifi")]
        [Method("GET")]
        public static void GetWifiState(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'GET' request on path '/wifi'");
            WebController.SendJson(event_args.Context.Response, new WifiStateRecord()
            {
                SSID = Wireless80211.GetConfiguration().Ssid,
                Connected = Wireless80211.Connected
            });
            Logger.LogInfo("'GET' response on path '/wifi'");
        }

        [Route("wifi/list")]
        [Method("GET")]
        public static void GetWifiList(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'GET' request on path '/wifi/list'");
            ArrayList list = new();
            foreach(WifiAvailableNetwork network in Wireless80211.GetAvailableNetworks())
            {
                WifiAvailableNetworkRecord response = new()
                {
                    SSID = network.Ssid,
                    SignalBars = network.SignalBars
                };
                list.Add(response);
            }
            WebController.SendJson(event_args.Context.Response, list);
            Logger.LogInfo("'GET' response on path '/wifi/list'");
        }

        [Route("wifi")]
        [Method("POST")]
        public static void PostWifi(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'POST' request on path '/wifi'");
            try
            {
                byte[] data = event_args.Context.Request.ReadBody();
                string json = Encoding.UTF8.GetString(data, 0, data.Length);
                var request = (WifiCredentialsRecord)JsonConvert.DeserializeObject(json, typeof(WifiCredentialsRecord));
                WifiConnectionStatus status = Wireless80211.TryConnect(request.SSID, request.Password);
                WifiConnectionResultRecord response = new()
                {
                    Connected = status is WifiConnectionStatus.Success,
                    Message = status switch
                    {
                        WifiConnectionStatus.AccessRevoked => "Access revoked",
                        WifiConnectionStatus.InvalidCredential => "Invalid password",
                        WifiConnectionStatus.NetworkNotAvailable => "Network not available",
                        WifiConnectionStatus.Success => "Success",
                        WifiConnectionStatus.Timeout => "Timeout",
                        WifiConnectionStatus.UnsupportedAuthenticationProtocol => "Unsupported authentication protocol",
                        _ => "Unexpected error"
                    }
                };
                WebController.SendJson(event_args.Context.Response, response);
            }
            catch(Exception exception)
            {
                Logger.LogException(exception);
                WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.BadRequest);
            }
            finally
            {
                Logger.LogInfo("'POST' response on path '/wifi'");
            }
        }

        [Route("wifi")]
        [Method("OPTIONS")]
        public static void PostWifiHeaders(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'OPTIONS' request on path '/wifi'");
            WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            Logger.LogInfo("'OPTIONS' response on path '/wifi'");
        }

        [Route("wifi/save")]
        [Method("POST")]
        public static void SaveWifi(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'POST' request on path '/wifi/save'");
            try
            {
                byte[] data = event_args.Context.Request.ReadBody();
                string json = Encoding.UTF8.GetString(data, 0, data.Length);
                var request = (WifiCredentialsRecord)JsonConvert.DeserializeObject(json, typeof(WifiCredentialsRecord));
                Wireless80211.SaveCredentials(request.SSID, request.Password);
                WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            }
            catch(Exception exception)
            {
                Logger.LogException(exception);
                WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.BadRequest);
            }
            finally
            {
                Logger.LogInfo("'POST' response on path '/wifi/save'");
            }
        }

        [Route("wifi/save")]
        [Method("OPTIONS")]
        public static void SaveWifiHeaders(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'OPTIONS' request on path '/wifi/save'");
            WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            Logger.LogInfo("'OPTIONS' response on path '/wifi/save'");
        }
    }
}