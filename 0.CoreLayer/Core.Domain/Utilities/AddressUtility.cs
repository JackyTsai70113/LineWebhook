namespace Core.Domain.Utilities {

    public static class AddressUtility {

        public static string GetSecondDivision(string address) {
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