var OpenWindowPlugin = {
    openWindow: function(link)
    {
    	var url = link;
        document.onmouseup = function()
        {
        	window.open(url,'_blank');
        	document.onmouseup = null;
        }
    },

    _JS_WebCamVideo_Start: function(deviceId)
    {
        window.alert("test!");

        if (activeWebCams[deviceId]) {
            ++activeWebCams[deviceId].refCount;
            return
        }
        if (!videoInputDevices[deviceId]) {
            console.error("Cannot start video input with ID " + deviceId + ". No such ID exists! Existing video inputs are:");
            console.dir(videoInputDevices);
            return
        }
        navigator.mediaDevices.getUserMedia({
            audio: false,
            video: videoInputDevices[deviceId].deviceId ? {
                width: 3840,
                height: 2160,
                deviceId: {
                    exact: videoInputDevices[deviceId].deviceId
                }
            } : true
        }).then(function(stream) {
            var video = document.createElement("video");
            video.srcObject = stream;
            if (/(iPhone|iPad|iPod)/.test(navigator.userAgent)) {
                warnOnce("Applying iOS Safari specific workaround to video playback: https://bugs.webkit.org/show_bug.cgi?id=217578");
                video.setAttribute("playsinline", "")
            }
            video.play();
            var canvas = document.createElement("canvas");
            activeWebCams[deviceId] = {
                video: video,
                canvas: document.createElement("canvas"),
                stream: stream,
                frameLengthInMsecs: 1e3 / stream.getVideoTracks()[0].getSettings().frameRate,
                nextFrameAvailableTime: 0,
                refCount: 1
            }
        }).catch(function(e) {
            console.error("Unable to start video input! " + e)
        })
    }
};

mergeInto(LibraryManager.library, OpenWindowPlugin);