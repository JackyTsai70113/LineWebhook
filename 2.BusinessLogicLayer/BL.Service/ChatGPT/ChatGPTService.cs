using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace BL.Service.Line
{
    public class ChatGPTService : IChatGPTService
    {
        private readonly string API_KEY;

        public ChatGPTService()
        {
        }

        public ChatGPTService(IConfiguration config)
        {
            API_KEY = config["ChatGPT:ApiKey"];
        }


        public Result CallChatGPT(string msg)
        {
            HttpClient client = new();
            string uri = "https://api.openai.com/v1/completions";

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Authorization", "Bearer " + API_KEY);

            var JsonString = @"
            {
  ""model"": ""text-davinci-003"",
  ""prompt"": ""question"",
  ""max_tokens"": 4000,
  ""temperature"": 0
}
            ".Replace("question", msg);
            var content = new StringContent(JsonString, Encoding.UTF8, "application/json");
            var response = client.PostAsync(uri, content).Result;
            var JSON = response.Content.ReadAsStringAsync().Result;
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<Result>(JSON, options);
        }
    }

    public class Choice
    {
        public string Text { get; set; }
        public int Index { get; set; }
        public object Logprobs { get; set; }
        public string Finish_reason { get; set; }
    }

    public class Result
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public int Created { get; set; }
        public string Model { get; set; }
        public List<Choice> Choices { get; set; }
        public Usage Usage { get; set; }
    }

    public class Usage
    {
        public int Prompt_tokens { get; set; }
        public int Completion_tokens { get; set; }
        public int Total_tokens { get; set; }
    }
}