using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using HorseFarm.Web.Logging;

namespace HorseFarm.Web.Logging
{
    // Code based on: http://weblogs.asp.net/pglavich/archive/2012/02/26/asp-net-web-api-request-response-usage-logging.aspx
    public class LoggingHandler : DelegatingHandler
    {

        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            // Log the request information
            LogRequestLoggingInfo(request);

            // Execute the request
            var response = base.SendAsync(request, cancellationToken);

            response.ContinueWith((responseMsg) =>
            {
                // Extract the response logging info then persist the information
                //  LogResponseLoggingInfo(responseMsg.Result);
            });

            return response;
        }

        private void LogRequestLoggingInfo(HttpRequestMessage request)
        {

            dynamic keys = new ExpandoObject();
            keys.HttpMethod = request.Method.Method;
            keys.UriAccessed = request.RequestUri.AbsoluteUri;
            keys.IpAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0";
            keys.MessageType = "Request";

            ExtractMessageHeadersIntoLoggingInfo(keys, request.Headers.ToList());




            var e =
            new HorseServiceEvent(request.RequestUri.AbsolutePath).WithKeys(new EventKeys
                {
                    ExceedClientId = "asdf",
                    PolicyNumber = "123"
                });


            if (request.Content != null)
            {
                request.Content.ReadAsByteArrayAsync()
                    .ContinueWith((task) =>
                    {
                        //           keys.BodyContent = UTF8Encoding.UTF8.GetString(task.Result);
                        e.Raise("Service Accessed", keys);

                    });

                return;
            }

            e.Raise("Service Accessed", keys);
        }

        //private void LogResponseLoggingInfo(HttpResponseMessage response)
        //{
        //    var info = new ApiLoggingInfo();
        //    info.MessageType = HttpMessageType.Response;
        //    info.HttpMethod = response.RequestMessage.Method.ToString();
        //    info.ResponseStatusCode = response.StatusCode;
        //    info.ResponseStatusMessage = response.ReasonPhrase;
        //    info.UriAccessed = response.RequestMessage.RequestUri.AbsoluteUri;
        //    info.IpAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0";

        //    ExtractMessageHeadersIntoLoggingInfo(info, response.Headers.ToList());

        //    if (response.Content != null)
        //    {
        //        response.Content.ReadAsByteArrayAsync()
        //            .ContinueWith(t =>
        //            {
        //                var responseMsg = System.Text.UTF8Encoding.UTF8.GetString(t.Result);
        //                info.BodyContent = responseMsg;
        //                _repository.Log(info);
        //            });

        //        return;
        //    }

        //    _repository.Log(info);
        //}

        private void ExtractMessageHeadersIntoLoggingInfo(dynamic info, List<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            var dictionary = (IDictionary<string, object>)info;

            headers.ForEach(h =>
            {
                // convert the header values into one long string from a series of IEnumerable<string> values so it looks for like a HTTP header
                var headerValues = new StringBuilder();

                if (h.Value != null)
                {
                    foreach (var hv in h.Value)
                    {
                        if (headerValues.Length > 0)
                        {
                            headerValues.Append(", ");
                        }
                        headerValues.Append(hv);
                    }
                }


                dictionary.Add(h.Key, headerValues.ToString());
            });
        }
    }

    public class DynamicKeys : DynamicObject
    {

    }
}