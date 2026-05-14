using System.Net;
using AWPS.IoT.Files;
using AWPS.IoT.Services;
using nanoFramework.WebServer;

namespace AWPS.IoT.WebControllers
{
    public static class TelemetryWebController
    {
        [Route("telemetry")]
        [Method("GET")]
        public static void GetTelemetry(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'GET' request on path '/telemetry'");
            WebController.SendObject(event_args.Context.Response, TelemetryFile.Records);
            Logger.LogInfo("'GET' response on path '/telemetry'");
        }

        [Route("telemetry")]
        [Method("DELETE")]
        public static void ClearTelemetry(WebServerEventArgs event_args)
        {
            Logger.LogInfo("'DELETE' request on path '/telemetry'");
            TelemetryFile.Reset();
            WebController.SendStatusCode(event_args.Context.Response, HttpStatusCode.OK);
            Logger.LogInfo("'DELETE' response on path '/telemetry'");
        }
    }
}