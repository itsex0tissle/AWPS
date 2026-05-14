using AWPS.IoT.Files;
using AWPS.IoT.Models;
using AWPS.IoT.Services;
using nanoFramework.Json;
using nanoFramework.WebServer;
using System;
using System.Net;
using System.Text;

namespace AWPS.IoT.WebControllers
{
    public static class SettingsWebController
    {
        [Route("settings")]
        [Method("GET")]
        public static void GetSettings(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'GET' request on path '/settings'");
            WebController.SendJson(event_args.Context.Response, SettingsFile.Record);
            Logger.LogInfo("'GET' response on path '/settings'");
        }

        [Route("settings")]
        [Method("PUT")]
        public static void PutSettings(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'PUT' request on path '/settings'");
            try
            {
                byte[] data = event_args.Context.Request.ReadBody();
                string json = Encoding.UTF8.GetString(data, 0, data.Length);
                SettingsFile.Record = (SettingsRecord)JsonConvert.DeserializeObject(json, typeof(SettingsRecord));
                WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
                WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.BadRequest);
            }
            finally
            {
                Logger.LogInfo("'PUT' response on path '/settings'");
            }
            Logger.LogInfo("'PUT' response on path '/settings'");
        }

        [Route("settings")]
        [Method("OPTIONS")]
        public static void PutSettingsHeaders(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'OPTIONS' request on path '/settings'");
            WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            Logger.LogInfo("'OPTIONS' response on path '/settings'");
        }

        [Route("settings")]
        [Method("DELETE")]
        public static void ResetSettings(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'DELETE' request on path '/settings'");
            SettingsFile.Reset();
            WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            Logger.LogInfo("'DELETE' response on path '/settings'");
        }
    }
}