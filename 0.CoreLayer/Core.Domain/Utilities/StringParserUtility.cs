using System;
using System.Globalization;

namespace Core.Domain.Utilities {

    public static class StringParserUtility {

        /// <summary>
        /// 判斷是否為可轉型的字串
        /// </summary>
        /// <param name="str">字串</param>
        /// <returns>是否可轉型</returns>
        public static bool IsValidString(this string str) {
            // 空白字串
            if (string.IsNullOrWhiteSpace(str)) {
                return false;
            }
            // 無值的json回傳值
            if (str == "--") {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 嘗試轉為 short，並取得轉換結果
        /// </summary>
        /// <param name="str">字串</param>
        /// <param name="shortNumber">short</param>
        /// <returns>是否轉換成功</returns>
        public static bool TryParse(this string str, out short shortNumber) {
            bool result;
            // 設定無法正確 Parse 的值
            shortNumber = -1;

            try {
                if (!str.IsValidString()) {
                    return false;
                }

                shortNumber = short.Parse(str);
                result = true;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                result = false;
                shortNumber = -1;
            }
            return result;
        }

        /// <summary>
        /// 嘗試轉為 int，並取得轉換結果
        /// </summary>
        /// <param name="str">字串</param>
        /// <param name="intNumber">int</param>
        /// <returns>是否轉換成功</returns>
        public static bool TryParse(this string str, out int intNumber) {
            bool result;
            // 設定無法正確 Parse 的值
            intNumber = -1;

            try {
                if (!str.IsValidString()) {
                    return false;
                }

                intNumber = int.Parse(str, NumberStyles.AllowThousands);
                result = true;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                result = false;
                intNumber = -1;
            }
            return result;
        }

        /// <summary>
        /// 嘗試轉為 long，並取得轉換結果
        /// </summary>
        /// <param name="str">字串</param>
        /// <param name="longNumber">long</param>
        /// <returns>是否轉換成功</returns>
        public static bool TryParse(this string str, out long longNumber) {
            bool result;
            // 設定無法正確 Parse 的值
            longNumber = -1;

            try {
                if (!IsValidString(str)) {
                    return false;
                }

                longNumber = long.Parse(str, NumberStyles.AllowThousands);
                result = true;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                result = false;
                longNumber = -1;
            }
            return result;
        }

        /// <summary>
        /// 嘗試轉為 float，並取得轉換結果
        /// </summary>
        /// <param name="str">字串</param>
        /// <param name="floatNumber">float</param>
        /// <returns>是否轉換成功</returns>
        public static bool TryParse(this string str, out float floatNumber) {
            bool result;
            // 設定無法正確 Parse 的值
            floatNumber = -1f;

            try {
                if (!IsValidString(str)) {
                    return false;
                }

                floatNumber = float.Parse(str, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint);
                result = true;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                result = false;
                floatNumber = -1f;
            }
            return result;
        }

        ///// <summary>
        ///// 嘗試轉為 DateTime，並取得轉換結果
        ///// </summary>
        ///// <param name="str">字串</param>
        ///// <param name="dateTime">日期</param>
        ///// <returns>是否轉換成功</returns>
        /// <summary>
        /// 嘗試轉為 DateTime，並取得轉換結果
        /// </summary>
        /// <param name="str">字串</param>
        /// <param name="dateTime">日期</param>
        /// <returns>是否轉換成功</returns>
        public static bool TryParse(this string str, out DateTime dateTime) {
            bool result;
            // 設定無法正確 Parse 的值
            dateTime = DateTimeUtility.Unix_Epoch_StartTime;
            string[] formatList = {
                            "yyyy/M/d tt hh:mm:ss",
                            "yyyy/MM/dd tt hh:mm:ss",
                            "yyyy/MM/dd HH:mm:ss",
                            "yyyy/M/d HH:mm:ss",
                            "yyyy/M/d",
                            "yyyy/MM/dd",
                            "yyyyMMdd"
                        };
            try {
                if (!IsValidString(str)) {
                    return false;
                }

                dateTime = DateTime.ParseExact(str, formatList, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
                result = true;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                result = false;
                dateTime = DateTimeUtility.Unix_Epoch_StartTime;
            }
            return result;
        }
    }
}