WebStresser
-----------

A simple tool for stress testing HTTP endpoints.

Usage:
websresser -uri=http://myserver/myendpoint

Options
  -?, -h, --help
  -u, --uri=VALUE            REQUIRED: The URI you want to call
  -m, --method=VALUE         The HTTP method. Default is GET
  -q, --querystring=VALUE    The data to be passed. Default is empty
  -i, --iterations=VALUE     Number of iterations to run, default is 1
  -t, --interval=VALUE       Iterval between each call in milliseconds, default is 10000
  -p, --postdata=VALUE       Path to file containing post data
  -r, --responses            Print responses
  -k, --keepalive            KeepAlive header value (true or false), default is true
  -a, --accept=VALUE         Accept header value, default is 'text/xml'
  -c, --contenttype=VALUE    ContentType header value, default is 'text/xml;charset="utf-8"'
  -z, --timeout=VALUE        Timeout in milliseconds, default is 10000
  -H[=VALUE1:VALUE2]         Add a header to the request. e.g: -HMyHeader=MyValue
  
Example:

C:\Source\WebStresser\src\WebStresser\bin\Debug>webstresser -u=http://mike-2008r2:8123/proxy 
-m=POST -i=100 -t=10 -p=postdata.txt 
-H=SOAPAction:http://tempuri.org/ICustomerService/GetCustomerDetails