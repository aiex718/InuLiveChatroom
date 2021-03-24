
var StreamInfoUrl = "http://" + document.location.hostname +":10500/api/streaminfo" ;

// var jqxhr = $.get( StreamInfoUrl, function(data) {
	
	// var stream_title = $('#stream_title')[0];
	// var stream_subtitle = $('#stream_subtitle')[0];
	// var stream_game = $('#stream_game')[0];
	
	// stream_title.innerHTML = data.title;
	// stream_subtitle.innerHTML = data.subtitle;
	// stream_game.innerHTML = data.game;
// })
  // .done(function() {
    // alert( "second success" );
  // })
  // .fail(function() {
    // alert( "error" );
  // })
  // .always(function() {
    // alert( "finished" );
  // });
 
 
 $(document).ready(function() 
{
	var jqxhr = $.get( StreamInfoUrl, function(data) {
	
	var stream_title = $('#stream_title')[0];
	var stream_subtitle = $('#stream_subtitle')[0];
	var stream_game = $('#stream_game')[0];
	
	stream_title.innerHTML = data.title;
	stream_subtitle.innerHTML = data.subtitle;
	stream_game.innerHTML = data.game;
	
	SocketConnect();
	
	}).fail(function() 
	{
		PrintInfo("無法連接至實況伺服器");
	});
});
