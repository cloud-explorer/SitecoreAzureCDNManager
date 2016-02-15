using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore.ApplicationCenter.Applications;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Resources.Media;
using Sitecore.Web;

namespace Sitecore.Sharedsource.AzureCDNManager
{
    public class CDNCacheControlProcessor : HttpRequestProcessor
    {
        /// <summary>
        /// Sets the cache-control header for the requested item
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            Item item = Context.Item;
            if (item == null)
            {
                //If the context item is null, check if this is a request for a media item on the actual page 
                MediaRequest request = MediaManager.ParseMediaRequest(args.Context.Request);
                if (request == null) return;
                Media media = MediaManager.GetMedia(request.MediaUri);
                item = media.MediaData.MediaItem;
            }
            //Check if this item is based on the caching info inemplate
            if (!item.InheritsFrom(AzureCDNConstants.AzureCachingInfoTemplateId)) return;
            //Get the TTL chosed by the editor
            string s = item["ttl"];
            int ttl;
            //If zero or non int value was chosen, use default behaviour 
            if (!int.TryParse(s, out ttl) || ttl <= 0) return;
            //Set the response headers
            args.Context.Response.Cache.SetCacheability(HttpCacheability.Public);
            TimeSpan delta = TimeSpan.FromMinutes(ttl);
            args.Context.Response.Cache.SetMaxAge(delta);
            args.Context.Response.Cache.SetExpires(DateTime.UtcNow + delta);
            args.Context.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            args.Context.Response.Cache.SetLastModified(item.Statistics.Updated);

        }
    }
}
