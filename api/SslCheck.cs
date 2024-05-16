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
    public record SslCheckResults(int days, bool hsts = false); // = false, bool hasHPKP = false, bool hasCSP = false, bool hasExpectCT = false, bool hasExpectStaple = false, bool hasExpectStapleReport = false);

    public static class SslChecker
    {
        [FunctionName("SslCheck")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"➡ SslCheck called with {req.Query["domain"]}");

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

            var ssldays = expirationDate.Subtract(DateTime.Now).Days;

            string responseMessage = string.IsNullOrEmpty(domain)
                ? "This here HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"{ssldays}";

            var jsonResponse = new SslCheckResults(ssldays);
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
