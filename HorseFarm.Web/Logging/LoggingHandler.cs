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
            dynamic keys = new
                {
                    HttpMethod = request.Method.Method,
                    UriAccessed = request.RequestUri.AbsoluteUri,
                    AbsolutePath = request.RequestUri.AbsolutePath,
                    IpAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0",
                    MessageType = "Request",
                    Headers = ExtractMessageHeadersIntoLoggingInfo(request.Headers.ToList()),
                    MessageId = (int)EventClassification.WebServiceRequest
                    
                };

            var e = new HorseServiceEvent(EventClassification.WebServiceRequest.ToString()).WithKeys(new EventKeys
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

        private string ExtractMessageHeadersIntoLoggingInfo(List<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            var headersString = new StringBuilder();

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


                headersString.AppendFormat("{1}: {0}", h.Key, headerValues.ToString());
            });
            return headersString.ToString();
        }
    }

    public class DynamicKeys : DynamicObject
    {

    }
}