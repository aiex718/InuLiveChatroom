using System.Collections.Generic;
using System.Threading.Tasks;
using InuLiveServer.Models;

namespace InuLiveServer.Core
{
    public interface IChatServer
    {

        Task ReceiveClientChatPayloadAsync(string cid,ChatPayload payload);
        Task UserConnectedAsync(string cid);
        Task UserDisconnectedAsync(string cid);
        Task SendChatPayloadToClientAsync(string cid,ChatPayload payload);
        Task SendChatPayloadToAllAsync(ChatPayload payload);


        delegate void OnReceiveChatPayloadEventHandler(object sender, string cid, ChatPayload payload);
        event OnReceiveChatPayloadEventHandler OnReceiveChatPayload;

        delegate void OnUserEventHandler(object sender, string cid);
        event OnUserEventHandler OnUserJoin;
        event OnUserEventHandler OnUserLeave;

        IEnumerable<string> ListUser();
        string GetUserName(string cid);
    } 
}