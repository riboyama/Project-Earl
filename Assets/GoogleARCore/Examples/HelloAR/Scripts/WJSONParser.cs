namespace EarlWatson
{
    using System;
    using System.Net;
    using System.Collections.Generic;
    using UnityEngine;

    using Newtonsoft.Json;

    public partial class WjsonToClass
    {
        [JsonProperty("images")]
        public Image[] Images { get; set; }

        [JsonProperty("images_processed")]
        public long ImagesProcessed { get; set; }

        [JsonProperty("custom_classes")]
        public long CustomClasses { get; set; }
    }

    public partial class Image
    {
        [JsonProperty("classifiers")]
        public Classifier[] Classifiers { get; set; }

        [JsonProperty("image")]
        public string PurpleImage { get; set; }
    }

    public partial class Classifier
    {
        [JsonProperty("classifier_id")]
        public string ClassifierId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("classes")]
        public Class[] Classes { get; set; }
    }

    public partial class Class
    {
        [JsonProperty("class")]
        public string PurpleClass { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("type_hierarchy")]
        public string TypeHierarchy { get; set; }
    }

    public partial class WjsonToClass
    {
        public static WjsonToClass FromJson(string json)
        {
            return JsonConvert.DeserializeObject<WjsonToClass>(json, Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this WjsonToClass self)
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