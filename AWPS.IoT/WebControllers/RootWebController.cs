using System.Net;
using AWPS.IoT.Services;
using nanoFramework.WebServer;

namespace AWPS.IoT.WebControllers
{
    public static class RootWebController
    {
        [Route("")]
        [Method("GET")]
        public static void Ping(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'GET' request on path '/'");
            WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            Logger.LogInfo("'GET' response on path '/'");
        }
    }
}