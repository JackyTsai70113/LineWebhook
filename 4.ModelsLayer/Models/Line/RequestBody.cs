using System;
using System.Collections.Generic;
using Models.Line.Webhook;

namespace Models.Line {

    public class RequestModelFromLineServer {
        public string Destination { get; set; }
        public List<Event> Events { get; set; }
    }
}