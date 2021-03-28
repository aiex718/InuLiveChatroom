

var playercontainer = document.getElementById("playercontainer");
var playerchatarea = document.getElementById("playerchatarea");
var titlebar = document.getElementById("titlebar");
var chatarea = document.getElementById("chatarea");


var unmutenotify = document.getElementById("unmutenotify");
var canvasBarrage = document.getElementById("canvasBarrage");
var videoBarrage = document.getElementById("videoBarrage");
var controlpanel = document.getElementById("controlpanel");

var volumebtn = document.getElementById("volumebtn");
var volumebar = document.getElementById("volumebar");
var playerchatinput = document.getElementById("playerchatinput");
var playerchatsendbtn = document.getElementById("playerchatsendbtn");


var fullbtn = document.getElementById("fullbtn");
var playbtn = document.getElementById("playbtn");
var barragebtn = document.getElementById("barragebtn");

var firstTimeAutoplay=false;

var databarrage=[{value: ' ',time: 1, speed: 0}];

var demoBarrage = new CanvasBarrage(canvasBarrage, videoBarrage, {
	data: databarrage
});

$(document).ready(function() {
	SetControlVisible(true);
	ResizeChatInput();
});

unmutenotify.addEventListener("click", function( event ) {
	unmutenotify.style.display='none';
	SetVolume(100);
	RefreshPlayerIcon();
}, false);

controlpanel.addEventListener("mouseenter", function( event ) {
	SetControlVisible(true);
}, false);

controlpanel.addEventListener("mouseleave", function( event ) {  
if (!videoBarrage.paused)
	SetControlVisible(false);
}, false);

var PlayerHoverTimer,PlayerHoverDelay=2000;
videoBarrage.addEventListener("mouseenter", function( event ) {
	SetControlVisible(true);
	PlayerHoverTimer = setTimeout(function() {
		SetControlVisible(false);
	}, PlayerHoverDelay);
}, false);

videoBarrage.addEventListener("mouseleave", function( event ) {  
clearTimeout(PlayerHoverTimer);
if (!videoBarrage.paused)
	SetControlVisible(false);
}, false);

videoBarrage.addEventListener("pause", function( event ) {
	SetControlVisible(true);
}, false);

videoBarrage.addEventListener("play", function( event ) {
	SetControlVisible(false);
}, false);

videoBarrage.addEventListener("dblclick", OnFullClick, false);
videoBarrage.addEventListener("click", OnPlayClick, false);

window.addEventListener("resize", function() {	
	ResizeChatInput();
	resetCanvasSize();
});

function StartPlayer()
{
	var httpflvUrl = 'http://'+ document.location.hostname +':10501/live?port=1935&app=live&stream=inulive';
	var hlsUrl = 'http://'+ document.location.hostname +':10501/inulive.m3u8';
	
	
	if (flvjs.isSupported()) 
	{
		//PrintInfo("Player:flvjs");
		var AutoPlayPromise;
		
		var flvPlayer = flvjs.createPlayer({
			type: 'flv',
			url: httpflvUrl
		});
		flvPlayer.attachMediaElement(videoBarrage);
		flvPlayer.load();
		flvPlayer.play().catch(error => {
			SetVolume(0);
			flvPlayer.play().then(_ => {
				unmutenotify.style.display='flex';
			});
		});
	}
	
	
	
	// if(!PlayerFound) 
	// {
		// videoBarrage.classList.add("video-js");
		// var player = videojs('videoBarrage');
		
		// if(player.canPlayType('application/x-mpegURL'))
		// {
			// PrintInfo("Player:video-js");
			// PlayerFound=true;
			
			// player.play();
		// }
		// else
		// {
			// videoBarrage.classList.remove("video-js");
		// }
	// }
	
	else if(Hls.isSupported()) 
	{		
		var hls = new Hls();
		hls.loadSource(hlsUrl);
		hls.attachMedia(videoBarrage);		
		PrintInfo("工讀生說 你的瀏覽器延遲很高，換一個好嗎∠( ᐛ 」∠)＿");
	}	
	else//Hls Native
	{
		var source = document.createElement('source');
		source.setAttribute('src', hlsUrl);
		videoBarrage.appendChild(source);
		PrintInfo("工讀生說 你的瀏覽器延遲很高，換一個好嗎∠( ᐛ 」∠)＿");
	}
	
	RefreshPlayerIcon();
}

