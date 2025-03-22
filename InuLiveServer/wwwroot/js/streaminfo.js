const StreamInfoEvents = {
	'StreamInfoQuerying':'StreamInfoQuerying',
	'StreamInfoReady':'StreamInfoReady',
	'StreamInfoFail': 'StreamInfoFail',
	'StreamInfoEmpty': 'StreamInfoEmpty'
};

var StreamInfoApiUrl = location.protocol + '//' + location.host +"/api/streaminfo" ;
 
$(document).ready(function() 
{
	var jqxhr = $.ajax({
		url: StreamInfoApiUrl, 
		timeout: 10000,
		beforeSend: function(){
			var evt = new CustomEvent(StreamInfoEvents['StreamInfoQuerying'], {'detail': StreamInfoApiUrl});
			window.dispatchEvent(evt);
		},
		success: function(data) 
		{	
			if (data) 
			{
				var evt = new CustomEvent(StreamInfoEvents['StreamInfoReady'], { 'detail': data });
				window.dispatchEvent(evt);
			}
			else
			{
				var evt = new CustomEvent(StreamInfoEvents['StreamInfoEmpty'], { });
				window.dispatchEvent(evt);
			}
		},
		error: function(xhr, status, error) 
		{
			var evt = new CustomEvent(StreamInfoEvents['StreamInfoFail'], {'detail': error});
			window.dispatchEvent(evt);
		}
	});
});
