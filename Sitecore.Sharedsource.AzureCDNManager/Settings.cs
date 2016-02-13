using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Diagnostics;
using Sitecore.Sites;

namespace Sitecore.Sharedsource.AzureCDNManager
{
    internal static class Settings
    {
        internal static string SiteCDNUrl
        {
            get
            {
                const string key = "siteCDNUrl";
                return GetSitePropertyForKey(key);
            }
        }

        private static string GetSitePropertyForKey(string key)
        {
            SiteContext siteContext = Context.Site;
            try
            {
                return siteContext.Properties[key];
            }
            catch (Exception ex)
            {
                //sadly eaten!
                Log.Error("could not find requested info for key " + key, ex, siteContext);
                return string.Empty;
            }
        }
    }
}
