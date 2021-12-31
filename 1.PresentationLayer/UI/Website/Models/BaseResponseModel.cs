namespace Website.Models {
    public class BaseResponseModel<TData, TError> {
        public bool isSuccess { get; set; }

        public TData data { get; set; }

        public TError error { get; set; }
    }
}
