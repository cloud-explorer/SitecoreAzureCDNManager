#region

using System;
using System.Runtime.Caching;
using System.Threading;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Sitecore.Data.Items;

#endregion

namespace Sitecore.Sharedsource.AzureCDNManager
{
    public class AzureResourceManager
    {
        public string GetAccessTokenUsingUserCredentials(Item azureSetting)
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
            //Get an authentication context for the specified tenant
            var context = new AuthenticationContext("https://login.windows.net/" + tenantId);
            //Spwan a new thread to let the user log in and acquire the access token
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
            //Set the access token to be returned
            string token = result.AccessToken;
            return token;
        }


        public string GetAccessTokenUsingServiceAccount(Item azureSetting)
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
            //get the service account user name and password from the settings node
            string userName = azureSetting["Service Account User Name"];
            string password = azureSetting["Service Account Password"];
            //Create a user credential object using the specified service account user name and password
            var credential = new UserCredential(userName, password);
            //Issue a request to obtain the access token
            var result = authenticationContext.AcquireToken("https://management.core.windows.net/", clientId, credential);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }
            //Set the access token to be returned
            string token = result.AccessToken;
            return token;
        }

        private static void SetAzureAuthAppInformation(Item azureSetting, out string tenantId, out string clientId,
            out string url)
        {
            //Check if the azure setting node is empty and it inherits from the azure settings template
            if (azureSetting == null || azureSetting.TemplateID != AzureCDNConstants.AzureSettingsTemplateId)
            {
                tenantId = clientId = url = string.Empty;
                return;
            }
            //Get the tentant Id, client Id and redirect URL from the settings node
            tenantId = azureSetting["TenantId"];
            clientId = azureSetting["ClientId"];
            url = azureSetting["RedirectUrl"];
        }
    }
}