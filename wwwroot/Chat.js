var chatloginarea = document.getElementById("chatloginarea");
var nicknameinput = document.getElementById("nicknameinput");
var chatmessages = document.getElementById("chatmessages");
var chatinput = document.getElementById("chatinput");

var Colors = ['coral', 'orange','khaki',
'greenyellow','mediumseagreen','lightseagreen',
'deepskyblue','mediumslateblue','mediumblue',
'hotpink','darkslategray','chocolate'];

var NickName;
var Color;

var socket;
var ChatWsConnUrl = "ws://" + document.location.hostname +":10500/chatws" ;

window.addEventListener("resize", function() {	
	ScrollToButtom();
});

function ScrollToButtom()
{
	chatmessages.scrollTop = chatmessages.scrollHeight; 	
}


nicknameinput.addEventListener("keydown", function(event) 
{
	if (event.keyCode === 13) 
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
		
		SocketSend(CretePayload("HandShake","Login"));	
	}
	else
		nicknameinput.style.border="2px solid #ff2f2f70";
}

chatinput.addEventListener("keydown", function(event) 
{
	if (event.keyCode === 13) 
	{
		event.preventDefault();
		OnChatSend();
	}
});

function OnChatSend()
{
	SendChat(chatinput.value);
	chatinput.value="";
}


function SendChat(text)
{
	if(NickName && text)
	{
		SocketSend(CretePayload(text));
	}
}

function CretePayload(msg,t)
{
	var payload={
		color:Color,
		message:msg,
		sender:NickName,
		type:"Msg"
	};
	
	if(t)
		payload.type=t;
	
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
	senderSpan.innerHTML = payload.sender;
	
	var colonSpan = document.createElement('span');
	colonSpan.innerHTML = "：";
	
	var msgSpan = document.createElement('span');
	msgSpan.innerHTML = payload.message;
	
	chatDiv.appendChild(senderSpan);
	chatDiv.appendChild(colonSpan);
	chatDiv.appendChild(msgSpan);
	
	chatmessages.appendChild(chatDiv);
	ScrollToButtom();
	
	if(payload.type.toLowerCase().includes("msg"))
		AddBarrage(payload.message,payload.color);//barrage
	
}

function PrintInfo(infotext)
{
	var chatDiv = document.createElement('div');
	chatDiv.className="chat-line-message chat-msg-info";	
	
	var msgSpan = document.createElement('span');
	msgSpan.innerHTML = infotext;
	
	chatDiv.appendChild(msgSpan);
	
	chatmessages.appendChild(chatDiv);
	ScrollToButtom();
}

function SocketClose() 
{
	if (!socket || socket.readyState !== WebSocket.OPEN) 
	{
		PrintInfo("socket not connected");
	}
	else
		socket.close(1000, "Closing from client");
};

function SocketSend(payload) 
{
	if (!socket || socket.readyState !== WebSocket.OPEN) 
	{
		PrintInfo("socket not connected");
	}
	else
	{
		var data = JSON.stringify(payload);
		socket.send(data);
	}
};

function SocketConnect() 
{
	PrintInfo("工讀生正在接線...");
	socket = new WebSocket(ChatWsConnUrl);
	
	socket.onopen = function (event) 
	{
		PrintInfo("線接好了(ง๑ •̀_•́)ง");	
	};
	
	socket.onclose = function (event) 
	{
		PrintInfo("踢到線了....");
		
		SocketConnect();
	};
	
	socket.onerror =function (event) 
	{
		PrintInfo("工讀生已罷工");
	};
	
	socket.onmessage = function (event) 
	{
		PrintMessage(JSON.parse(event.data));
	};
};
