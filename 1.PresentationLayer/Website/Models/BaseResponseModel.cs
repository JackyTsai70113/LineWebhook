namespace Website.Models {
    /// <summary>
    /// 回應Model
    /// </summary>
    /// <typeparam name="TData">成功資料型態</typeparam>
    /// <typeparam name="TError">錯誤資料型態</typeparam>
    public class BaseResponseModel<TData, TError> {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 成功的回傳資料
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// 失敗的錯誤資料
        /// </summary>
        public TError Error { get; set; }
    }
}
