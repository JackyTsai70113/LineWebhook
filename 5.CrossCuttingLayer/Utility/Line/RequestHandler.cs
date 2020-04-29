using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Models.Line;
using Models.Line.Webhook;
using Newtonsoft.Json;

namespace Utility.Line {

    /// <summary>
    /// 處理Request
    /// </summary>
    public class RequestHandler {
        public RequestBodyFromLineServer _requestBody { get; set; }

        /// <summary>
        /// 訊息型態
        /// </summary>
        public string _messageType { get; set; }

        /// <summary>
        /// LINE Token
        /// </summary>
        public string _replyToken { get; set; }

        public RequestHandler(dynamic requestBody) {
            try {
                RequestBodyFromLineServer lineRequestBody = JsonConvert.
                    DeserializeObject<RequestBodyFromLineServer>(requestBody.ToString());
                Event lineEvent = JsonConvert.
                    DeserializeObject<Event>(lineRequestBody.Events[0].ToString());
                Message message = lineEvent.message;
                _replyToken = lineEvent.replyToken;
                // set this.type
                _messageType = message.type;

                // set this.requestBody
                switch (message.type) {
                    case "text":
                        lineRequestBody.Events[0].message = JsonConvert.
                            DeserializeObject<TextMessage>(lineEvent.message.ToString());
                        break;

                    case "location":
                        lineRequestBody.Events[0].message = JsonConvert.
                            DeserializeObject<LocationMessage>(lineEvent.message.ToString());
                        break;

                    case "sticker":
                        lineRequestBody.Events[0].message = JsonConvert.
                            DeserializeObject<StickerMessage>(lineEvent.message.ToString());
                        break;

                    case "image":
                        lineRequestBody.Events[0].message = JsonConvert.
                            DeserializeObject<ImageMessage>(lineEvent.message.ToString());
                        break;

                    default:
                        break;
                }
                // bool aaaaaaa = body.events[0].message is Message;
                // bool a1aaaaa = body.events[0].message is TextMessage;
                // bool a2aaaaa = body.events[0].message is LocationMessage;
                _requestBody = lineRequestBody;
            } catch (Exception ex) {
                Console.WriteLine($"[RequestHandler.Handle] Ex: {ex.Message}");
            }
        }

        public static RequestBodyFromLineServer GetLineRequestBody(dynamic requestBody) {
            RequestBodyFromLineServer lineRequestBody = JsonConvert.
                    DeserializeObject<RequestBodyFromLineServer>(requestBody.ToString());
            foreach (Event @event in lineRequestBody.Events) {
                switch (@event.message.type.Value) {
                    case "text":
                        @event.message = JsonConvert.DeserializeObject<TextMessage>(@event.message.ToString());
                        break;

                    case "location":
                        @event.message = JsonConvert.DeserializeObject<LocationMessage>(@event.message.ToString());
                        break;

                    default:
                        break;
                }
            }
            return lineRequestBody;
        }
    }
}