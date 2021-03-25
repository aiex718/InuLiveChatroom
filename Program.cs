
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.IO;
using System.Net.WebSockets;
using InuLiveServer.Models;
using InuLiveServer.Core;
using Microsoft.AspNetCore.SignalR;
using InuLiveServer.Hubs;

namespace InuLiveServer
{
    public class Program
    {
        internal static StreamInfo streamInfo {get;private set;} = new StreamInfo();
        internal static int port = 10500;

        //internal static WSChatServer wsChatServer = new WSChatServer();
        internal static readonly InuChatBot chatbot = new InuChatBot();
        internal static readonly SignalRChatServer chatServer = new SignalRChatServer();

        static void Main(string[] args)
        {
            if(streamInfo.Read()==false || streamInfo.IsValid()==false)
            {
                Console.WriteLine("Stream info not found");
                streamInfo.ReadFromConsole();
                streamInfo.Save();
            }

            Console.WriteLine("Using StreamInfo");
            streamInfo.Print();

            var host = new WebHostBuilder()
                .UseKestrel(options => {options.Listen(new IPAddress(0), port);})
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            var hubContext = host.Services.GetService(typeof(IHubContext<ChatHub>));
            
            chatbot.Attach(chatServer);
            chatServer.ConnectToHub(hubContext as IHubContext<ChatHub>);

            host.Run();
        }
    }
}
