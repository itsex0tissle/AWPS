#nullable enable

using System.Net;

namespace AWPS.IoT.WebControllers
{
    public static class ResponseHelper
    {
        public static void SetHeaders(HttpListenerResponse response)
        {
            response.Headers.Set("Access-Control-Allow-Origin", "*");
            response.Headers.Set("Access-Control-Allow-Headers", "*");
            response.Headers.Set("Access-Control-Allow-Methods", "GET, POST");
        }
        public static void SendStatusCode(HttpListenerResponse response, HttpStatusCode status_code)
        {
            SetHeaders(response);
            response.ContentType = null;
            response.ContentLength64 = 0;
            response.StatusCode = (int)status_code;
            response.Close();
        }
        public static void SendStream(HttpListenerResponse response, string content_type, byte[] content)
        {
            SetHeaders(response);
            response.ContentType = content_type;
            response.ContentLength64 = content.Length;
            response.OutputStream.Write(content, 0, content.Length);
        }
    }
}