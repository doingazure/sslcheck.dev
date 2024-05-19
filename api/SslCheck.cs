using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
// using Newtonsoft.Json;
using System.Text.Json;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace DoingAzure
{
    // record SslCheckResults with fields int days, bool hasHTST, bool hasHPKP, bool hasCSP, bool hasExpectCT, bool hasExpectStaple, bool hasExpectStapleReport
    // and a constructor that sets the fields
    public record SslCheckResults(string domain, int ssldays, int seconds, string machine,
            bool hsts = false); // = false, bool hasHPKP = false, bool hasCSP = false, bool hasExpectCT = false, bool hasExpectStaple = false, bool hasExpectStapleReport = false);

    public static class SslChecker
    {
        private static readonly string[] _hstsHeaders = new string[] { "Strict-Transport-Security" };
        private static readonly string[] _hpkpHeaders = new string[] { "Public-Key-Pins", "Public-Key-Pins-Report-Only" };  
        private static readonly string[] _cspHeaders = new string[] { "Content-Security-Policy" };
        private static readonly string[] _expectCTHeaders = new string[] { "Expect-CT" };
        private static readonly string[] _expectStapleHeaders = new string[] { "Expect-Staple" };
        private static readonly string[] _expectStapleReportHeaders = new string[] { "Expect-Staple-Report" };

        private static bool HasHeader(string header, string[] headers)
        {
            foreach (var h in headers)
            {
                if (header.StartsWith(h, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }


        // get machine name
        private static string GetMachineName()
        {
            return Environment.MachineName;
        }


        [FunctionName("SslCheck")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"➡ SslCheck called with {req.Query["domain"]}");
            log.LogWarning($"➡ SslCheck called with {req.Query["domain"]} - not really a Warning");

            string domain = req.Query["domain"];

#if false
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            domain = name ?? data?.name;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonSerializer.Deserialize<dynamic>(requestBody);
            domain = name ?? data;
// #else
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonSerializer.Deserialize<dynamic>(requestBody);
            // string data = JsonSerializer.Deserialize<string>(requestBody);
            domain = domain ?? data?.domain;
#endif

#if false
            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {domain}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
#else
            // string url = domain;
            DateTime expirationDate = await GetSslCertificateExpirationAsync(domain);
            Console.WriteLine($"➡ SSL for {domain} expires on {expirationDate}");
            Console.WriteLine($"<{(expirationDate - DateTime.Now).TotalDays} days left>");

            var ssldays = expirationDate.Subtract(DateTime.Now).TotalDays;
            var sslseconds = expirationDate.Subtract(DateTime.Now).TotalSeconds;

            string responseMessage = string.IsNullOrEmpty(domain)
                ? "This here HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"{ssldays}";

            var jsonResponse = new SslCheckResults(domain, ssldays, sslseconds, GetMachineName());
            return new OkObjectResult(jsonResponse);
#endif
        }

        static async Task<DateTime> GetSslCertificateExpirationAsync(string url, int port = 443)
        {
            using TcpClient client = new TcpClient(url, port);
            using SslStream sslStream = new SslStream(client.GetStream(), false,
               new RemoteCertificateValidationCallback((sender, certificate, chain, errors) => { return true; }), null);
            await sslStream.AuthenticateAsClientAsync(url);
            return ((X509Certificate2)sslStream.RemoteCertificate).NotAfter;
        }
    }
}
