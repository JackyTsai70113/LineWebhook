using System;
using System.Globalization;

namespace Core.Domain.Utilities {

    public static class NumberUtility {

        public static int ThousandToInt(this string str) {
            try {
                if (string.IsNullOrEmpty(str)) {
                    return -1;
                }
                return int.Parse(str, NumberStyles.AllowThousands);
            } catch (Exception ex) {
                Console.WriteLine($"ThousandToInt 失敗, str: {str}, ex: {ex}");
                return -1;
            }
        }

        public static float ThousandToFloat(this string str) {
            try {
                if (string.IsNullOrEmpty(str)) {
                    return -1f;
                }
                return float.Parse(str, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint);
            } catch (Exception ex) {
                Console.WriteLine($"ThousandToFloat 失敗, str: {str}, ex: {ex}");
                return -1f;
            }
        }

        public static float ToFloat(this string str) {
            if (string.IsNullOrEmpty(str)) {
                return -1f;
            }

            return float.Parse(str, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint);
        }
    }
}