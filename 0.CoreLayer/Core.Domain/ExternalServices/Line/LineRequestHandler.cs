using Core.Domain.DTO.ResponseDTO.Line.Messages;
using Models.Line;
using Models.Line.Webhook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.ExternalServices.Line {

    public class LineRequestHandler {

        public static RequestModelFromLineServer GetLineRequestModel(dynamic requestBody) {
            RequestModelFromLineServer lineRequestBody = JsonConvert.
                DeserializeObject<RequestModelFromLineServer>(requestBody.ToString());
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