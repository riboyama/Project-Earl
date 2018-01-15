namespace npconvo
{
    using UnityEngine;
    using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
    using IBM.Watson.DeveloperCloud.Utilities;
    using IBM.Watson.DeveloperCloud.Logging;
    using System.Collections;
    using FullSerializer;
    using System.Collections.Generic;
    using IBM.Watson.DeveloperCloud.Connection;
    using nsconvojson;
    using nsTTS;
    using System;

    public class convoScript
    {
        private string _username = "f1632ae1-9ef6-4871-b730-edd24db2d2ec";
        private string _password = "CueXUUft0Lif";
        private string _url = "https://gateway.watsonplatform.net/conversation/api";
        private string _workspaceId = "63244a8d-4b63-4d57-9c9c-01fd2cd1f4d7";

        private Conversation _conversation;
        private string _conversationVersionDate = "2017-05-26";

        private DateTime old = new DateTime(1970, 1, 1, 1, 1, 1);

        ttsController ttsCon;

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("ExampleConversation.OnFail()", "Error received: {0}", error.ToString());
        }

        //  Send a simple message using a string
        public void Message(string msg)
        {
            Credentials credentials = new Credentials(_username, _password, _url);
            _conversation = new Conversation(credentials);
            _conversation.VersionDate = _conversationVersionDate;
            DateTime now = DateTime.Now;
            DateTime check = new DateTime(1970, 1, 1, 1, 1, 1);
            double diffInSeconds = 0;
            if (old != check)
            {
                diffInSeconds = (now - old).TotalSeconds;
            }
             
            if (diffInSeconds > 3 || old==check) {
                old = DateTime.Now;
                if (!_conversation.Message(OnMessage, OnFail, _workspaceId, msg))
                Log.Debug("ExampleConversation.Message()", "Failed to message!");
            }
        }

        private void OnMessage(object resp, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleConversation.OnMessage()", "Conversation: Message Response: {0}", customData["json"].ToString());
            ConvoJson data = ConvoJson.FromJson(customData["json"].ToString());
            string text = data.Output.Text[0];
            ttsCon = new ttsController();
            ttsCon.Synthesize(text);
            
        }
    }
}