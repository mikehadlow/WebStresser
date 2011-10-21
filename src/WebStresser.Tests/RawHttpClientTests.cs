using System;

namespace WebStresser.Tests
{
    public class RawHttpClientTests
    {
        private static readonly Uri serviceUri = new Uri("http://localhost:8123/hello");
        private const string action = "http://tempuri.org/ITimerService/Wait";
        private const int iterations = 900;
        private const int intervalMilliseconds = 0;

        const string soapEnvelope =
@"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
<s:Body>
    <Wait xmlns=""http://tempuri.org/"">
        <correlationId>101</correlationId>
        <millisecondsToWait>1000</millisecondsToWait>
    </Wait>
</s:Body>
</s:Envelope>";

        /// <summary>
        /// Make sure the TestTimerService is running before trying this test.
        /// </summary>
        public static void Call_TestTimerService()
        {
            var configuration = new HttpCallConfiguration
            {
                ServiceUri = serviceUri,
                Method = HttpMethod.POST,
                Iterations = iterations,
                IntervalMilliseconds = intervalMilliseconds,
                PostData = soapEnvelope,
                PrintResponse = false,
                Expect100Continue = false,
                UseNagleAlgorithm = false,
                KeepAlive = true
            };

            configuration.Headers.Add("SOAPAction", action);
            var client = new RawHttpClient(configuration, Console.Out);

            client.MakeRawHttpCall();
        }
    }
}