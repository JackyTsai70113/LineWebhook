using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Core.Domain.Enums {

    /// <summary>
    /// 股票代號Enum
    /// </summary>
    public enum StockCodeEnum {

        #region 通信網路業

        /// <summary>
        /// 中華電(2412)
        /// </summary>
        _2412 = 2412,

        #endregion 通信網路業

        #region 金融保險業

        /// <summary>
        /// 彰銀(2801)
        /// </summary>
        _2801 = 2801,

        /// <summary>
        /// 京城銀(2809)
        /// </summary>
        _2809 = 2809,

        /// <summary>
        /// 台中銀(2812)
        /// </summary>
        _2812 = 2812,

        /// <summary>
        /// 旺旺保(2816)
        /// </summary>
        _2816 = 2816,

        /// <summary>
        /// 華票(2820)
        /// </summary>
        _2820 = 2820,

        /// <summary>
        /// 中壽(2823)
        /// </summary>
        _2823 = 2823,

        /// <summary>
        /// 台產(2832)
        /// </summary>
        _2832 = 2832,

        /// <summary>
        /// 臺企銀(2834)
        /// </summary>
        _2834 = 2834,

        /// <summary>
        /// 高雄銀(2836)
        /// </summary>
        _2836 = 2836,

        /// <summary>
        /// 聯邦銀(2838)
        /// </summary>
        _2838 = 2838,

        ///// <summary>
        ///// 聯邦銀甲特(2838A)
        /////</summary>
        //_2838A = 2838 + 10000,

        /// <summary>
        /// 遠東銀(2845)
        /// </summary>
        _2845 = 2845,

        /// <summary>
        /// 安泰銀(2849)
        /// </summary>
        _2849 = 2849,

        /// <summary>
        /// 新產(2850)
        /// </summary>
        _2850 = 2850,

        /// <summary>
        /// 中再保(2851)
        /// </summary>
        _2851 = 2851,

        /// <summary>
        /// 第一保(2852)
        /// </summary>
        _2852 = 2852,

        /// <summary>
        /// 統一證(2855)
        /// </summary>
        _2855 = 2855,

        /// <summary>
        /// 三商壽(2867)
        /// </summary>
        _2867 = 2867,

        /// <summary>
        /// 華南金(2880)
        /// </summary>
        _2880 = 2880,

        /// <summary>
        /// 富邦金(2881)
        /// </summary>
        _2881 = 2881,

        ///// <summary>
        ///// 富邦特(2881A)
        ///// </summary>
        //_2881A = 2881 + 10000,

        ///// <summary>
        ///// 富邦金乙特(2881B)
        /////</summary>
        //_2881B = 2881 + 20000,

        /// <summary>
        /// 國泰金(2882)
        /// </summary>
        _2882 = 2882,

        ///// <summary>
        ///// 國泰特(2882A)
        ///// </summary>
        //_2882A = 2882 + 10000,

        ///// <summary>
        ///// 國泰金乙特(2882B)
        /////</summary>
        //_2882B = 2882 + 20000,

        /// <summary>
        /// 開發金(2883)
        /// </summary>
        _2883 = 2883,

        /// <summary>
        /// 玉山金(2884)
        /// </summary>
        _2884 = 2884,

        /// <summary>
        /// 元大金(2885)
        /// </summary>
        _2885 = 2885,

        /// <summary>
        /// 兆豐金(2886)
        /// </summary>
        _2886 = 2886,

        /// <summary>
        /// 台新金(2887)
        /// </summary>
        _2887 = 2887,

        ///// <summary>
        ///// 台新戊特(2887E)
        ///// </summary>
        //_2887E = 2887 + 50000,

        ///// <summary>
        ///// 台新戊特二(2887F)
        /////</summary>
        //_2887F = 2887 + 60000,

        /// <summary>
        /// 新光金(2888)
        /// </summary>
        _2888 = 2888,

        ///// <summary>
        ///// 新光金甲特(2888A)
        /////</summary>
        //_2888A = 2888 + 10000,

        /// <summary>
        /// 國票金(2889)
        /// </summary>
        _2889 = 2889,

        /// <summary>
        /// 永豐金(2890)
        /// </summary>
        _2890 = 2890,

        /// <summary>
        /// 中信金(2891)
        /// </summary>
        _2891 = 2891,

        ///// <summary>
        ///// 中信金乙特(2891B)
        /////</summary>
        //_2891B = 2891 + 20000,

        ///// <summary>
        ///// 中信金丙特(2891C)
        /////</summary>
        //_2891C = 2891 + 30000,

        /// <summary>
        /// 第一金(2892)
        /// </summary>
        _2892 = 2892,

        /// <summary>
        /// 王道銀行(2897)
        /// </summary>
        _2897 = 2897,

        ///// <summary>
        ///// 王道銀甲特(2897A)
        /////</summary>
        //_2897A = 2897 + 10000,

        /// <summary>
        /// 上海商銀(5876)
        /// </summary>
        _5876 = 5876,

        /// <summary>
        /// 合庫金(5880)
        /// </summary>
        _5880 = 5880,

        /// <summary>
        /// 群益證(6005)
        /// </summary>
        _6005 = 6005

        #endregion 金融保險業
    }

    public static class Extensions {

        public static string ToStockCode(this StockCodeEnum stockCodeEnum) {
            return stockCodeEnum.ToString().TrimStart('_');
        }

        public static string[] GetNames() {
            return Enum.GetNames(typeof(StockCodeEnum));
        }
    }
}