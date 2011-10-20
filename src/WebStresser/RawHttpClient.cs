using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace WebStresser
{
    public class RawHttpClient
    {
        private readonly HttpCallConfiguration configuration;
        private readonly TextWriter outputWriter;

        private int completed = 0;
        private int faulted = 0;

        private readonly object applicationFaulLock = new object();
        private bool applicationFault = false;

        private readonly List<long> elapsed = new List<long>();
        private readonly object elapsedLock = new object();

        public RawHttpClient(HttpCallConfiguration configuration, TextWriter outputWriter)
        {
            this.configuration = configuration;
            this.outputWriter = outputWriter;
        }

        public void MakeRawHttpCall()
        {
            // http://computercabal.blogspot.com/2007/09/httpwebrequest-in-c-for-web-traffic.html
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;

            var servicePoint = ServicePointManager.FindServicePoint(configuration.ServiceUri);

            // allow as many connections as the number of iterations
            // http://social.msdn.microsoft.com/Forums/en/ncl/thread/94ae61ec-08df-430b-a5d2-bb287a3acef0
            servicePoint.ConnectionLimit = configuration.Iterations;

            outputWriter.WriteLine("Starting test...");
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            ThreadPool.QueueUserWorkItem(ExecuteRequests);

            WaitUntilAllCallsAreComplete(servicePoint);

            stopwatch.Stop();

            outputWriter.WriteLine("Completed All {0:#,##0}", completed);
            outputWriter.WriteLine("Faulted {0:#,##0}", faulted);
            outputWriter.WriteLine("Elapsed ms {0:#,###}", stopwatch.ElapsedMilliseconds);
            outputWriter.WriteLine("Calls per second {0}", (completed * 1000) / stopwatch.ElapsedMilliseconds);
            if (elapsed.Count > 0)
            {
                outputWriter.WriteLine("Average call duration ms {0:#,###}", elapsed.Average());
            }
        }

        private void WaitUntilAllCallsAreComplete(ServicePoint servicePoint) {
            Thread.Sleep(10);
            var waitCount = 0;
            while (Thread.VolatileRead(ref completed) < configuration.Iterations && !applicationFault)
            {
                if(waitCount == 9)
                {
                    outputWriter.WriteLine("Completed: {0:#,##0} \tFaulted: {1:#,##0} \tConnections: {2}",
                                           completed, faulted, servicePoint.CurrentConnections);
                    waitCount = 0;
                }
                else
                {
                    waitCount++;
                }
                Thread.Sleep(10);    
            }
        }

        private void ExecuteRequests(object state)
        {
            try
            {
                for (var i = 0; i < configuration.Iterations; i++)
                {
                    ExecuteRequest();
                    Thread.Sleep(configuration.IntervalMilliseconds);
                }
            }
            catch (Exception)
            {
                lock (applicationFaulLock)
                {
                    applicationFault = true;
                }
                throw;
            }
        }

        private void ExecuteRequest() {

            var webRequest = (HttpWebRequest)WebRequest.CreateDefault(configuration.ServiceUri);

            foreach (var headerKey in configuration.Headers.Keys)
            {
                var headerValue = configuration.Headers[headerKey];
                webRequest.Headers.Add(headerKey, headerValue);
            }

            webRequest.ContentType = configuration.ContentType;
            webRequest.Accept = configuration.Accept;
            webRequest.Method = configuration.MethodAsString;
            webRequest.Timeout = configuration.TimeoutMilliseconds;
            webRequest.KeepAlive = configuration.KeepAlive;

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            if (configuration.Method == HttpMethod.POST || configuration.Method == HttpMethod.PUT)
            {
                // both GetRequestStream _and_ GetResponse must be aysnc, or both will be
                // called syncronously.
                webRequest.BeginGetRequestStream(getRequestStreamAsyncResponse =>
                {
                    using (var stream = webRequest.EndGetRequestStream(getRequestStreamAsyncResponse))
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(configuration.PostData);
                    }

                    webRequest.BeginGetResponse(getResponseAsyncResult =>
                    {
                        try
                        {
                            using (var response = webRequest.EndGetResponse(getResponseAsyncResult))
                            {
                                ConsumeResponse(response);

                                stopwatch.Stop();
                                lock (elapsedLock)
                                {
                                    elapsed.Add(stopwatch.ElapsedMilliseconds);
                                }
                            }
                        }
                        catch (WebException webException)
                        {
                            Interlocked.Increment(ref faulted);

                            if (!webException.Message.StartsWith("The underlying connection was closed"))
                            {
                                ConsumeResponse(webException.Response);
                            }
                        }
                        finally
                        {
                            Interlocked.Increment(ref completed);
                        }
                    }, null);

                }, null);
            }
        }

        public void ConsumeResponse(WebResponse response)
        {
            var httpResponse = response as HttpWebResponse;
            if (httpResponse == null)
            {
                return;
            }

            if (configuration.PrintResponse)
            {
                WriteResponse(httpResponse);
            }
        }

        public void WriteResponse(HttpWebResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            outputWriter.WriteLine();
            outputWriter.WriteLine("{0}", response.ResponseUri);
            outputWriter.WriteLine("Status: {0}, {1}", (int)response.StatusCode, response.StatusDescription);

            foreach (var key in response.Headers.AllKeys)
            {
                outputWriter.WriteLine("{0}: {1}", key, response.Headers[key]);
            }
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                outputWriter.WriteLine(reader.ReadToEnd());
            }
        }
    }
}