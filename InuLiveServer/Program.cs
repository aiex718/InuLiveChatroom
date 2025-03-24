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
        //internal static WSChatServer wsChatServer = new WSChatServer();
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IChatServer,SignalRChatServer>();
            builder.Services.AddHostedService<InuChatBot>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();

            app.MapHub<ChatHub>($"/{nameof(ChatHub)}");

            app.Run();
        }
    }
}
