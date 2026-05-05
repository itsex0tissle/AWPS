using System.Net;
using System.Text;
using nanoFramework.Json;
using nanoFramework.MessagePack;

namespace AWPS.IoT.WebControllers
{
    public static class WebController
    {
        public const string AllowOrigin = "*";
        public const string AllowHeaders = "*";
        public const string AllowMethods = "GET, POST, OPTIONS, DELETE";

        public static void SetHeaders(HttpListenerResponse response)
        {
            response.Headers.Set("Access-Control-Allow-Origin", AllowOrigin);
            response.Headers.Set("Access-Control-Allow-Headers", AllowHeaders);
            response.Headers.Set("Access-Control-Allow-Methods", AllowMethods);
        }
        public static void SendStatusCode(HttpListenerResponse response, HttpStatusCode status_code)
        {
            SetHeaders(response);
            response.ContentType = null;
            response.ContentLength64 = 0;
            response.StatusCode = (int)status_code;
            response.Close();
        }
        public static void SendStream(HttpListenerResponse response, byte[] content, string content_type = "application/octet-stream")
        {
            SetHeaders(response);
            response.ContentType = content_type;
            response.ContentLength64 = content.Length;
            response.OutputStream.Write(content, 0, content.Length);
        }
        public static void SendString(HttpListenerResponse response, string content, string content_type = "plain/text")
        {
            SendStream(response, Encoding.UTF8.GetBytes(content), content_type);
        }
        public static void SendJson(HttpListenerResponse response, object obj)
        {
            SendString(response, JsonConvert.SerializeObject(obj), "application/json");
        }
        public static void SendObject(HttpListenerResponse response, object? obj)
        {
            SendStream(response, MessagePackSerializer.Serialize(obj), "application/x-msgpack");
        }
    }
}