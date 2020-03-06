using System;
using System.Collections.Generic;
using Models.Line.Webhook;

namespace Models.Line
{
    public class LineRequestBody
    {
        public string destination { get; set; }
        public List<Event> events { get; set; }
    }
}
