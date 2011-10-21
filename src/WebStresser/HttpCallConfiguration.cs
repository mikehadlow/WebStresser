using System;
using System.Collections.Generic;

namespace WebStresser
{
    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        HEAD,
        DELETE
    }

    public class HttpCallConfiguration
    {
        public Uri ServiceUri { get; set; }
        public IDictionary<string, string> Headers { get; private set; }
        public int Iterations { get; set; }
        public int IntervalMilliseconds { get; set; }
        public string PostData { get; set;  }

        public bool PrintResponse { get; set; }
        public bool KeepAlive { get; set; }
        public string Accept { get; set; }
        public string ContentType { get; set; }
        public int TimeoutMilliseconds { get; set; }
        public HttpMethod Method { get; set; }
        public string MethodAsString
        {
            get { return Enum.GetName(typeof (HttpMethod), Method); }
        }

        public bool Expect100Continue { get; set; }
        public bool UseNagleAlgorithm { get; set; }

        public HttpCallConfiguration()
        {
            Method = HttpMethod.GET;
            Iterations = 1;
            IntervalMilliseconds = 10000;

            Headers = new Dictionary<string, string>();
            PrintResponse = false;
            KeepAlive = true;
            Accept = "text/xml";
            ContentType = "text/xml;charset=\"utf-8\"";
            TimeoutMilliseconds = 10000;

            // http://computercabal.blogspot.com/2007/09/httpwebrequest-in-c-for-web-traffic.html
            Expect100Continue = false;
            UseNagleAlgorithm = false;
        }

        public bool IsValid()
        {
            return
                (ServiceUri != null);
        }
    }
}