using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace BL.Services.Google {

    public class MapService {
        private static readonly string key = "AIzaSyAt8mx-_cf_K7SlutrTWNwuO0g4dPjqMNY";

        /// <summary>
        /// 打google Api 算地址的地理資訊
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>Geocoding</returns>
        public static Geocoding GetGeocoding(string address) {
            string uri = "https://maps.googleapis.com/maps/api/geocode/json";
            uri += $"?address={address}";
            uri += $"&key={key}";
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.Headers.Add("Content-Type", "application/json");

            var response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream);
            var geocodingStr = streamReader.ReadToEnd();

            // 去除換行字元
            geocodingStr = geocodingStr.Replace("\n", "");
            return JsonConvert.DeserializeObject<Geocoding>(geocodingStr);
        }

        /// <summary>
        /// 打google Api 算起點和終點間的距離及抵達時間
        /// </summary>
        /// <param name="destinationAddress">起點地址</param>
        /// <param name="originAddress">終點地址</param>
        /// <returns>DistanceMatrix</returns>
        public static DistanceMatrix GetDistanceMatrix(string destinationAddress, string originAddress) {
            string uri = "https://maps.googleapis.com/maps/api/distancematrix/json";
            uri += $"?origins={destinationAddress}";
            uri += $"&destinations={originAddress}";
            uri += $"&key={key}";
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.Headers.Add("Content-Type", "application/json");

            var response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream);
            var distanceMatrixStr = streamReader.ReadToEnd();

            // 去除換行字元
            distanceMatrixStr = distanceMatrixStr.Replace("\n", "");
            return JsonConvert.DeserializeObject<DistanceMatrix>(distanceMatrixStr);
        }
    }

    public class DistanceMatrix {
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

    public class Row {
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

    public class Element {
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

    public class Duration {
        public string text { get; set; }
        public int value { get; set; }
        /*
            {
                "text" : "225 mi",
                "value" : 361715
            }
        */
    }

    public class Distance {
        public string text { get; set; }
        public int value { get; set; }
        /*
            {
                "text" : "3 hours 49 mins",
                "value" : 13725
            }
        */
    }

    public class Geocoding {
        public List<GeocodingResult> results { get; set; }
        public string status { get; set; }
        /*
            {
                "results" : [
                    {
                        "address_components" : [
                            {
                            "long_name" : "1600",
                            "short_name" : "1600",
                            "types" : [ "street_number" ]
                            },
                            {
                            "long_name" : "Amphitheatre Parkway",
                            "short_name" : "Amphitheatre Pkwy",
                            "types" : [ "route" ]
                            },
                            {
                            "long_name" : "Mountain View",
                            "short_name" : "Mountain View",
                            "types" : [ "locality", "political" ]
                            },
                            {
                            "long_name" : "Santa Clara County",
                            "short_name" : "Santa Clara County",
                            "types" : [ "administrative_area_level_2", "political" ]
                            },
                            {
                            "long_name" : "California",
                            "short_name" : "CA",
                            "types" : [ "administrative_area_level_1", "political" ]
                            },
                            {
                            "long_name" : "United States",
                            "short_name" : "US",
                            "types" : [ "country", "political" ]
                            },
                            {
                            "long_name" : "94043",
                            "short_name" : "94043",
                            "types" : [ "postal_code" ]
                            }
                        ],
                        "formatted_address" : "1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA",
                        "geometry" : {
                            "location" : {
                            "lat" : 37.4267861,
                            "lng" : -122.0806032
                            },
                            "location_type" : "ROOFTOP",
                            "viewport" : {
                            "northeast" : {
                                "lat" : 37.4281350802915,
                                "lng" : -122.0792542197085
                            },
                            "southwest" : {
                                "lat" : 37.4254371197085,
                                "lng" : -122.0819521802915
                            }
                            }
                        },
                        "place_id" : "ChIJtYuu0V25j4ARwu5e4wwRYgE",
                        "plus_code" : {
                            "compound_code" : "CWC8+R3 Mountain View, California, United States",
                            "global_code" : "849VCWC8+R3"
                        },
                        "types" : [ "street_address" ]
                    }
                ],
                "status" : "OK"
            }
        */
    }

    public class GeocodingResult {
        public List<address_component> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public Plus_code plus_code { get; set; }
        public List<string> types { get; set; }
        /*
            {
                "address_components" : [
                    {
                    "long_name" : "1600",
                    "short_name" : "1600",
                    "types" : [ "street_number" ]
                    },
                    {
                    "long_name" : "Amphitheatre Parkway",
                    "short_name" : "Amphitheatre Pkwy",
                    "types" : [ "route" ]
                    },
                    {
                    "long_name" : "Mountain View",
                    "short_name" : "Mountain View",
                    "types" : [ "locality", "political" ]
                    },
                    {
                    "long_name" : "Santa Clara County",
                    "short_name" : "Santa Clara County",
                    "types" : [ "administrative_area_level_2", "political" ]
                    },
                    {
                    "long_name" : "California",
                    "short_name" : "CA",
                    "types" : [ "administrative_area_level_1", "political" ]
                    },
                    {
                    "long_name" : "United States",
                    "short_name" : "US",
                    "types" : [ "country", "political" ]
                    },
                    {
                    "long_name" : "94043",
                    "short_name" : "94043",
                    "types" : [ "postal_code" ]
                    }
                ],
                "formatted_address" : "1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA",
                "geometry" : {
                    "location" : {
                    "lat" : 37.4267861,
                    "lng" : -122.0806032
                    },
                    "location_type" : "ROOFTOP",
                    "viewport" : {
                    "northeast" : {
                        "lat" : 37.4281350802915,
                        "lng" : -122.0792542197085
                    },
                    "southwest" : {
                        "lat" : 37.4254371197085,
                        "lng" : -122.0819521802915
                    }
                    }
                },
                "place_id" : "ChIJtYuu0V25j4ARwu5e4wwRYgE",
                "plus_code" : {
                    "compound_code" : "CWC8+R3 Mountain View, California, United States",
                    "global_code" : "849VCWC8+R3"
                },
                "types" : [ "street_address" ]
            }
        */
    }

    public class address_component {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
        /*
            {
                "long_name" : "1600",
                "short_name" : "1600",
                "types" : [ "street_number" ]
            }
        */
    }

    public class Geometry {
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
        /*
            {
                "location" : {
                "lat" : 37.4267861,
                "lng" : -122.0806032
                },
                "location_type" : "ROOFTOP",
                "viewport" : {
                    "northeast" : {
                        "lat" : 37.4281350802915,
                        "lng" : -122.0792542197085
                    },
                    "southwest" : {
                        "lat" : 37.4254371197085,
                        "lng" : -122.0819521802915
                    }
                }
            }
        */
    }

    public class Location {
        public string lat { get; set; }
        public string lng { get; set; }
        /*
            {
                "lat" : 37.4267861,
                "lng" : -122.0806032
            }
        */
    }

    public class Plus_code {
        public string compound_code { get; set; }
        public string global_code { get; set; }
        /*
            {
                "compound_code" : "CWC8+R3 Mountain View, California, United States",
                "global_code" : "849VCWC8+R3"
            }
        */
    }

    public class Viewport {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
        /*
            {
                "northeast" : {
                    "lat" : 37.4281350802915,
                    "lng" : -122.0792542197085
                },
                "southwest" : {
                    "lat" : 37.4254371197085,
                    "lng" : -122.0819521802915
                }
            }
        */
    }

    public class Northeast {
        public string lat { get; set; }
        public string lng { get; set; }
        /*
            {
                "lat" : 37.4281350802915,
                "lng" : -122.0792542197085
            }
        */
    }

    public class Southwest {
        public string lat { get; set; }
        public string lng { get; set; }
        /*
            {
                "lat" : 37.4254371197085,
                "lng" : -122.0819521802915
            }
        */
    }
}