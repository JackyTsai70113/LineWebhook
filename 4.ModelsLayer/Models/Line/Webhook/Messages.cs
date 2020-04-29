using System;
using System.Collections.Generic;

namespace Models.Line.Webhook {

    public class Message {
        public string type { get; set; }
        public string id { get; set; }
    }

    /// <summary>
    /// 文字訊息
    /// </summary>
    public class TextMessage : Message {

        public TextMessage() {
            type = "text";
        }

        public string text { get; set; }
    }

    /// <summary>
    /// 地點訊息
    /// </summary>
    public class LocationMessage : Message {

        public LocationMessage() {
            type = "location";
        }

        public string title { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

        /*
            {
                "type": "location",
                "id": "325708",
                "title": "my location",
                "address": "〒150-0002 東京都渋谷区渋谷２丁目２１−１",
                "latitude": 35.65910807942215,
                "longitude": 139.70372892916203
            }
        */
    }

    /// <summary>
    /// 貼圖訊息
    /// </summary>
    public class StickerMessage : Message {

        public StickerMessage() {
            type = "sticker";
        }

        public string stickerId { get; set; }
        public string packageId { get; set; }
        public string stickerResourceType { get; set; }
    }

    /// <summary>
    /// 圖片訊息
    /// </summary>
    public class ImageMessage : Message {

        public ImageMessage() {
            type = "image";
        }

        public string originalContentUrl { get; set; }
        public string previewImageUrl { get; set; }

        /*
            {
                "type": "image",
                "originalContentUrl": "https://example.com/original.jpg",
                "previewImageUrl": "https://example.com/preview.jpg"
            }
        */
    }

    /// <summary>
    /// 影片訊息
    /// </summary>
    public class VideoMessage : Message {

        public VideoMessage() {
            type = "video";
        }

        /// <summary>
        /// 影片長度
        /// </summary>
        public int duration { get; set; }

        public string originalContentUrl { get; set; }
        public string previewImageUrl { get; set; }

        /*
            {
                "type": "video",
                "originalContentUrl": "https://example.com/original.mp4",
                "previewImageUrl": "https://example.com/preview.jpg"
            }
        */
    }

    /// <summary>
    /// 音訊訊息
    /// </summary>
    public class AudioMessage : Message {

        public AudioMessage() {
            type = "audio";
        }

        public string originalContentUrl { get; set; }
        public int duration { get; set; }

        /*
            {
                "type": "audio",
                "originalContentUrl": "https://example.com/original.m4a",
                "duration": 60000
            }
        */
    }

    public class ImagemapMessage : Message {

        public ImagemapMessage() {
            type = "imagemap";
        }

        public string baseUrl { get; set; }
        public string altText { get; set; }
        public BaseSize baseSize { get; set; }
        public Video video { get; set; }
        public List<@Action> actions { get; set; }

        /*
            {
                "type": "imagemap",
                "baseUrl": "https://example.com/bot/images/rm001",
                "altText": "This is an imagemap",
                "baseSize": {
                    "width": 1040,
                    "height": 1040
                },
                "video": {
                    "originalContentUrl": "https://example.com/video.mp4",
                    "previewImageUrl": "https://example.com/video_preview.jpg",
                    "area": {
                        "x": 0,
                        "y": 0,
                        "width": 1040,
                        "height": 585
                    },
                    "externalLink": {
                        "linkUri": "https://example.com/see_more.html",
                        "label": "See More"
                    }
                },
                "actions": [
                    {
                        "type": "uri",
                        "linkUri": "https://example.com/",
                        "area": {
                            "x": 0,
                            "y": 586,
                            "width": 520,
                            "height": 454
                        }
                    },
                    {
                        "type": "message",
                        "text": "Hello",
                        "area": {
                            "x": 520,
                            "y": 586,
                            "width": 520,
                            "height": 454
                        }
                    }
                ]
            }
        */
    }

    public class BaseSize {
        public int width { get; set; }
        public int height { get; set; }
        /*
            {
                "width": 1040,
                "height": 1040
            }
        */
    }

    public class Video {
        public string originalContentUrl { get; set; }
        public string previewImageUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        /*
            {
                "originalContentUrl": "https://example.com/video.mp4",
                "previewImageUrl": "https://example.com/video_preview.jpg",
                "area": {
                    "x": 0,
                    "y": 0,
                    "width": 1040,
                    "height": 585
                },
                "externalLink": {
                    "linkUri": "https://example.com/see_more.html",
                    "label": "See More"
                }
            }
        */
    }

    public class Area {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        /*
            {
                "x": 0,
                "y": 0,
                "width": 1040,
                "height": 585
            }
        */
    }

    public class ExternalLink {
        public string linkUri { get; set; }
        public string label { get; set; }

        /*
            {
                "linkUri": "https://example.com/see_more.html",
                "label": "See More"
            }
        */
    }

    public class Action {
    }

    public class UriAction : Action {
        public string type { get; set; } = "uri";
        public string label { get; set; }
        public string linkUri { get; set; }
        public Area area { get; set; }

        /*
            {
                "type":"uri",
                "label":"https://example.com/",
                "linkUri":"https://example.com/",
                "area":{
                    "x":0,
                    "y":0,
                    "width":520,
                    "height":1040
                }
            }
        */
    }

    public class MessageAction : Action {
        public string message { get; set; } = "message";
        public string label { get; set; }
        public string text { get; set; }
        public Area area { get; set; }

        /*
            {
                "type":"message",
                "label":"hello",
                "text":"hello",
                "area":{
                    "x":520,
                    "y":0,
                    "width":520,
                    "height":1040
                }
            }
        */
    }
}