using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using NetCoreAudio;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace InuLiveServer.Core
{
    class GoogleTTSService 
    {
        Player player;
        string path = Path.GetTempPath() + "audio.mp3";
        //string path = "S:\\audio.mp3";

        BlockingCollection<string> BufferSpeechText;
        bool Enabled;

        public GoogleTTSService()
        {
            player = new Player();
            BufferSpeechText = new BlockingCollection<string>();
        }

        public void QueueText(string text)
        {
            BufferSpeechText.Add(text);
        }

        public void StartSpeech()
        {
            Enabled=true;

            Task.Factory.StartNew(async ()=> await SpeechService());
        }

        public void StopSpeech()
        {
            Enabled=false;
        }

        async Task SpeechService()
        {
            while (Enabled)
            {
                var text = BufferSpeechText.Take();
                await DoSpeech(text);
            }
        }

        async Task<bool> DoSpeech(string text)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string url = "http://translate.google.com/translate_tts?ie=UTF-8&tl=zh-TW&q=" + HttpUtility.UrlEncode(text) + "&client=tw-ob";
                    await webClient.DownloadFileTaskAsync(new Uri(url), path);
                    await player.Play(path);
                    File.Delete(path);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.Write("Exception in GoogleTTSEngine.DoSpeech");
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}
