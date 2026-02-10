// WebGL JavaScript Plugin for GMD1
// This file provides safe JavaScript bridge methods for Unity WebGL

mergeInto(LibraryManager.library, {
  
  // Log a message to the browser console
  WebGLLogToConsole: function(message) {
    console.log(UTF8ToString(message));
  },
  
  // Get current browser information
  WebGLGetBrowserInfo: function() {
    var browserInfo = navigator.userAgent;
    var bufferSize = lengthBytesUTF8(browserInfo) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(browserInfo, buffer, bufferSize);
    return buffer;
  },
  
  // Check if the browser supports WebGL
  WebGLCheckSupport: function() {
    try {
      var canvas = document.createElement('canvas');
      return !!(canvas.getContext('webgl') || canvas.getContext('experimental-webgl'));
    } catch(e) {
      return false;
    }
  }
  
});
