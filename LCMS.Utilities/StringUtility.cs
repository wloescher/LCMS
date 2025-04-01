using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace LCMS.Utilities
{
    public static class StringUtility
    {
        private const string RegExPattern_TrackedChangesOrInlineComments = "<delete[^>]*>(.*?)</delete>|<insert[^>]*>|</insert>|<comment[^>]*>(.*?)</comment>";

        #region Public Methods

        /// <summary>
        /// Get the first N characters of a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns>string.</returns>
        public static string Left(this string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            maxLength = Math.Abs(maxLength);
            return value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength);
        }

        /// <summary>
        /// Get the last N characters of a string.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns>string.</returns>
        public static string StripHtml(string? inputString)
        {
            inputString = HttpUtility.HtmlDecode(inputString ?? string.Empty);
            inputString = inputString.Replace("<p>", string.Empty).Replace("</p>", "\r\n\r\n").Replace("<br/>", "\r\n").Replace("<br>", "\r\n");
            const string HTML_TAG_PATTERN = "(?i)<.*?>";
            var retVal = Regex.Replace(inputString.Replace("<!--", string.Empty), HTML_TAG_PATTERN, string.Empty);
            return retVal.Replace("&nbsp;", string.Empty).Replace("&rdquo;", string.Empty).Replace("&ldquo;", string.Empty).Replace("&rsquo;", string.Empty).Replace("&lsquo;", string.Empty);
        }

        /// <summary>
        /// strips out all non-alphanumerics except @ . and - sanitizes user input for malicious or unexpected chars.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>string.</returns>
        public static string SanitizeUserInput(string? input)
        {
            if (string.IsNullOrEmpty(input)) { return string.Empty; }
            try
            {
                return Regex.Replace(input, @"[^\w\.@-]", " ", RegexOptions.None, TimeSpan.FromSeconds(2));
            }
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Get sanitized HTML.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Valid HTML without Word formatting.</returns>
        public static string GetSanitizedHtml(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Decode HTML
            var html = HttpUtility.HtmlDecode(input);
            var sanitizedHtml = html;

            // Make targeted names bold
            foreach (Match match in Regex.Matches(sanitizedHtml, @"<(span).*?data-atwho-at-query.*?>(.*?)<\/\1>", RegexOptions.IgnoreCase))
            {
                sanitizedHtml = sanitizedHtml.Replace(match.ToString(), string.Format(System.Globalization.CultureInfo.InvariantCulture, "<b><a>{0}</a></b>", match.Groups[2].Value));
            }

            // Remove unwanted tags
            sanitizedHtml = Regex.Replace(sanitizedHtml, @"<[/]?(font|span|xml|del|ins|[ovwxp]:\w+)[^>]*?>", string.Empty, RegexOptions.IgnoreCase);

            // Remove unwanted attributes (requires multiple passes)
            var htmlWithoutAttributes = sanitizedHtml;
            var allAttributesRemoved = false;
            while (!allAttributesRemoved)
            {
                // TODO: Replace all aria-* and data-* attributes (currently hard coded)

                // Remove specified attributes
                htmlWithoutAttributes = Regex.Replace(htmlWithoutAttributes, @"<([^>]*)(?:class|lang|style|size|face|id|role|tabindex|aria-label|aria-expanded|aria-haspopup|jsaction|data-ved|[ovwxp]:\w+)=(?:'[^']*'|""[^""]*""|[^\s>]+)([^>]*)>", @"<$1$2>", RegexOptions.IgnoreCase);

                // Check for a change
                if (sanitizedHtml != htmlWithoutAttributes)
                {
                    sanitizedHtml = htmlWithoutAttributes;
                }
                else
                {
                    allAttributesRemoved = true;
                }
            }

            // Remove trailing spaces from HTML tags (now that class, style, etc. attributes have been removed)
            sanitizedHtml = Regex.Replace(sanitizedHtml, @"<*\s+>", @">", RegexOptions.IgnoreCase);

            // Add placeholder text to empty links
            sanitizedHtml = Regex.Replace(sanitizedHtml, @"></a>", @">Link</a>", RegexOptions.IgnoreCase);

            // Remove comments
            sanitizedHtml = Regex.Replace(sanitizedHtml, @"\<\!\-\-\[i[\w\s\p{P}\p{S}]+if\]\-\-\>", "", RegexOptions.IgnoreCase);

            // Remove fragment tags
            sanitizedHtml = sanitizedHtml.Replace("<!--EndFragment-->", string.Empty);
            sanitizedHtml = sanitizedHtml.Replace("<!--StartFragment-->", string.Empty);

            // Replace smart quotes and special characters
            sanitizedHtml = ReplaceSmartQuotesAndSpecialCharacters(sanitizedHtml);

            return sanitizedHtml;
        }

        /// <summary>
        /// Replace smart quotes.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>string.</returns>
        public static string ReplaceSmartQuotes(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var output = input.Replace("‘", "'")
                .Replace("’", "'")
                .Replace("“", "\"")
                .Replace("”", "\"")
                .Replace("&lsquo;", "'")
                .Replace("&rsquo;", "'")
                .Replace("&ldquo;", "\"")
                .Replace("&rdquo;", "\"");

            return output;
        }

        /// <summary>
        /// Replace string with superscripted Registration Marks.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>string.</returns>
        public static string ForceSuperscriptRegistrationMarks(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var output = input.Replace("<sup>&reg;</sup>", "&reg;") // Remove tag - no spaces
                .Replace("<sup> &reg;</sup>", "&reg;")              // Remove tag - leading space
                .Replace("<sup>&reg; </sup>", "&reg;")              // Remove tag - trailing space
                .Replace("&reg;", "<sup>&reg;</sup>");              // Add tag

            return output;
        }

        /// <summary>
        /// Replace smart quotes and special characters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>string.</returns>
        public static string ReplaceSmartQuotesAndSpecialCharacters(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var output = ReplaceSmartQuotes(input);

            output = output.Replace("–", "&ndash;")
                .Replace("—", "&mdash;")
                .Replace("&#233;", "&eacute;")
                .Replace("&#160;", "&nbsp;")
                .Replace("&amp;amp;", "&")
                .Replace("&amp;", "&")
                .Replace("&nbsp;", " ")
                .Replace("</p> <p>", "</p><p>")
                .Replace("\r", "<br />")
                .Replace("\n", "<br />");

            output = ReplaceSpecialCharacters(output);

            return output;
        }

        /// <summary>
        /// Replace special characters.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>string.</returns>
        public static string ReplaceSpecialCharacters(string? buffer)
        {
            if (string.IsNullOrWhiteSpace(buffer)) return string.Empty;

            if (buffer.IndexOf('\u2013') > -1) buffer = buffer.Replace('\u2013', '-');
            if (buffer.IndexOf('\u2014') > -1) buffer = buffer.Replace('\u2014', '-');
            if (buffer.IndexOf('\u2015') > -1) buffer = buffer.Replace('\u2015', '-');
            if (buffer.IndexOf('\u2017') > -1) buffer = buffer.Replace('\u2017', '_');
            if (buffer.IndexOf('\u2018') > -1) buffer = buffer.Replace('\u2018', '\'');
            if (buffer.IndexOf('\u2019') > -1) buffer = buffer.Replace('\u2019', '\'');
            if (buffer.IndexOf('\u201a') > -1) buffer = buffer.Replace('\u201a', ',');
            if (buffer.IndexOf('\u201b') > -1) buffer = buffer.Replace('\u201b', '\'');
            if (buffer.IndexOf('\u201c') > -1) buffer = buffer.Replace('\u201c', '\"');
            if (buffer.IndexOf('\u201d') > -1) buffer = buffer.Replace('\u201d', '\"');
            if (buffer.IndexOf('\u201e') > -1) buffer = buffer.Replace('\u201e', '\"');
            if (buffer.IndexOf('\u2026') > -1) buffer = buffer.Replace("\u2026", "...");
            if (buffer.IndexOf('\u2032') > -1) buffer = buffer.Replace('\u2032', '\'');
            if (buffer.IndexOf('\u2033') > -1) buffer = buffer.Replace('\u2033', '\"');
            return buffer;
        }

        /// <summary>
        /// Get the value of a JSON field.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="json"></param>
        /// <returns>string.</returns>
        public static string GetJsonValue(string field, string json)
        {
            dynamic? data = null;
            try
            {
                data = JObject.Parse(json);
            }
            catch (Newtonsoft.Json.JsonReaderException ex)
            {
                // Invalid JSON cause an error
                Console.Write(ex.Message);
            }

            var token = data == null ? null : data[field];
            if (token == null) return string.Empty;
            return data != null ? data[field].ToString() : string.Empty;
        }

        /// <summary>
        /// Get the scrubbed value of a JSON field..
        /// </summary>
        /// <param name="field"></param>
        /// <param name="json"></param>
        /// <returns>string.</returns>
        public static string GetScrubbedJsonValue(string field, string json)
        {
            var jsonValue = GetJsonValue(field, json);

            // HTML encode
            var encodedValue = HttpUtility.HtmlEncode(jsonValue);

            // Replace square brackets?
            var scrubbedValue = Regex.Replace(encodedValue, @"<[^>]*>", string.Empty);

            // Replace smart quotes and special characters
            scrubbedValue = ReplaceSmartQuotesAndSpecialCharacters(scrubbedValue);

            // Remove track changes and comments
            var regex = new Regex(RegExPattern_TrackedChangesOrInlineComments);
            scrubbedValue = HttpUtility.HtmlEncode(regex.Replace(HttpUtility.HtmlDecode(scrubbedValue), string.Empty));

            // Remove non-breaking spaces and multiple spaces
            var regex2 = new Regex("[ ]{2,}", RegexOptions.None);
            scrubbedValue = regex2.Replace(scrubbedValue.Replace("&nbsp;", " "), " ");

            return scrubbedValue;
        }

        /// <summary>
        /// Clean html text by removing "a" tags without href attributes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>string</returns>
        internal static string RemoveEmptyLinkTags(string? data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;

            var document = new HtmlDocument();
            document.LoadHtml(data);

            // Check for "a" tags
            if (document.DocumentNode.SelectNodes("//a") != null)
            {
                var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("//a"));
                while (nodes.Count > 0)
                {
                    var node = nodes.Dequeue();
                    var parentNode = node.ParentNode;

                    if (node.Name == "a")
                    {
                        if (node.Attributes["href"]?.Value == null)
                        {
                            var childNodes = node.SelectNodes("./*|./text()");

                            if (childNodes != null)
                            {
                                foreach (var child in childNodes)
                                {
                                    nodes.Enqueue(child);
                                    parentNode.InsertBefore(child, node);
                                }
                            }

                            parentNode.RemoveChild(node);
                        }
                    }
                }
            }

            return document.DocumentNode.InnerHtml;
        }

        /// <summary>
        /// Replace "safelink" URLs.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>string.</returns>
        internal static string ReplaceSafelinkUrls(string? data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;

            var document = new HtmlDocument();
            document.LoadHtml(data);

            // Check for "a" tags
            if (document.DocumentNode.SelectNodes("//a") != null)
            {
                var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("//a"));
                while (nodes.Count > 0)
                {
                    var node = nodes.Dequeue();
                    if (node.Name == "a")
                    {
                        var href = node.Attributes["href"].Value;
                        if (href.Contains("safelinks.protection.outlook.com"))
                        {
                            // Get target URL from "safelink" URL querystring
                            var querystring = href.Split('?')[1];
                            var keyValues = querystring.Split('&');
                            for (var i = 0; i < keyValues.Length; i++)
                            {
                                var keyValue = keyValues[i];
                                var key = keyValue.Split('=')[0];
                                var value = keyValue.Split('=')[1];
                                if (key.ToLower() == "url")
                                {
                                    var targetUrl = HttpUtility.UrlDecode(value);
                                    node.Attributes["href"].Value = targetUrl;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return document.DocumentNode.InnerHtml;
        }

        /// <summary>
        /// Clean text by removing white space tags and HTML encoding special characters.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="removeEmptySpanTags"></param>
        /// <returns>string.</returns>
        public static string CleanText(string? value, bool removeEmptySpanTags = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var cleanBodyText = AddNoFollowAndReplaceHtmlEncodedAmpersandsInQuerystring(value)
                .Replace("<br/>", string.Empty)
                .Replace("\r\n", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("&nbsp;", " ")
                .Replace("ū", "&umacr;")
                .Replace("ć", "&#263;");

            if (removeEmptySpanTags)
            {
                cleanBodyText = new Regex(@"<span\s*>")
                    .Replace(cleanBodyText, string.Empty)
                    .Replace("</span>", string.Empty);
            }

            // Strips code that pinterest plugin adds to all images, the plugin styles change, so just replace the 'Save' text.
            var pinterestPluginCode = "11px;\">Save</p>";
            cleanBodyText = cleanBodyText.Replace(pinterestPluginCode, "11px;\"></p>");

            cleanBodyText = RemoveEmptyLinkTags(cleanBodyText);

            cleanBodyText = ReplaceSafelinkUrls(cleanBodyText);

            return cleanBodyText;
        }

        /// <summary>
        /// Truncate a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxChars"></param>
        /// <returns>string.</returns>
        public static string? TruncateString(this string? value, int maxChars)
        {
            if (value == null)
            {
                return value;
            }
            return value.Length <= maxChars ? value : value.Substring(0, maxChars);
        }

        /// <summary>
        /// Truncate a string by word.
        /// </summary>
        /// <param name="intputString"></param>
        /// <param name="wordLimit"></param>
        /// <param name="stripHtml"></param>
        /// <returns>string.</returns>
        public static string TruncateStringByWord(string? intputString, int wordLimit, bool stripHtml = true)
        {
            if (string.IsNullOrWhiteSpace(intputString)) return string.Empty;

            // Remove double spaces and tags
            var cleanString = intputString.Replace("  ", " ");

            // Create array of words
            cleanString = stripHtml ? StripHtml(cleanString) : cleanString;
            var words = cleanString.Split(' ');

            // Check for less words than limit
            if (words.Length < wordLimit)
            {
                wordLimit = words.Length;
            }

            // Create truncated string
            var truncatedString = string.Join(" ", words, 0, wordLimit);

            // Check for more words than limit
            if (words.Length > wordLimit)
            {
                truncatedString += "... ";
            }

            return truncatedString;
        }

        /// <summary>
        /// Truncate a string by character.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="limit"></param>
        /// <returns>string.</returns>
        public static string? TruncateStringByChar(string? input, int limit)
        {
            if (input == null) { return null; }
            if (input.Length < limit)
            {
                limit = input.Length;
            }
            input = input.Remove(limit, input.Length - limit) + "...";
            return input;
        }

        /// <summary>
        /// Force first character to uppercase.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string FirstCharToUpper(string? input)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            if (string.IsNullOrEmpty(input)) throw new ArgumentException("ARGH!");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            return input.First().ToString(System.Globalization.CultureInfo.InvariantCulture).ToUpper(System.Globalization.CultureInfo.InvariantCulture) + string.Join(string.Empty, input.Skip(1));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Remove diacritics from a string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>string.</returns>
        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Looks for all anchor tags in a string, checks if they have a nofollow tag. If not, it will get added.
        /// </summary>
        /// <param name="htmlCode">The text string to search</param>
        /// <returns>string.</returns>
        private static string AddNoFollowAndReplaceHtmlEncodedAmpersandsInQuerystring(string htmlCode)
        {
            var result = htmlCode;
            if (!string.IsNullOrEmpty(htmlCode))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlCode);

                var links = doc.DocumentNode.SelectNodes("//a");
                if (links != null)
                {
                    foreach (var link in links)
                    {
                        // Add no follow
                        if (link.Attributes["rel"] == null)
                        {
                            link.SetAttributeValue("rel", "nofollow");
                        }

                        // Convert HTML encoded ampersands in querystring
                        var href = link.Attributes["href"] == null ? string.Empty : link.Attributes["href"].Value;
                        if (href.Split('?').Length > 1)
                        {
                            var url = href.Split('?')[0];
                            var querystring = href.Split('?')[1];
                            if (!string.IsNullOrEmpty(querystring) && querystring.Contains("&amp;"))
                            {
                                href = string.Format("{0}?{1}", url, querystring.Replace("&amp;", "&"));
                                link.SetAttributeValue("href", href);
                            }
                        }
                    }

                    result = doc.DocumentNode.OuterHtml;
                }
            }

            return result;
        }

        /// <summary>
        /// Replace a string case insensitive.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <returns>string.</returns>
        private static string ReplaceStringCaseInsensitive(string input, string pattern, string replacement)
        {
            var posCurrent = 0;
            var lenPattern = pattern.Length;
            var idxNext = input.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase);
            var result = new StringBuilder();
            while (idxNext >= 0)
            {
                result.Append(input, posCurrent, idxNext - posCurrent);
                result.Append(replacement);

                posCurrent = idxNext + lenPattern;

                idxNext = input.IndexOf(pattern, posCurrent, StringComparison.InvariantCultureIgnoreCase);
            }
            result.Append(input, posCurrent, input.Length - posCurrent);
            return result.ToString();
        }

        #endregion
    }
}
