namespace nsconvojson
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class ConvoJson
    {
        [JsonProperty("intents")]
        public Intent[] Intents { get; set; }

        [JsonProperty("entities")]
        public object[] Entities { get; set; }

        [JsonProperty("input")]
        public Input Input { get; set; }

        [JsonProperty("output")]
        public Output Output { get; set; }

        [JsonProperty("context")]
        public Context Context { get; set; }

        [JsonProperty("alternate_intents")]
        public bool AlternateIntents { get; set; }
    }

    public partial class Context
    {
        [JsonProperty("conversation_id")]
        public string ConversationId { get; set; }

        [JsonProperty("system")]
        public System2 System { get; set; }
    }

    public partial class System2
    {
        [JsonProperty("dialog_stack")]
        public DialogStack[] DialogStack { get; set; }

        [JsonProperty("dialog_turn_counter")]
        public long DialogTurnCounter { get; set; }

        [JsonProperty("dialog_request_counter")]
        public long DialogRequestCounter { get; set; }

        [JsonProperty("_node_output_map")]
        public NodeOutputMap NodeOutputMap { get; set; }

        [JsonProperty("branch_exited")]
        public bool BranchExited { get; set; }

        [JsonProperty("branch_exited_reason")]
        public string BranchExitedReason { get; set; }
    }

    public partial class DialogStack
    {
        [JsonProperty("dialog_node")]
        public string DialogNode { get; set; }
    }

    public partial class NodeOutputMap
    {
        [JsonProperty("node_1_1516010958045")]
        public long[] Node1_1516010958045 { get; set; }
    }

    public partial class Input
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public partial class Intent
    {
        [JsonProperty("intent")]
        public string PurpleIntent { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }

    public partial class Output
    {
        [JsonProperty("text")]
        public string[] Text { get; set; }

        [JsonProperty("nodes_visited")]
        public string[] NodesVisited { get; set; }

        [JsonProperty("log_messages")]
        public object[] LogMessages { get; set; }
    }

    public partial class ConvoJson
    {
        public static ConvoJson FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ConvoJson>(json, Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this ConvoJson self)
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