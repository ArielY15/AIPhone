using Microsoft.CognitiveServices.SpeechRecognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPhone
{
    class SpeechRecManager
    {
        const string key = "937eadf7077443c98a1d00f4c788c9ea";

        private MicrophoneRecognitionClient speechClient;
        private object speechClientLocker = new object();

        public event Action<string> ResponseReceived;
        public event Action<string> PartialResponseReceived;


        public SpeechRecManager()
        {
            speechClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(SpeechRecognitionMode.LongDictation, "en-US", key);
            speechClient.OnPartialResponseReceived += OnPartialResponseReceived;
            speechClient.OnResponseReceived += OnResponseReceived;
            speechClient.OnMicrophoneStatus += (s, e) => Console.WriteLine(e.Recording);
        }

        public void Start()
        {
            lock (speechClientLocker)
                speechClient.StartMicAndRecognition();
        }

        public void Stop()
        {
            lock (speechClientLocker)
                speechClient.EndMicAndRecognition();
        }

        private void OnResponseReceived(object sender, SpeechResponseEventArgs e)
        {
            Console.WriteLine(e.PhraseResponse.RecognitionStatus);
            if (e.PhraseResponse.RecognitionStatus == RecognitionStatus.InitialSilenceTimeout ||
                e.PhraseResponse.RecognitionStatus == RecognitionStatus.DictationEndSilenceTimeout)
            {
                Task.Run(() =>
                {
                    lock (speechClientLocker)
                    {
                        speechClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(SpeechRecognitionMode.LongDictation, "en-US", key);
                        speechClient.OnPartialResponseReceived += OnPartialResponseReceived;
                        speechClient.OnResponseReceived += OnResponseReceived;
                        speechClient.StartMicAndRecognition(); 
                    }
                });
            }
            else
            {
                var result = e.PhraseResponse.Results?.OrderByDescending(i => i.Confidence).Select(i => i.DisplayText).FirstOrDefault();
                if (!string.IsNullOrEmpty(result))
                {
                    ResponseReceived?.Invoke(result);
                }
            }
        }

        private void OnPartialResponseReceived(object sender, PartialSpeechResponseEventArgs e)
        {
            PartialResponseReceived?.Invoke(e.PartialResult);
        }
    }
}
