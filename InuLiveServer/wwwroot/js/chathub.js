var chatloginarea = document.getElementById("chatloginarea");
var nicknameinput = document.getElementById("nicknameinput");
var chatjoinbtn = document.getElementById("chatjoinbtn");
var chatmessages = document.getElementById("chatmessages");
var chatinput = document.getElementById("chatinput");



const ChatEvents = {'ReceiveChat':'ReceiveChat'};

const Colors = ['coral', 'orange','khaki',
'greenyellow','mediumseagreen','lightseagreen',
'deepskyblue','mediumslateblue','mediumblue',
'hotpink','darkslategray','chocolate'];

var ConnUrl = location.protocol + '//' + location.host + "/ChatHub";
var ChatHubConn = new signalR.HubConnectionBuilder()
							.withUrl(ConnUrl)
							.withAutomaticReconnect()
							.build();

var NickName;
var Color;

window.addEventListener("resize", ChatScrollToButtom);

nicknameinput.addEventListener("keydown", function(event) 
{
	if (event.key == "Enter")
	{
		event.preventDefault();
		OnJoinClick();
	}
});


function OnJoinClick()
{
	if(nicknameinput.value)
	{
		chatloginarea.style.display="none";
		NickName=nicknameinput.value;
		
		Color = Colors[Math.floor(Math.random() * Colors.length)];
		
		playerchatinput.disabled=false;
		playerchatinput.placeholder="傳送訊息";		
		
		playerchatsendbtn.disabled=false;
		
		ChatSendPayload(CreatePayload("HandShake","Login"));	
	}
	else
	{
		nicknameinput.classList.add("wrong-input");
	}
}

chatinput.addEventListener("keydown", function(event) 
{
	if (event.key == "Enter")
	{
		event.preventDefault();
		OnChatSend();
	}
});

function ChatScrollToButtom()
{
	chatmessages.scrollTop = chatmessages.scrollHeight; 	
}


function OnChatSend()
{
	SendChat(chatinput.value);
	chatinput.value="";
}


function SendChat(text)
{
	if(NickName && text)
	{
		ChatSendPayload(CreatePayload(text));
	}
}

function CreatePayload(msg,t)
{
	var payload={
		color:Color,
		message:msg,
		nickname:NickName,
		type:null,
	};
	
	if(t)
		payload.type=t;
	else if(msg.startsWith("/"))
		payload.type="Cmd";
	else
		payload.type="Msg";

	
	return payload;
}

function PrintMessage(payload)
{
	var chatDiv = document.createElement('div');
	chatDiv.className="chat-line-message";
	
	if(payload.type.toLowerCase().includes("info"))
		chatDiv.className+=" chat-msg-info";	
	
	var senderSpan = document.createElement('span');
	senderSpan.style.color=payload.color;
	senderSpan.innerHTML = payload.nickname;
	
	var colonSpan = document.createElement('span');
	colonSpan.innerHTML = "：";
	
	var msgSpan = document.createElement('span');
	msgSpan.innerHTML = payload.message;
	
	chatDiv.appendChild(senderSpan);
	chatDiv.appendChild(colonSpan);
	chatDiv.appendChild(msgSpan);
	
	chatmessages.appendChild(chatDiv);
	ChatScrollToButtom();
	
	if(payload.type.toLowerCase().includes("msg"))
	{
		var evt = new CustomEvent(ChatEvents['ReceiveChat'], {'detail': payload});
		window.dispatchEvent(evt);
	}	
}

function PrintInfo(infotext)
{
	var chatDiv = document.createElement('div');
	chatDiv.className="chat-line-message chat-msg-info";	
	
	var msgSpan = document.createElement('span');
	msgSpan.innerHTML = infotext;
	
	chatDiv.appendChild(msgSpan);
	
	chatmessages.appendChild(chatDiv);
	ChatScrollToButtom();
}

function ChatClose() 
{
	if (ChatHubConn) 
	{
		ChatHubConn.stop();
	}
};

function ChatSendPayload(payload) 
{
	if (ChatHubConn && ChatHubConn.state==signalR.HubConnectionState.Connected) 
	{
		ChatHubConn.invoke("ClientSendChatPayload",payload)
		.catch(function (err) {
			return console.error(err.toString());
		});
	}
	else
		PrintInfo("尚未連接至聊天室");
};

async function ChatConnect() 
{
	try {
		PrintInfo("工讀生正在接線....");
        await ChatHubConn.start();
		PrintInfo("線接好了(ง๑ •̀_•́)ง");

		nicknameinput.disabled = false;
		nicknameinput.placeholder = "輸入暱稱";
		chatjoinbtn.disabled = false;

    } catch (err) {
		PrintInfo("工讀生已罷工");
        console.log(err);
    }
};

"use strict";

ChatHubConn.onclose(function(){
	PrintInfo("已離開聊天室");
});

ChatHubConn.onreconnecting(function(){
	PrintInfo("踢到線了....");
	PrintInfo("工讀生正在接線....");
});

ChatHubConn.onreconnected(function(){
	PrintInfo("線接好了(ง๑ •̀_•́)ง");
	if(NickName)
		ChatSendPayload(CreatePayload("HandShake","Login"));	
});

//called by server
ChatHubConn.on("ServerSendChatPayload", function (payload) {
	PrintMessage(payload);
});


