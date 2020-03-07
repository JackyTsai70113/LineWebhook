using System;
using System.Collections.Generic;

namespace Models.Google.API
{
    public class DistanceMatrix
    {
        public List<string> destination_addresses { get; set; }

        public List<string> origin_addresses { get; set; }

        public List<Row> rows { get; set; }
        public string status { get; set; }

        /*
            {
                "destination_addresses" : [ "New York, NY, USA" ],
                "origin_addresses" : [ "Washington, DC, USA" ],
                "rows" : 
                [
                    {
                        "elements" : [
                            {
                            "distance" : {
                                "text" : "225 mi",
                                "value" : 361715
                            },
                            "duration" : {
                                "text" : "3 hours 49 mins",
                                "value" : 13725
                            },
                            "status" : "OK"
                            }
                        ]
                    }
                ],
                "status" : "OK"
            }
        */
    }

    public class Row
    {
        public List<Element> elements { get; set; }
        /*
            {
                "elements" : [
                    {
                    "distance" : {
                        "text" : "225 mi",
                        "value" : 361715
                    },
                    "duration" : {
                        "text" : "3 hours 49 mins",
                        "value" : 13725
                    },
                    "status" : "OK"
                    }
                ]
            }
        */
    }

    public class Element
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string status { get; set; }
        /*
            {
                "distance" : {
                    "text" : "225 mi",
                    "value" : 361715
                },
                "duration" : {
                    "text" : "3 hours 49 mins",
                    "value" : 13725
                },
                "status" : "OK"
            }
        */
    }

    public class Duration
    {
        public string text { get; set; }
        public int value { get; set; }
        /*
            {
                "text" : "225 mi",
                "value" : 361715
            }
        */
    }

    public class Distance
    {
        public string text { get; set; }
        public int value { get; set; }
        /*
            {
                "text" : "3 hours 49 mins",
                "value" : 13725
            }
        */
    }
}
