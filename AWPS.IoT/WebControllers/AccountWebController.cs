using System;
using System.Net;
using System.Text;
using AWPS.IoT.Files;
using AWPS.IoT.Models;
using AWPS.IoT.Services;
using nanoFramework.Json;
using nanoFramework.WebServer;

namespace AWPS.IoT.WebControllers
{
    public static class AccountWebController
    {
        [Route("account")]
        [Method("GET")]
        public static void GetAccount(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'GET' request on path '/account'");
            WebController.SendJson(event_args.Context.Response, AccountFile.Record);
            Logger.LogInfo("'GET' response on path '/account'");
        }

        [Route("account")]
        [Method("POST")]
        public static void PostAccount(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'POST' request on path '/account'");
            try
            {
                byte[] data = event_args.Context.Request.ReadBody();
                string json = Encoding.UTF8.GetString(data, 0, data.Length);
                AccountFile.Record = (AccountRecord)JsonConvert.DeserializeObject(json, typeof(AccountRecord));
                WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            }
            catch(Exception exception)
            {
                Logger.LogException(exception);
                WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.BadRequest);
            }
            finally
            {
                Logger.LogInfo("'POST' response on path '/account'");
            }
            Logger.LogInfo("'POST' response on path '/account'");
        }

        [Route("account")]
        [Method("OPTIONS")]
        public static void PostAccountHeaders(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'OPTIONS' request on path '/account'");
            WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            Logger.LogInfo("'OPTIONS' response on path '/account'");
        }

        [Route("account")]
        [Method("DELETE")]
        public static void DeleteAccount(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'DELETE' request on path '/account'");
            AccountFile.Reset();
            WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            Logger.LogInfo("'DELETE' response on path '/account'");
        }
    }
}