using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Web;

namespace Sitecore.Sharedsource.AzureCDNManager
{
    public class CDNCacheControlProcessor : HttpRequestProcessor
    {
        /// <summary>
        /// Runs the processor.
        /// 
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            string path = MainUtil.DecodeName(args.Url.ItemPath);
            Item item = args.GetItem(path);
            if (!item.InheritsFrom(AzureCDNConstants.AzureCDNProfileTemplateId))
            {
                return;
            }
            //args.Context.Response.AddHeader();
            int num = WebUtil.GetQueryString("sc_thumbnail") == string.Empty ? (false ? 1 : 0) : (true ? 1 : 0);
        }
    }
}
