using System;
using System.Collections.Generic;

namespace Models.Google.API
{
    public class Geocoding
    {
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

    public class GeocodingResult
    {
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

    public class address_component
    {
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

    public class Geometry
    {  
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

    public class Location
    {
        public string lat { get; set; }
        public string lng { get; set; }
        /*
            {
                "lat" : 37.4267861,
                "lng" : -122.0806032
            }
        */
    }

    public class Plus_code
    {
        public string compound_code { get; set; }
        public string global_code { get; set; }
        /*
            {
                "compound_code" : "CWC8+R3 Mountain View, California, United States",
                "global_code" : "849VCWC8+R3"
            }
        */
    }

    public class Viewport
    {
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

    public class Northeast
    {
        public string lat { get; set; }
        public string lng { get; set; }
        /*
            {
                "lat" : 37.4281350802915,
                "lng" : -122.0792542197085
            }
        */
    }

    public class Southwest
    {
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