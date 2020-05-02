﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Core.Domain.Utility {

    public static class NumberUtility {

        public static int ThousandToInt(this string str) {
            return int.Parse(str, NumberStyles.AllowThousands);
        }

        public static float ThousandToFloat(this string str) {
            return float.Parse(str, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint);
        }
    }
}