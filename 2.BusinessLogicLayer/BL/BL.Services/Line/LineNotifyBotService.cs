using System;
using System.Net.Http;
using Serilog;

namespace BL.Services.Line {

    public class LineNotifyBotService {
        private readonly string _bearerToken_Jacky;

        //private readonly string _bearerToken_Jessi;
        private readonly string _bearerToken_Group;

        private readonly string _notifyUri = "https://notify-api.line.me/api/notify";

        public LineNotifyBotService() {
            _bearerToken_Group = ConfigService.Line_NotifyBearerToken_Group;
            _bearerToken_Jacky = ConfigService.Line_NotifyBearerToken_Jacky;
        }

        public bool PushMessage_Group(string text) {
            return PushMessage(_bearerToken_Group, text);
        }

        public bool PushMessage_Jacky(string text) {
            return PushMessage(_bearerToken_Jacky, text);
        }

        private bool PushMessage(string bearerToken, string text) {
            try {
                string result;
                string uri = _notifyUri + $"?message={text}";
                using (var httpClient = new HttpClient()) {
                    httpClient.BaseAddress = new Uri(uri);
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
                    result = httpClient.PostAsync(uri, new StringContent("")).Result.ToString();
                }

                Log.Information(result);
                return true;
            } catch (Exception ex) {
                Log.Information(ex.ToString());
                return false;
            }
        }
    }
}