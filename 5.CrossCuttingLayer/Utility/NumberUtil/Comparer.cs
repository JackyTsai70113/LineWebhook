using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Models.Google.API;
using Utility.MaskDataHandler;

namespace Utility.NumberUtil
{
    public class Comparer
    {
        public static int CompareDistance(
            MaskDataDistances x, MaskDataDistances y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    int compare = x.distance.CompareTo(y.distance);

                    if (compare != 0)
                    {
                        return compare;
                    }
                    else
                    {
                        return x.maskDataIndex.CompareTo(y.maskDataIndex);
                    }
                }
            }
        }
    }
}