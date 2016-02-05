using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Sitecore.Data.Items;

namespace Sitecore.Sharedsource.AzureCDNManager
{
    public class AzureResourceManager 
    {
        // This method taken from: http://msdn.microsoft.com/en-us/library/azure/dn790557.aspx
        public string GetAuthorizationHeader(Item azureSetting)
        {
            string tenantId;
            string clientId;
            string url;
            SetAzureAuthAppInformation(azureSetting, out tenantId, out clientId, out url);
            if (string.IsNullOrEmpty(tenantId) ||
                string.IsNullOrEmpty(clientId) ||
                string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            Uri redirectUrl = new Uri(url);
            AuthenticationResult result = null;

            var context = new AuthenticationContext("https://login.windows.net/" + tenantId);

            var thread = new Thread(() =>
            {
                result = context.AcquireToken(
                  "https://management.core.windows.net/",
                  clientId,
                  redirectUrl,
                  PromptBehavior.Always);
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "AquireTokenThread";
            thread.Start();
            thread.Join();

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            string token = result.AccessToken;
            return token;
        }

        private static void SetAzureAuthAppInformation(Item azureSetting, out string tenantId, out string clientId,
            out string url)
        {
            //Check if the azure setting node is empty and it inherits from the azure settings template
            if (azureSetting == null || azureSetting.TemplateID != Constants.AzureSettingsTemplateId)
            {
                tenantId = clientId = url = string.Empty; 
                return;
            }
            tenantId = azureSetting["TenantId"];
            clientId = azureSetting["ClientId"];
            url = azureSetting["RedirectUrl"];
        }


        public string GetAuthTokenUsingUserNamePassword(Item azureSetting)
        {
            string tenantId;
            string clientId;
            string url;
            SetAzureAuthAppInformation(azureSetting, out tenantId, out clientId, out url);
            if (string.IsNullOrEmpty(tenantId) ||
                string.IsNullOrEmpty(clientId) ||
                string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            var authenticationContext = new AuthenticationContext("https://login.windows.net/" + tenantId);
            //var credential = new ClientCredential(clientId: clientId, clientSecret: clientSecret);

            var credential = new UserCredential("automated-service@unwomen.onmicrosoft.com", "Welcome!23");

            var result = authenticationContext.AcquireToken(resource: "https://management.core.windows.net/", clientId: clientId, userCredential: credential);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            string token = result.AccessToken;

            return token;
        }
    }
}
