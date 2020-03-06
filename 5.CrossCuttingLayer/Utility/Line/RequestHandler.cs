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
        public static LineRequestBody HandleBody(dynamic requestBody)
        {
            var result = new LineRequestBody();
            try
            {
                LineRequestBody body = JsonConvert.
                    DeserializeObject<LineRequestBody>(requestBody.ToString());
                var message = body.events[0].message;
                result = body;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[RequestHandler.Handle] Ex: {ex.Message}");
            }

            return result;
        }

        private Message HandleMessage(dynamic message)
        {
            return new Message();
        }
    }
}