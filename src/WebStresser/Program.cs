using System;
using System.IO;
using Mono.Options;

namespace WebStresser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new HttpCallConfiguration();
            var optionSet = CreateOptions(configuration);

            try
            {
                optionSet.Parse(args);
                if (!configuration.IsValid())
                {
                    throw new OptionException("Missing required parameters", "");
                }

                var client = new RawHttpClient(configuration, Console.Out);
                client.MakeRawHttpCall();
            }
            catch (OptionException exception)
            {
                Console.WriteLine(exception.Message);
                ShowHelp(optionSet);
            }
        }

        private static void ShowHelp(OptionSet optionSet)
        {
            optionSet.WriteOptionDescriptions(Console.Out);
        }

        private static OptionSet CreateOptions(HttpCallConfiguration configuration)
        {
            var builder = new ConfigurationBuilder(configuration);

            return new OptionSet()
                .Add("?|h|help", SetShowHelp)
                .Add("u=|uri=", "REQUIRED: The URI you want to call",
                    builder.GetServiceUri)
                .Add("m=|method=", "The HTTP method. Default is GET",
                    builder.GetHttpMethod)
                .Add("q=|querystring=", "The data to be passed. Default is empty",
                    option => configuration.QueryString = option ?? string.Empty)
                .Add("i=|iterations=", "Number of iterations to run, default is 1",
                    builder.GetIterations)
                .Add("t=|interval=", "Iterval between each call in milliseconds, default is 10000",
                    builder.GetInterval)
                .Add("p=|postdata=", "Path to file containing post data",
                    builder.LoadPostData)
                .Add("r|responses", "Print responses",
                    option => configuration.PrintResponse = (option != null))
                .Add("k|keepalive", "KeepAlive header value (true or false), default is true",
                    builder.GetKeepAlive)
                .Add("a=|accept=", "Accept header value, default is 'text/xml'",
                    option => configuration.Accept = option ?? "text/xml")
                .Add("c=|contenttype=", "ContentType header value, default is 'text/xml;charset=\"utf-8\"'",
                    option => configuration.ContentType = option ?? "text/xml;charset=\"utf-8\"")
                .Add("z=|timeout=", "Timeout in milliseconds, default is 10000",
                    builder.GetTimeout)
                .Add("H:", "Add a header to the request. e.g: -H MyHeader=MyValue",
                    builder.AddHeader);
        }

        private static void SetShowHelp(string option)
        {
            if (option != null) throw new OptionException("Options", "help");
        }
    }

    public class ConfigurationBuilder
    {
        private readonly HttpCallConfiguration configuration;

        public ConfigurationBuilder(HttpCallConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void GetServiceUri(string option)
        {
            if (option == null)
            {
                throw new OptionException("uri is required", "uri");
            }
            Uri serviceUri = null;
            if(!Uri.TryCreate(option, UriKind.Absolute, out serviceUri))
            {
                throw new OptionException(string.Format("'{0}' is not a valid uri", option), "uri");
            }
            configuration.ServiceUri = serviceUri;
        }

        public void GetHttpMethod(string option)
        {
            if(option == null)
            {
                configuration.Method = HttpMethod.GET;
                return;
            }
            try
            {
                configuration.Method = (HttpMethod)Enum.Parse(typeof(HttpMethod), option);
            }
            catch (ArgumentException)
            {
                throw new OptionException(string.Format("'{0}' is not a valid method", option), "method");
            }
        }

        public void GetIterations(string option)
        {
            configuration.Iterations = ParseInt(option, "iterations");
        }

        public void GetInterval(string option)
        {
            configuration.IntervalMilliseconds = ParseInt(option, "interval");
        }

        public void GetKeepAlive(string option)
        {
            configuration.KeepAlive = ParseBool(option, "keepalive");
        }

        public void GetTimeout(string option)
        {
            configuration.TimeoutMilliseconds = ParseInt(option, "timeout");
        }

        private static int ParseInt(string value, string optionName)
        {
            var result = 0;
            if(!int.TryParse(value, out result))
            {
                throw new OptionException(string.Format("'{0}' is not a valid integer value", value), optionName);
            }
            return result;
        }

        private static bool ParseBool(string value, string optionName)
        {
            var result = false;
            if (!bool.TryParse(value, out result))
            {
                throw new OptionException(string.Format("'{0}' must be 'true' or 'false'", optionName), optionName);
            }
            return result;
        }

        public void LoadPostData(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            if (!File.Exists(path))
            {
                throw new OptionException(string.Format("POST data file '{0}' does not exist", path), "postdata");
            }
            configuration.PostData = File.ReadAllText(path);
        }

        public void AddHeader(string name, string value)
        {
            if (name == null)
            {
                throw new OptionException("Header name must have a value", "-D");
            }
            if (value == null)
            {
                throw new OptionException(string.Format("Header '{0}' has no value", name), "-D");
            }
            configuration.Headers.Add(name, value);
        }
    }
}