using System.Net;
using System.Diagnostics;
using nanoFramework.WebServer;

namespace AWPS.IoT.WebControllers
{
    public static class RootWebController
    {
        [Route("")]
        [Method("GET")]
        public static void Ping(WebServerEventArgs event_args)
        {
            Debug.WriteLine("'GET' request on path '/'");
            WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            Debug.WriteLine("'GET' response on path '/'");
        }
    }
}