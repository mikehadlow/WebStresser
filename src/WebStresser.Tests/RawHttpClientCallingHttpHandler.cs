using System;

namespace WebStresser.Tests
{
    public class RawHttpClientCallingHttpHandler
    {
        private static readonly Uri serviceUri = new Uri("http://timerhandler/timer");
        private const int iterations = 100;
        private const int intervalMilliseconds = 0;

        /// <summary>
        /// Make sure the TestHttpHandler is running before trying this test.
        /// </summary>
        public static void Call_TestHttpHandler()
        {
            var configuration = new HttpCallConfiguration
            {
                ServiceUri = serviceUri,
                Method = HttpMethod.GET,
                Iterations = iterations,
                IntervalMilliseconds = intervalMilliseconds,
                PrintResponse = false,
                Expect100Continue = false,
                UseNagleAlgorithm = false,
                KeepAlive = true
            };

            var client = new RawHttpClient(configuration, Console.Out);

            client.MakeRawHttpCall();
        }
    }
}