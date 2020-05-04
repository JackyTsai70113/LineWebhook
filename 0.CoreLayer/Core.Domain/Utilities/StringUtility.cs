using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Core.Domain.Utilities {

    public static class StringUtility {

        /// <summary>
        /// 去除 html string 的 Tag
        /// </summary>
        /// <param name="htmlStr">來源字串</param>
        /// <returns>字串</returns>
        public static string StripHtmlTag(this string htmlStr) {
            return Regex.Replace(htmlStr, "<.*?>", string.Empty);
        }

        public static string StripHtmlSpace(this string htmlStr) {
            return htmlStr.HtmlDecode().Trim();
        }

        /// <summary>
        /// 轉換成Html-encoding string
        /// </summary>
        /// <param name="htmlStr">來源字串</param>
        /// <returns>字串</returns>
        public static string HtmlEncode(this string htmlStr) {
            return HttpUtility.HtmlEncode(htmlStr);
        }

        public static string HtmlDecode(this string htmlStr) {
            StringWriter stringWriter = new StringWriter();
            HttpUtility.HtmlDecode(htmlStr, stringWriter);
            return stringWriter.ToString();
        }

        public static string TrimEnd(this string str, string trimStr) {
            int yearIndex = str.IndexOf(trimStr);
            return str.Substring(0, yearIndex);
        }
    }
}