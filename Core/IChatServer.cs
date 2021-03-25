using System.Collections.Generic;
using System.Threading.Tasks;
using InuLiveServer.Models;

namespace InuLiveServer.Core
{
    public interface IChatServer
    {
        delegate void OnReceiveMsgEventHandler(object sender, ChatPayload payload);
        event OnReceiveMsgEventHandler OnReceiveMsg;

        delegate void OnUserEventHandler(object sender, string username);
        event OnUserEventHandler OnUserJoin;
        event OnUserEventHandler OnUserLeave;
        Task SendPayloadAsync(ChatPayload payload,string username=null);
        IEnumerable<string> ListUser();
    } 
}