#nullable enable

using System.Net;
using System.Diagnostics;
using nanoFramework.WebServer;

namespace AWPS.IoT.WebControllers
{
    public sealed class RootWebController
    {
        [Route("")]
        [Method("GET")]
        public void GetPing(WebServerEventArgs event_args)
        {
            Debug.WriteLine("GET request on path ''");
            ResponseHelper.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            Debug.WriteLine("GET response on path ''");
        }
    }
}