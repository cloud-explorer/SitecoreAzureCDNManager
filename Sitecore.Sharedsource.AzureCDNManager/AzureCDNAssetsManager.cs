using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Sitecore.Resources.Media;
using Sitecore.Web.UI.HtmlControls;
using TagBuilder = System.Web.Mvc.TagBuilder;

namespace Sitecore.Sharedsource.AzureCDNManager
{
    public abstract class AzureCDNAssetsManager
    {
        protected virtual string GetUrlWithModifiedDate(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return fileName;
            string mapPath = HttpContext.Current.Server.MapPath(fileName);
            if (File.Exists(mapPath))
            {
                FileInfo fi = new FileInfo(mapPath);
                fileName = string.Format("{0}{1}?d={2}", Settings.SiteCDNUrl, fileName.Trim('~'), fi.LastWriteTime.ToString("s"));
            }
            return fileName;

           // MediaManager.GetMediaUrl();
        }
    }
}