function AddBarrage(Text,Color)
{
	if(!videoBarrage.paused)
	{
		demoBarrage.add(
		{
			value: Text,
			//color: Color,
			time: videoBarrage.currentTime
		});
	}
}

function ResizeChatInput()
{
	var w = parseInt(videoBarrage.offsetWidth);

	if(w<500)
	{
		playerchatarea.style.display="none";
	}
	else
	{
		playerchatarea.style.display="flex";
	}
}

function getreqfullscreen (root) {
	var root = document.documentElement;
	return root.requestFullscreen || root.webkitRequestFullscreen || root.mozRequestFullScreen || root.msRequestFullscreen;
}
var fullscreen = getreqfullscreen();


function IsFullScreen()
{
	return document.fullscreen || document.mozFullScreen||document.webkitIsFullScreen;
}

function OnFullClick() {
	videoBarrage.removeAttribute('playsinline');//for iphone
	if(IsFullScreen())
		document.exitFullscreen();	
	else
	{
		if(videoBarrage.paused)
			OnPlayClick();
		fullscreen.call(playercontainer);
	}
	
	setTimeout(function(){ 
		RefreshPlayerIcon();
	},100);
}

function OnViewSizeClick()
{
	if(document.fullscreen || document.mozFullScreen||document.webkitIsFullScreen)
		OnFullClick();//leave full screen

	$("#titlebar").toggle();
	$("#chatarea").toggle();
	
	RefreshPlayerIcon();
	window.dispatchEvent(new Event('resize'));
}

function OnPlayClick() {
	if (videoBarrage.paused)
		videoBarrage.play();
	else
		videoBarrage.pause();
	
	RefreshPlayerIcon();
}

function OnBarrageClick()
{
	$("#canvasBarrage").toggle();
	RefreshPlayerIcon();
}


var PresetVolume = videoBarrage.volume*100;
function OnVolumeClick()
{
	if(videoBarrage.volume<=0)
	{
		SetVolume(PresetVolume);
	}
	else
	{
		PresetVolume = videoBarrage.volume*100;
		SetVolume(0);
	}
}

function resetCanvasSize()
{
	setTimeout(function() {
		canvasBarrage.height=videoBarrage.offsetHeight;
		canvasBarrage.width=videoBarrage.offsetWidth;
	}, 10);
}

function SetVolume(val)
{
	videoBarrage.volume = parseInt(val) / 100;
	RefreshPlayerIcon();
}

function SetControlVisible(val)
{
	if(val)
		$( "#controlpanel" ).stop().fadeTo('medium',1);
	else
		$( "#controlpanel" ).stop().fadeTo('medium',0);
}

playerchatinput.addEventListener("keydown", function(event) 
{
	if (event.keyCode === 13) //enter
	{
		event.preventDefault();
		OnPlayerChatSend();
	}
});

function OnPlayerChatSend()
{
	SendChat(playerchatinput.value);
	playerchatinput.value="";
}

function RefreshPlayerIcon()
{
	//volume
	volumebar.value = videoBarrage.volume*100;
	if(videoBarrage.volume<=0)
		volumebtn.classList.add("mute-icon");
	else
		volumebtn.classList.remove("mute-icon");
	
	//Barrage
	if(canvasBarrage.style.display=="none")
		barragebtn.classList.add("uncheck-icon");
	else
		barragebtn.classList.remove("uncheck-icon");
	
	//Play
	if (!videoBarrage.paused)
		playbtn.classList.add("pause-icon");
	else
		playbtn.classList.remove("pause-icon");
	
	//View size
	if(titlebar.style.display=="none" && chatarea.style.display=="none")	
		viewbtn.classList.add("windowview-icon");
	else
		viewbtn.classList.remove("windowview-icon");
	
	//Full 
	if(IsFullScreen())
		fullbtn.classList.add("minimize-icon");
	else
		fullbtn.classList.remove("minimize-icon");
}
