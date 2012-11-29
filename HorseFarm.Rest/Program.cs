using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using System.IO;
using System.Web;

namespace HorseFarm.Rest
{
    class Program
    {
        static string token;
        static Uri namespaceBaseAddress;
        static void Main(string[] args)
        {
            namespaceBaseAddress = new Uri("https://Dev-PC:9355/ServiceBusDefaultNamespace/");
            Usage();
            GetQueue();


        }
        public static void GetQueue()
        {
            Uri requestUri = new Uri(namespaceBaseAddress, "SomethingQueue");
            HttpWebRequest request = WebRequest.Create(requestUri) as HttpWebRequest;
            request.MaximumAutomaticRedirections = 1;
            request.Method = "Get";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = 0;
            request.Headers.Add(HttpRequestHeader.Authorization, token);

            string body = string.Empty;

            using (var response = request.GetResponse() as HttpWebResponse)
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        body = reader.ReadToEnd();
                    }
                }
            }

            Console.WriteLine(body);

        }
        public static void Usage()
        {
            token = GetOAuthAccessToken(@"dev", "Star11pop", TimeSpan.FromMinutes(10));
            Console.WriteLine(token);


        }

        public static string GetOAuthAccessToken(string userName, string userPassword, TimeSpan timeout)
        {
            const int ServicePointMaxIdleTimeMilliSeconds = 50000;
            const string OAuthTokenServicePath = "$STS/OAuth/";
            const string ClientPasswordFormat = "grant_type=authorization_code&client_id={0}&client_secret={1}&scope={2}";

            Uri requestUri = new Uri(namespaceBaseAddress, OAuthTokenServicePath);
            string requestContent = string.Format(CultureInfo.InvariantCulture,
                ClientPasswordFormat, HttpUtility.UrlEncode(userName),
                HttpUtility.UrlEncode(userPassword),
                HttpUtility.UrlEncode(namespaceBaseAddress.AbsoluteUri));
            byte[] body = Encoding.UTF8.GetBytes(requestContent);

            HttpWebRequest request = WebRequest.Create(requestUri) as HttpWebRequest;
            request.ServicePoint.MaxIdleTime = ServicePointMaxIdleTimeMilliSeconds;
            request.AllowAutoRedirect = true;
            request.MaximumAutomaticRedirections = 1;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = body.Length;
            request.Timeout = Convert.ToInt32(timeout.TotalMilliseconds, CultureInfo.InvariantCulture);

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(body, 0, body.Length);
            }

            string rawAccessToken = null;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        rawAccessToken = reader.ReadToEnd();
                    }
                }
            }

            string simpleWebToken = string.Format(CultureInfo.InvariantCulture, "WRAP access_token=\"{0}\"", rawAccessToken);
            return simpleWebToken;

        }
    }
}
