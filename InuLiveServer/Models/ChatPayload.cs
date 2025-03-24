using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InuLiveServer.Models
{
    public enum PayloadType 
    {
        Info,
        Msg,
        Cmd,
        Login
    };

    public class ChatPayload
    {
        public string nickname {get;set;}
        public string message {get;set;}
        public string color {get;set;}   
        public string type 
        {
            get=>payloadType.ToString();
            set=> payloadType = (PayloadType)Enum.Parse(typeof(PayloadType),value);
        }   

        [JsonIgnoreAttribute]
        public PayloadType payloadType {get;set;}

        public void Print()
        {
            Console.WriteLine($"Receive ChatPayload from {nickname}");
            Console.WriteLine($"Message:{message}");
            Console.WriteLine($"Type:{type},Color:{color}");
        }

        public bool IsValid()
        {
            return String.IsNullOrEmpty(nickname)==false &&
            String.IsNullOrEmpty(message)==false &&
            String.IsNullOrEmpty(color)==false ;
        }
    }
}
