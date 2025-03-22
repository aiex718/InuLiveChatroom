using InuLiveServer.Core;
using InuLiveServer.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace InuLiveServer
{
    public class Program
    {
        internal static readonly DateTime startTime = DateTime.Now;
        internal static readonly InuChatBot chatbot = new InuChatBot();
        //internal static WSChatServer wsChatServer = new WSChatServer();
        internal static readonly SignalRChatServer chatServer = new SignalRChatServer();

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();

            app.MapHub<ChatHub>("/chatHub");

            var hubContext = app.Services.GetService(typeof(IHubContext<ChatHub>));
            chatServer.ConnectToHub(hubContext as IHubContext<ChatHub>);
            chatbot.Attach(chatServer);

            app.Run();
        }
    }
}
