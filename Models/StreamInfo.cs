using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InuLiveServer.Models
{
    public class StreamInfo
    {
        const string ConfigFileName = "StreamInfo.json";

        public string title {get;set;}
        public string subtitle {get;set;}
        public string game {get;set;}   

        public bool Read()
        {
            try
            {
                if(File.Exists(ConfigFileName))
                {
                    using (StreamReader inputFile = new StreamReader(ConfigFileName))
                    {
                        var json = inputFile.ReadToEnd();
                        var info = JsonSerializer.Deserialize<StreamInfo>(json);
                        info.CopyTo(this);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("StreamInfo Save Ex:{0}",ex.Message);
            }

            return false;
        }

        public bool Save()
        {
            try
            {
                using (StreamWriter outputFile = new StreamWriter(ConfigFileName,false))
                {
                    outputFile.Write(JsonSerializer.Serialize(this));
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("StreamInfo Save Ex:{0}",ex.Message);
            }
            
            return false;
        }

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
