namespace nsTTS
{

    using UnityEngine;
    using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
    using IBM.Watson.DeveloperCloud.Logging;
    using IBM.Watson.DeveloperCloud.Utilities;
    using System.Collections;
    using System.Collections.Generic;
    using IBM.Watson.DeveloperCloud.Connection;


    public class ttsController
    {
        private string _username = "66bbac7b-dd35-4d29-8a13-6863f083727f";
        private string _password = "4L0pZtOEXlUs";
        private string _url = "https://stream.watsonplatform.net/text-to-speech/api";

        private bool busy = false;

        TextToSpeech _textToSpeech;
        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("ExampleTextToSpeech.OnFail()", "Error received: {0}", error.ToString());
        }

        //  Send a simple message using a string
        public void Synthesize(string textsent)
        {
            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            _textToSpeech = new TextToSpeech(credentials);
            _textToSpeech.Voice = VoiceType.en_US_Allison;
            if (!_textToSpeech.ToSpeech(OnSynthesize, OnFail, textsent, true))
                Log.Debug("ExampleTextToSpeech.ToSpeech()", "Failed to synthesize!");
        }

        private void OnSynthesize(AudioClip clip, Dictionary<string, object> customData)
        {
            PlayClip(clip);
        }

        private void PlayClip(AudioClip clip)
        {
            if (Application.isPlaying && clip != null && busy == false)
            {
                busy = true;
                GameObject audioObject = new GameObject("AudioObject");
                AudioSource source = audioObject.AddComponent<AudioSource>();
                source.spatialBlend = 0.0f;
                source.loop = false;
                source.clip = clip;
                source.Play();

                Object.Destroy(audioObject, clip.length);
                busy = false;
            }
        }
    }
}