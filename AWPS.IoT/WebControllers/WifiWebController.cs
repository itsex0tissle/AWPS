using System;
using System.Net;
using System.Collections;
using System.Device.Wifi;
using System.Diagnostics;
using AWPS.IoT.MsgPack.Models;
using nanoFramework.WebServer;
using nanoFramework.MessagePack;
using AWPS.IoT.Services;

namespace AWPS.IoT.WebControllers
{
    public static class WifiWebController
    {
        [Route("wifi")]
        [Method("GET")]
        public static void GetWifiState(WebServerEventArgs event_args)
        {
            Debug.WriteLine("'GET' request on path '/wifi'");
            WebController.SendObject(event_args.Context.Response, new WifiStateRecord()
            {
                SSID = Wireless80211.GetConfiguration().Ssid,
                Connected = Wireless80211.Connected
            });
            Debug.WriteLine("'GET' response on path '/wifi'");
        }

        [Route("wifi/list")]
        [Method("GET")]
        public static void GetWifiList(WebServerEventArgs event_args)
        {
            Debug.WriteLine("'GET' request on path '/wifi/list'");
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
            WebController.SendObject(event_args.Context.Response, list);
            Debug.WriteLine("'GET' response on path '/wifi/list'");
        }

        [Route("wifi")]
        [Method("POST")]
        public static void PostWifi(WebServerEventArgs event_args)
        {
            Debug.WriteLine("'POST' request on path '/wifi'");
            try
            {
                if(MessagePackSerializer.Deserialize(typeof(WifiCredentialsRecord), event_args.Context.Request.ReadBody()) is not WifiCredentialsRecord request)
                {
                    WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.BadRequest);
                    return;
                }
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
                WebController.SendObject(event_args.Context.Response, response);
            }
            catch(Exception exc)
            {
                Debug.WriteLine($"'{nameof(PostWifi)}' action failed. Exception: {exc}");
                WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.BadRequest);
            }
            finally
            {
                Debug.WriteLine("'POST' response on path '/wifi'");
            }
        }

        [Route("wifi")]
        [Method("OPTIONS")]
        public static void PostWifiHeaders(WebServerEventArgs event_args)
        {
            Debug.WriteLine("'OPTIONS' request on path '/wifi'");
            WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            Debug.WriteLine("'OPTIONS' response on path '/wifi'");
        }

        [Route("wifi/save")]
        [Method("POST")]
        public static void SaveWifi(WebServerEventArgs event_args)
        {
            Debug.WriteLine("'POST' request on path '/wifi/save'");
            try
            {
                if (MessagePackSerializer.Deserialize(typeof(WifiCredentialsRecord), event_args.Context.Request.ReadBody()) is not WifiCredentialsRecord request)
                {
                    WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.BadRequest);
                    return;
                }
                Wireless80211.SaveCredentials(request.SSID, request.Password);
                WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            }
            catch(Exception exc)
            {
                Debug.WriteLine($"'{nameof(SaveWifi)}' action failed. Exception: {exc}");
                WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.BadRequest);
            }
            finally
            {
                Debug.WriteLine("'POST' response on path '/wifi/save'");
            }
        }

        [Route("wifi/save")]
        [Method("OPTIONS")]
        public static void SaveWifiHeaders(WebServerEventArgs event_args)
        {
            Debug.WriteLine("'OPTIONS' request on path '/wifi/save'");
            WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            Debug.WriteLine("'OPTIONS' response on path '/wifi/save'");
        }
    }
}