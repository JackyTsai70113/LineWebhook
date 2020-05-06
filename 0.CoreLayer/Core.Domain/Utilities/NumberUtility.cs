using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Core.Domain.Utilities {

    public static class NumberUtility {

        public static int ThousandToInt(this string str) {
            try {
                if (string.IsNullOrEmpty(str)) {
                    return -1;
                }
                return int.Parse(str, NumberStyles.AllowThousands);
            } catch (Exception ex) {
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
                return -1f;
            }
        }

        public static float ToFloat(this string str) {
            if (string.IsNullOrEmpty(str)) {
                return 0.0f;
            }

            return float.Parse(str, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint);
        }
    }
}