using System;
using System.IO;
using System.Net;
using System.Text;
using Models.Line;
using Models.Line.Webhook;
using Newtonsoft.Json;

namespace Utility.Line
{
    public class RequestHandler
    {
        public LineRequestBody requestBody { get; set;}
        public string messageType { get; set; }
        public RequestHandler(dynamic requestBody)
        {
            try
            {
                LineRequestBody body = JsonConvert.
                    DeserializeObject<LineRequestBody>(requestBody.ToString());

                Message message = JsonConvert.
                    DeserializeObject<Message>(body.events[0].message.ToString());

                // set this.type
                this.messageType = message.type;

                // set this.requestBody
                switch(message.type)
                {
                    case "text":
                        body.events[0].message = JsonConvert.
                            DeserializeObject<TextMessage>(body.events[0].message.ToString());
                        break;
                    case "location":
                        body.events[0].message = JsonConvert.
                            DeserializeObject<TextMessage>(body.events[0].message.ToString());
                        break;
                    default:
                        break;
                }
                // bool aaaaaaa = body.events[0].message is Message;
                // bool a1aaaaa = body.events[0].message is TextMessage;
                // bool a2aaaaa = body.events[0].message is LocationMessage;
                this.requestBody = body;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[RequestHandler.Handle] Ex: {ex.Message}");
            }
        }

        private Message HandleMessage(dynamic message)
        {
            return new Message();
        }
    }
}