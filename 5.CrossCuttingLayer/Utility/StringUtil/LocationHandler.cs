namespace Utility.StringUtil {

    public class LocationHandler {

        public static string GetLocationFirstDivision(string address) {
            // 去除郵遞區號及台灣兩字
            int indexOfTaiwan = address.IndexOf("台灣");
            if (indexOfTaiwan != -1) {
                address = address.Substring(indexOfTaiwan + 2);
            }

            int indexOfFirstDivision = -1;
            if (address.Contains("市")) {
                indexOfFirstDivision = address.IndexOf("市");
            } else if (address.Contains("縣")) {
                indexOfFirstDivision = address.IndexOf("縣");
            }

            return address.Substring(0, indexOfFirstDivision + 1);
        }

        public static string GetLocationSecondDivision(string address) {
            // 去除郵遞區號及台灣兩字
            int indexOfTaiwan = address.IndexOf("台灣");
            if (indexOfTaiwan != -1) {
                address = address.Substring(indexOfTaiwan + 2);
            }

            int indexOfSecondDivision = -1;
            if (address.Contains("區")) {
                indexOfSecondDivision = address.IndexOf("區");
            } else if (address.Contains("鄉")) {
                indexOfSecondDivision = address.IndexOf("鄉");
            } else if (address.Contains("鎮")) {
                indexOfSecondDivision = address.IndexOf("鎮");
            } else if (address.IndexOf("縣") != -1 && address.Contains("市")) {
                indexOfSecondDivision = address.IndexOf("市");
            }

            return address.Substring(0, indexOfSecondDivision + 1);
        }
    }
}