using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Core.Domain.DTO.ResponseDTO.Line;
using Core.Domain.DTO.ResponseDTO.Line.Messages;
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

                    case "sticker":
                        @event.message = JsonConvert.DeserializeObject<StickerMessage>(@event.message.ToString());
                        break;

                    default:
                        break;
                }
            }
            return lineRequestBody;
        }
    }
}