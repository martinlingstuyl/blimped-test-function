using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.Core;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace Company.Function
{
    public class TimerTrigger1
    {
        [FunctionName("TimerTrigger1")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            #if DEBUG
                var certificate = CertificateHelper.FindByThumbprint("<thumbprint>", StoreName.My, StoreLocation.CurrentUser);
                var credential = new ClientCertificateCredential("<tenantId>", "<appId>", certificate);
            #else
                var credential = new ManagedIdentityCredential();
            #endif

            var sharePointToken = await credential.GetTokenAsync(new TokenRequestContext(new[] { $"https://contoso.sharepoint.com/.default" }), new System.Threading.CancellationToken());

            var authManager = new PnP.Framework.AuthenticationManager();
            
            using(var clientContext = authManager.GetAccessTokenContext("https://contoso.sharepoint.com/sites/sales", sharePointToken.Token)) 
            {
                clientContext.Load(clientContext.Web, w => w.Title);
                clientContext.ExecuteQuery();

                log.LogInformation("Web title = " + clientContext.Web.Title);
            }

            var graphClient = new GraphServiceClient(credential, new string[] { "https://graph.microsoft.com/.default"});
            
            var teams = await graphClient.Users["alexw@contoso.com"].JoinedTeams.Request().GetAsync();

            foreach (var team in teams)
                log.LogInformation($"Team: {team.DisplayName}");
        }
    }
}
