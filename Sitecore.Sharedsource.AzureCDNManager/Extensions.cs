using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;

namespace Sitecore.Sharedsource.AzureCDNManager
{
    internal static class Extensions
    {
        /// <summary>
        ///     Test if an item derives directly or indirectly from a specifed template
        /// </summary>
        /// <param name="item"> </param>
        /// <param name="templateId"> </param>
        /// <returns> </returns>
        public static bool InheritsFrom(this Item item, ID templateId)
        {
            if (item == null || templateId.IsNull) return false;

            TemplateItem templateItem = item.Database.Templates[templateId];

            return InheritsFrom(item, templateItem);
        }

        public static bool InheritsFrom(this Item item, TemplateItem templateItem)
        {
            bool returnValue = false;
            if (templateItem != null)
            {
                Template template = TemplateManager.GetTemplate(item);
                returnValue = template != null &&
                              (template.ID == templateItem.ID || template.DescendsFrom(templateItem.ID));
            }

            return returnValue;
        }

        public static bool InheritsFrom(this Item item, string templateId)
        {
            ID tempID;
            if (ID.TryParse(templateId, out tempID))
            {
                return InheritsFrom(item, tempID);
            }
            return false;
        }

        public static bool InheritsFrom(this TemplateID childTemplateId, ID parentTemplateId, Database db = null)
        {
            if (db == null) db = Context.Database;

            Template template = TemplateManager.GetTemplate(childTemplateId, db);
            bool returnValue = template != null &&
                               (template.ID == parentTemplateId || template.DescendsFrom(parentTemplateId));
            return returnValue;
        }
    }
}
