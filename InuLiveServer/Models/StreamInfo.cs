using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InuLiveServer.Models
{
    public class StreamInfo
    {
        public string title {get;set;}
        public string subtitle {get;set;}
        public string game {get;set;}
        public bool? isLive {get;set;}
        public List<string> urls {get;set;}

        public void CopyTo(StreamInfo other)
        {
            other.title = this.title;
            other.subtitle = this.subtitle;
            other.game = this.game;
            other.isLive = this.isLive;
            other.urls = this.urls;
        }

        public void ReadFromConsole()
        {
            Console.Write("Input title:");
            title = Console.ReadLine();
            Console.Write("Input subtitle:");
            subtitle = Console.ReadLine();
            Console.Write("Input game:");
            game = Console.ReadLine();
            Console.Write("Input isLive:");
            isLive = bool.Parse(Console.ReadLine());

            Console.Write("Input urls (enter to end):");
            urls = new List<string>();
            while(true)
            {
                string line = Console.ReadLine();
                if (String.IsNullOrEmpty(line))
                    break;
                urls.Add(line);
            }
        }

        public void Print()
        {
            Console.WriteLine($"Title:{title}");
            Console.WriteLine($"SubTitle:{subtitle}");
            Console.WriteLine($"Game:{game}");
            Console.WriteLine($"IsLive:{isLive}");
            Console.WriteLine($"Urls:{String.Join(",",urls)}");
        }

        public bool IsValid()
        {
            return String.IsNullOrEmpty(title)==false &&
            String.IsNullOrEmpty(subtitle)==false &&
            String.IsNullOrEmpty(game)==false &&
            isLive!=null &&
            urls!=null && urls.Count>0 ;
        }
    }
}
