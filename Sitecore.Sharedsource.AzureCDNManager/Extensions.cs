using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.Sharedsource.AzureCDNManager
{
    internal static class Extensions
    {
        public static bool InheritsFrom(this TemplateItem template, ID templateID, bool includeSelf = false, bool recursive = true)
        {
            if (includeSelf && template.ID == templateID)
            {
                return true;
            }
            return recursive && template.BaseTemplates.Any(baseTemplate => baseTemplate.InheritsFrom(templateID, includeSelf, recursive));
        }
    }
}
