
var player;
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
	video.play();
	SetVolume(100);
	RefreshPlayerIcon();
}, false);

unmutenotify.addEventListener("mouseenter", function( event ) {
	unmutenotify.classList.remove("fadeout");
	unmutenotify.classList.add("fadein");
}, false);

unmutenotify.addEventListener("mouseleave", function( event ) {
	unmutenotify.classList.remove("fadein");
	unmutenotify.classList.add("fadeout");
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



function StartPlayer(video_url,isLive)
{
	//var httpflvUrl = 'http://'+ document.location.hostname +':8081/live/livestream.flv';
	//var hlsUrl = 'http://'+ document.location.hostname +':8081/live/livestream.m3u8';
	//var httpflvUrl = 'http://192.168.88.10:8081/live/livestream.flv';
	//var hlsUrl = 'http://192.168.88.10:8081/live/livestream.m3u8';

	SetVolume(0);

	if(video_url.endsWith(".flv"))
	{
		if(!mpegts.isSupported())
		{
			console.warn('Mpegts is not supported');
			return false;
		}

		PrintInfo("播放器:Mpegts");
		
		player = mpegts.createPlayer({
			type: 'flv',
			isLive: isLive,
			url: video_url
		});

		player.on(mpegts.Events.ERROR, (errorType, errorDetail, errorInfo) => {
			console.error('MPEGTS.js Error:', errorType, errorDetail, errorInfo);
	
			// Handle different error types
			if (errorType === mpegts.ErrorTypes.NETWORK_ERROR) 
			{
				console.warn('Network error occurred:', errorDetail);
			} 
			else if (errorType === mpegts.ErrorTypes.MEDIA_ERROR) 
			{
				console.warn('Media error occurred:', errorDetail);
			} 
			else if (errorType === mpegts.ErrorTypes.OTHER_ERROR) 
			{
				console.warn('Unknown error:', errorDetail);
			}
		});

		player.attachMediaElement(videoBarrage);
		player.load();
	}
	else if(video_url.endsWith(".m3u8") && !isIOSSafari()) 
	{
		if(!Hls.isSupported())
		{
			console.warn('HLS.js is not supported');
			return false;
		}
		PrintInfo("播放器:HLS.js");

		player = new Hls();
		player.loadSource(video_url);
		player.attachMedia(videoBarrage);

		if(isLive)
			PrintInfo("工讀生說 你的瀏覽器延遲很高，換一個好嗎∠( ᐛ 」∠)＿");
	}	
	else//use native video for other formats
	{
		PrintInfo("播放器:Native");
		player = null;
		videoBarrage.setAttribute("src", video_url);
		
		if(isLive)
			PrintInfo("工讀生說 你的瀏覽器延遲真的很高，換一個好嗎∠( ᐛ 」∠)＿");
	}

	unmutenotify.classList.add("fade-in-out");

	videoBarrage.play();

	RefreshPlayerIcon();
	return true;
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
	if(val==0)
		video.muted=true;
	else
		video.muted=false;
	
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
	if(videoBarrage.volume<=0 || videoBarrage.muted)
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

function isIOSSafari() {
    const ua = navigator.userAgent;
    const isIOS = /iPad|iPhone|iPod/.test(ua) || (navigator.platform === 'MacIntel' && navigator.maxTouchPoints > 1);
    const isSafari = /^((?!chrome|android).)*safari/i.test(ua);
    return isIOS && isSafari;
}
