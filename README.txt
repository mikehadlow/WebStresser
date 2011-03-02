WebStresser
-----------

A simple tool for stress testing HTTP endpoints.

Usage:
websresser -uri=http://myserver/myendpoint

Options
  -?, -h, --help
  -u, --uri=VALUE            REQUIRED: The URI you want to call
  -m, --method=VALUE         The HTTP method. Default is GET
  -i, --iterations=VALUE     Number of iterations to run, default is 1
  -t, --interval=VALUE       Iterval between each call in milliseconds, default is 10000
  -p, --postdata=VALUE       Path to file containing post data
  -r, --responses            Print responses
  -k, --keepalive            KeepAlive header value (true or false), default is true
  -a, --accept=VALUE         Accept header value, default is 'text/xml'
  -c, --contenttype=VALUE    ContentType header value, default is 'text/xml;charset="utf-8"'
  -z, --timeout=VALUE        Timeout in milliseconds, default is 10000
  -H[=VALUE1:VALUE2]         Add a header to the request. e.g: -HMyHeader=MyValue