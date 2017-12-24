namespace Earl
{
    using System;
    using System.Net;
    using System.Collections.Generic;
    using UnityEngine;

    using Newtonsoft.Json;

    public partial class EmotionData
    {
        [JsonProperty("faceRectangle")]
        public FaceRectangle FaceRectangle { get; set; }

        [JsonProperty("scores")]
        public Scores Scores { get; set; }
    }

    public partial class Scores
    {
        [JsonProperty("anger")]
        public double Anger { get; set; }

        [JsonProperty("contempt")]
        public double Contempt { get; set; }

        [JsonProperty("disgust")]
        public double Disgust { get; set; }

        [JsonProperty("fear")]
        public double Fear { get; set; }

        [JsonProperty("happiness")]
        public double Happiness { get; set; }

        [JsonProperty("neutral")]
        public double Neutral { get; set; }

        [JsonProperty("sadness")]
        public double Sadness { get; set; }

        [JsonProperty("surprise")]
        public double Surprise { get; set; }
    }

    public partial class FaceRectangle
    {
        [JsonProperty("left")]
        public long Left { get; set; }

        [JsonProperty("top")]
        public long Top { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }
    }

    public partial class EmotionData
    {
        public static EmotionData[] FromJson(string json)
        {
            return JsonConvert.DeserializeObject<EmotionData[]>(json, Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this EmotionData[] self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}