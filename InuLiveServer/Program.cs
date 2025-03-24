using InuLiveServer.Core;
using InuLiveServer.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace InuLiveServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            //builder.Services.AddSingleton<IChatServer,WSChatServer>();
            builder.Services.AddSingleton<IChatServer,SignalRChatServer>();
            builder.Services.AddSingleton<IVideoSyncServer,SignalRSyncServer>();
            builder.Services.AddHostedService<InuChatBot>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();

            app.MapHub<LiveLinkHub>($"/{nameof(LiveLinkHub)}");

            app.Run();
        }
    }
}
