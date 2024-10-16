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

        public void CopyTo(StreamInfo other)
        {
            other.title = this.title;
            other.subtitle = this.subtitle;
            other.game = this.game;
        }

        public void ReadFromConsole()
        {
            Console.Write("Input title:");
            title = Console.ReadLine();
            Console.Write("Input subtitle:");
            subtitle = Console.ReadLine();
            Console.Write("Input game:");
            game = Console.ReadLine();
        }

        public void Print()
        {
            Console.WriteLine($"Title:{title}");
            Console.WriteLine($"SubTitle:{subtitle}");
            Console.WriteLine($"Game:{game}");
        }

        public bool IsValid()
        {
            return String.IsNullOrEmpty(title)==false &&
            String.IsNullOrEmpty(subtitle)==false &&
            String.IsNullOrEmpty(game)==false ;
        }
    }
}
