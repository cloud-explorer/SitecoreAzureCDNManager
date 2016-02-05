
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.MobileControls;
using Sitecore.Caching;
using Sitecore.Common;
using Sitecore.Data;
using Sitecore.Data.Engines;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Jobs;
using Sitecore.Links;
using Sitecore.Publishing;
using Sitecore.Publishing.Diagnostics;
using Sitecore.Publishing.Pipelines.Publish;

namespace Sitecore.Sharedsource.AzureCDNManager
{
    public class PurgeAzureCDNProcessor : PublishProcessor
    {
        private readonly string LastUpdate = "AzureCDN_LastPurgeTime";
        private List<ID> cacheQueue = new List<ID>();
        private List<string> urls = new List<string>();
        public override void Process(PublishContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            using (new Timer("[Publishing] - Clearing Azure CDN"))
            {
                try
                {
                    ProcessPublishedItems(context);
                }
                catch (Exception ex)
                {
                    PublishingLog.Error("An error during Publish Pipeline Process Queue execution.", ex);
                    throw;
                }
            }
            //UpdateJobStatus(context);
        }

        //based on an old blog by Alex Shyba
        //http://sitecoreblog.alexshyba.com/get_all_published_items_in_sitecore_6_1_6_2/
        protected virtual void ProcessPublishedItems(PublishContext context)
        {
            ProcessHistoryStorage(context.PublishOptions.TargetDatabase);
            foreach (ID id in cacheQueue)
            {
                Item publishedItem = context.PublishOptions.TargetDatabase.GetItem(id);
                //only work with items that inherit from the AzureCachingInfoTemplate
                if (!publishedItem.Template.InheritsFrom(AzureCDNConstants.AzureCachingInfoTemplateId))
                {
                    continue;
                }
                if (publishedItem.Paths.IsMediaItem)
                {
                    string mediaUrl = StringUtil.EnsurePrefix('/', Sitecore.Resources.Media.MediaManager.GetMediaUrl(publishedItem));
                    urls.Add(mediaUrl);
                }
                else
                {
                    string url = StringUtil.EnsurePrefix('/', LinkManager.GetItemUrl(publishedItem));
                    urls.Add(url);
                }

            }
            Log.Info("*** Total processed: " + cacheQueue.Count, this);

        }

        private void ProcessHistoryStorage(Database database)

        {
            cacheQueue.Clear();
            var utcNow = DateTime.UtcNow;
            // accessing the date of last operation
            var from = LastUpdateTime(database);
            // get the history collection for the specified dates:
            var entrys = HistoryManager.GetHistory(database, from, utcNow);
            if (entrys.Count > 0)
            {
                foreach (var entry in entrys)
                {
                    // if the entry is not added yet and it is related to an item
                    if (!cacheQueue.Contains(entry.ItemId) && entry.Category == HistoryCategory.Item)
                    {
                        cacheQueue.Add(entry.ItemId);
                        database.Properties[LastUpdate] = DateUtil.ToIsoDate(entry.Created, true);
                    }
                }
            }
            // writing back the date flag of our last operation
            database.Properties[LastUpdate] = DateUtil.ToIsoDate(utcNow, true);
        }

        protected DateTime LastUpdateTime(Database database)
        {
            var lastUpdate = database.Properties[LastUpdate];
            if (lastUpdate.Length > 0)
            {
                return DateUtil.ParseDateTime(lastUpdate, DateTime.MinValue);
            }
            return DateTime.MinValue;
        }
    }
}
