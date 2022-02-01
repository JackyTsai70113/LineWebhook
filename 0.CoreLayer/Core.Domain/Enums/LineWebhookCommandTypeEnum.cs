namespace Core.Domain.Enums {

    /// <summary>
    /// Line Webhook 指令類型 Enum
    /// </summary>
    public enum LineWebhookCommandTypeEnum {

        /// <summary>
        /// 無
        /// </summary>
        None,

        /// <summary>
        /// "": 空字串
        /// </summary>
        Empty,

        /// <summary>
        /// "cd": 劍橋詞典
        /// </summary>
        CambridgeDictionary,

        /// <summary>
        /// "sp": 永豐報價
        /// </summary>
        SinoPac,

        /// <summary>
        /// "st": Line貼圖
        /// </summary>
        Sticker,

        /// <summary>
        /// ""交易量
        /// </summary>
        TradingVolume
    }
}