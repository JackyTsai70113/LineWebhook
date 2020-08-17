namespace Utility.StringUtil {

    public class LocationHandler {

        public static string GetLocationFirstDivisionSuffix(string address) {
            int strStart;

            // 去除郵遞區號及台灣兩字
            if ((strStart = address.IndexOf("灣")) == -1) {
                strStart = 0;
            }

            int index = -1;
            if ((index = address.IndexOf("市")) != -1) {
                return address.Substring(strStart + 1, index - strStart);
            } else if ((index = address.IndexOf("縣")) != -1) {
                return address.Substring(strStart + 1, index - strStart);
            }
            return "";
        }

        public static string GetLocationSecondDivisionSuffix(string address) {
            int strStart;

            // 去除郵遞區號及台灣兩字
            if ((strStart = address.IndexOf("灣")) == -1) {
                strStart = 0;
            }

            int index;
            if ((index = address.IndexOf("區")) != -1) {
                return address.Substring(strStart + 1, index - strStart);
            } else if ((index = address.IndexOf("鄉")) != -1) {
                return address.Substring(strStart + 1, index - strStart);
            } else if ((index = address.IndexOf("鎮")) != -1) {
                return address.Substring(strStart + 1, index - strStart);
            } else if (address.IndexOf("縣") != -1 && (index = address.IndexOf("市")) != -1) {
                return address.Substring(strStart + 1, index - strStart);
            }
            return "";
        }
    }
}