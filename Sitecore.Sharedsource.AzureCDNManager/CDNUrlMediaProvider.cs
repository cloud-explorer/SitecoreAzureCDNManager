using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace Sitecore.Sharedsource.AzureCDNManager
{
    public class CDNUrlMediaProvider : MediaProvider
    {
        public override string GetMediaUrl(MediaItem item, MediaUrlOptions options)
        {
            string mediaUrl = base.GetMediaUrl(item, options);
            if (string.IsNullOrEmpty(mediaUrl)) return mediaUrl;

            int versionNumber = item.InnerItem.Version.Number;
            string langIsoCode = item.InnerItem.Language.CultureInfo.TwoLetterISOLanguageName;
            string updatedDate = item.InnerItem.Statistics.Updated.ToString("s");
            NameValueCollection parameters = new NameValueCollection
            {
                {"v", versionNumber.ToString()},
                {"lang", langIsoCode},
                {"modified", updatedDate}
            };
            if (options.AlwaysIncludeServerUrl)
            {
                UriBuilder uriBuilder = new UriBuilder(mediaUrl);
                NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
                queryString.Add(parameters);
                uriBuilder.Query = ToQueryString(queryString);
                return uriBuilder.ToString();
            }
            StringBuilder sb = new StringBuilder(mediaUrl);
            string seperator = "?";
            if (mediaUrl.Contains(seperator)) seperator = "&";
            foreach (string key in parameters.Keys)
            {
                sb.AppendFormat("{0}{1}={2}", seperator, key, parameters[key]);
                seperator = "&";
            }
            return sb.ToString();
        }

        private string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }
    }
}
