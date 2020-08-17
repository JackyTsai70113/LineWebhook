using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Core.Domain.Utilities {

    public static class StringUtility {

        /// <summary>
        /// 去除 html string 的 Tag
        /// </summary>
        /// <param name="htmlStr">來源字串</param>
        /// <returns>字串</returns>
        public static string StripHtmlTag(string htmlStr) {
            return Regex.Replace(htmlStr, "<.*?>", string.Empty);
        }

        /// <summary>
        /// 以html解碼後，去除頭尾的空白後傳回字串。
        /// </summary>
        /// <param name="htmlStr">字串</param>
        /// <returns>字串</returns>
        public static string StripHtmlSpace(this string htmlStr) {
            string htmlDecodeStr = htmlStr.HtmlDecode();
            return htmlDecodeStr.Trim();
        }

        /// <summary>
        /// 轉換成Html-encoding string
        /// </summary>
        /// <param name="htmlStr">來源字串</param>
        /// <returns>字串</returns>
        /// <remarks>例如: (&nbsp;) 轉為 ( ) 的不換行空格字符</remarks>
        public static string HtmlEncode(this string htmlStr) {
            return HttpUtility.HtmlEncode(htmlStr);
        }

        /// <summary>
        /// 轉換成Html-encoding string
        /// </summary>
        /// <param name="htmlStr">來源字串</param>
        /// <returns>字串</returns>
        /// <remarks>例如: (&nbsp;) 轉為 ( ) 的不換行空格字符</remarks>
        public static string HtmlDecode(this string htmlStr) {
            StringWriter stringWriter = new StringWriter();
            HttpUtility.HtmlDecode(htmlStr, stringWriter);
            return stringWriter.ToString();
        }

        public static string TrimEnd(this string str, string trimStr) {
            int yearIndex = str.IndexOf(trimStr);
            return str.Substring(0, yearIndex);
        }

        public static string Serialize(this Object obj) {
            return JsonConvert.SerializeObject(obj);
        }
    }
}