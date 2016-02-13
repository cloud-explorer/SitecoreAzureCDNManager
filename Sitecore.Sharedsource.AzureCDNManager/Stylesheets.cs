using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Sitecore.Sharedsource.AzureCDNManager
{
    public class Stylesheets : AzureCDNAssetsManager
    {
        private List<string> _styleSheets;

        public Stylesheets()
        {
            _styleSheets = new List<string>();
        }
        private Stylesheets(List<string> styleSheets)
        {
            _styleSheets = styleSheets;
        }

        /// <summary>
        /// Add a script to the list of scripts to be rendered with the CDN url
        /// </summary>
        /// <param name="scriptPath">Path of the script to be added relatetive to the site root</param>
        /// <returns></returns>
        public Stylesheets Add(string scriptPath)
        {
            if (string.IsNullOrEmpty(scriptPath)) throw new ArgumentException("Script path need to be provided", "scriptPath");
            _styleSheets.Add(scriptPath);
            return new Stylesheets(_styleSheets);
        }

        public IHtmlString Render()
        {
            var cssBundle = new StringBuilder();
            foreach (var script in _styleSheets)
            {
                var tagBuilder = new TagBuilder("link");
                tagBuilder.Attributes["href"] = GetUrlWithModifiedDate(script); // add GUID
                tagBuilder.Attributes["rel"] = "stylesheet";
                cssBundle.AppendLine(tagBuilder.ToString());
            }
            return MvcHtmlString.Create(cssBundle.ToString());
        }
    }
}
