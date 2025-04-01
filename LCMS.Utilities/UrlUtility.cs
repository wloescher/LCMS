using HtmlAgilityPack;
using OpenGraphNet;
using System.Globalization;
using System.Net;
using System.Web;

namespace LCMS.Utilities
{
    public static class UrlUtility
    {
        /// <summary>
        /// Check if the URL is an image.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>True/False</returns>
        public static bool IsImageUrl(string url)
        {
            var uri = new Uri(url);
            return IsImageUrl(uri);
        }

        /// <summary>
        /// Check if the URL is an image.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>True/False</returns>
        public static bool IsImageUrl(Uri uri)
        {
            if (uri == null)
            {
                return false;
            }

            try
            {
                if (!uri.AbsolutePath.StartsWith("http", System.StringComparison.InvariantCulture))
                {
                    var url = uri.AbsolutePath;
                    url = string.Format(System.Globalization.CultureInfo.InvariantCulture, "https:{0}", url);
                    uri = new Uri(url);
                }

#pragma warning disable SYSLIB0014 // Type or member is obsolete
                var req = (HttpWebRequest)WebRequest.Create(uri);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                req.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
                req.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                req.Method = "HEAD";

                using (var resp = req.GetResponse())
                {
                    return resp.ContentType.ToLower(CultureInfo.InvariantCulture).StartsWith("image/", System.StringComparison.InvariantCulture);
                }
            }
            catch (UriFormatException)
            {
                return false;
            }
            catch (WebException)
            {
                return false;
            }
        }

        /// <summary>
        /// Get OpenGraph data from URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>OpenGraphReturnModel object.</returns>
        public static OpenGraphReturnModel? GetOpenGraphFromUrl(string url)
        {
            var urlObject = new Uri(url);
            return GetOpenGraphFromUrl(urlObject);
        }

        /// <summary>
        /// Get OpenGraph data from URL.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>OpenGraphReturnModel object.</returns>
        public static OpenGraphReturnModel? GetOpenGraphFromUrl(Uri uri)
        {
            if (uri == null) { return null; }
            if (Uri.IsWellFormedUriString(uri.AbsolutePath, UriKind.Absolute))
            {
                var urlObject = uri; //new Uri(url);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var document = new HtmlWeb().Load(urlObject);

                var graph = OpenGraph.ParseHtml(document.DocumentNode.OuterHtml);
                var returnObject = new OpenGraphReturnModel();
                if (graph != null)
                {
                    returnObject.image = graph.Image ?? new Uri(string.Empty);
                    returnObject.url = graph.Url ?? new Uri(string.Empty);
                    returnObject.title = HttpUtility.HtmlDecode(graph.Title);
                    returnObject.description = HttpUtility.HtmlDecode(graph.Metadata["description"].First().Value);
                }

                if (returnObject.image == null || string.IsNullOrEmpty(returnObject.image.ToString()))
                {
                    var urls = document.DocumentNode.Descendants("img")
                                               .Select(e => e.GetAttributeValue("src", string.Empty))
                                               .Where(s => !string.IsNullOrEmpty(s));
                    var imageUrl = urls.First();
                    if (!imageUrl.Contains(urlObject.Host.Replace("www.", "")))
                    {
                        imageUrl = urlObject.Scheme + "://" + urlObject.Host + imageUrl;
                    }
                    returnObject.image = new Uri(imageUrl);
                }
                if (string.IsNullOrEmpty(returnObject.title))
                {
                    var node = document.DocumentNode.SelectSingleNode("//head/title");
                    if (node != null)
                    {
                        returnObject.title = node.InnerText;
                    }
                }
                if (returnObject.url == null)
                {
                    returnObject.url = urlObject;
                }
                return returnObject;
            }
            return null;
        }

        /// <summary>
        /// Get the URL friendly string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>string.</returns>
        public static string GetUrlFriendlyString(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return value.Trim().ToLower(System.Globalization.CultureInfo.InvariantCulture)
                .Replace("\"", string.Empty)
                .Replace("'", string.Empty)
                .Replace("[", string.Empty)
                .Replace("]", string.Empty)
                .Replace("&", "and")
                .Replace(" ", "-")
                .Replace("\\", "-")
                .Replace("/", "-")
                .Replace(",", "-")
                .Replace("+:", "-")
                .Replace(":", "-")
                .Replace(".", "-")
                .Replace("?", "-")
                .Replace("!", "-")
                .Replace("--", "-")
                .Replace("%", "");
        }
    }

    public class OpenGraphReturnModel
    {
        public Uri image { get; set; } = new Uri(string.Empty);
        public Uri url { get; set; } = new Uri(string.Empty);
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
    }
}
