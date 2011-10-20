using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace TestTimerService
{
    public class ConsoleServiceHost
    {
        public static void Start<TSerivce, TImplementation>(Uri baseAddress)
        {
            using (var host = new ServiceHost(typeof(TImplementation), baseAddress))
            {
                // Enable metadata publishing.
                var smb = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    MetadataExporter = { PolicyVersion = PolicyVersion.Policy15 }
                };
                host.Description.Behaviors.Add(smb);
                var throttling = new ServiceThrottlingBehavior
                {
                    MaxConcurrentCalls = 10000,
                    MaxConcurrentInstances = 10000,
                    MaxConcurrentSessions = 10000
                };
                host.Description.Behaviors.Add(throttling);

                host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;

                host.AddServiceEndpoint(typeof(TSerivce), new BasicHttpBinding(), baseAddress);

                // Open the ServiceHost to start listening for messages. Since
                // no endpoints are explicitly configured, the runtime will create
                // one endpoint per base address for each service contract implemented
                // by the service.
                host.Open();

                Console.WriteLine("The service is ready at {0}", baseAddress);
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                // Close the ServiceHost.
                host.Close();
            }
        }
    }
}