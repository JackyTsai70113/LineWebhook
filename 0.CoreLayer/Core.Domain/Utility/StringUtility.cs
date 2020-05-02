using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Domain.Utility {

    public static class StringUtility {

        /// <summary>
        /// 去除 html string 的 Tag
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <returns></returns>
        public static string StripHTML(this string htmlStr) {
            return Regex.Replace(htmlStr, "<.*?>", string.Empty);
        }
    }
}