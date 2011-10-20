using System;

namespace WebStresser.Tests
{
    public class RawHttpClientTests
    {
        private static readonly Uri serviceUri = new Uri("http://localhost:8123/hello");
        private const string action = "http://tempuri.org/ITimerService/Wait";
        private const int iterations = 1;
        private const int intervalMilliseconds = 4;

        const string soapEnvelope =
@"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
<s:Body>
    <GetCustomerDetails xmlns=""http://tempuri.org/"">
        <correlationId>101</correlationId>
        <millisecondsToWait>1000</millisecondsToWait>
    </GetCustomerDetails>
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
                PostData = soapEnvelope
            };

            configuration.Headers.Add("SOAPAction", action);
            var client = new RawHttpClient(configuration, Console.Out);

            client.MakeRawHttpCall();
        }
    }
}