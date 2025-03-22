const VideoSyncEvents = {
	'SyncTimeReady':'SyncTimeReady',
	'SyncTimeError':'SyncTimeError',
};

var VideoSyncTimer = null;
var VideoSyncInterval = 30000;//check every 30s

var VideoSyncApiUrl = location.protocol + '//' + location.host +"/api/videosync" ;

function GetSyncTime(params) {
    var jqxhr = $.ajax({
		url: VideoSyncApiUrl,
		timeout: 10000,
		success: function(data) 
		{
            console.log("GetSyncTime: " + data);
			var evt = new CustomEvent(VideoSyncEvents['SyncTimeReady'], { 'detail': data });
            window.dispatchEvent(evt);
		},
        error: function(xhr, status, error) 
		{
			var evt = new CustomEvent(VideoSyncEvents['SyncTimeError'], {'detail': error});
			window.dispatchEvent(evt);
		}
	});
}

function StartVideoSyncTimer() {
    if(VideoSyncTimer)
        console.log("VideoSyncTimer is already running");
    else
        VideoSyncTimer = setInterval( GetSyncTime, VideoSyncInterval);

}

function StopVideoSyncTimer() {
    if(VideoSyncTimer)
    {
        console.log("StopVideoSyncTimer");
        clearInterval(VideoSyncTimer);
        VideoSyncTimer = null;
    }
    else
        console.log("VideoSyncTimer is not running");
}

function IsVideoSyncTimerEnabled() {
    return VideoSyncTimer != null;
}

function secondsToHMS(seconds) {
    let h = Math.floor(seconds / 3600);
    let m = Math.floor((seconds % 3600) / 60);
    let s = Math.floor(seconds % 60);

    return [h, m, s].map(unit => String(unit).padStart(2, '0')).join(':');
}