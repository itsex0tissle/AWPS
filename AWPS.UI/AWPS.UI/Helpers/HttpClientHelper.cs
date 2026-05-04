#if ANDROID
using Xamarin.Android.Net;
#endif

namespace AWPS.UI.Helpers;

public static class HttpClientHelper
{
    #region Static
    public static string BaseUrl
    {
        get
        {
#if DEBUG
            if(DeviceInfo.Platform == DevicePlatform.Android)
            {
                field = field.Replace("localhost", "10.0.2.2");
            }
#endif
            return field;
        }
    } = "https://localhost:7037";

    public static HttpClient CreatePlatformHttpClient()
    {
#if WINDOWS || MACCATALYST
        return new HttpClient()
#else
        return new HttpClient(HttpsClientHandlerService.CreatePlatformMessageHandler())
#endif
        {
            BaseAddress = new Uri(BaseUrl),
        };
    }
    #endregion

    #region Types
    public static class HttpsClientHandlerService
    {
        public static HttpMessageHandler CreatePlatformMessageHandler()
        {
#if ANDROID
            return new AndroidMessageHandler()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    if(cert is not null && cert.Issuer.Equals("CN=localhost"))
                    {
                        return true;
                    }
                    return errors is System.Net.Security.SslPolicyErrors.None;
                }
            };
#elif IOS
            return new NSUrlSessionHandler()
            {
                TrustOverrideForUrl = IsHttpsLocalhost
            };
#else
            throw new PlatformNotSupportedException("Only Android and iOS supported.");
#endif
        }
#if IOS
        public static bool IsHttpsLocalhost(NSUrlSessionHandler sender, string url, Security.SecTrust trust)
        {
            return url.StartsWith("https://localhost");
        }
#endif
    }
    #endregion
}