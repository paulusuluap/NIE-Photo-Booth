mergeInto(LibraryManager.library, {

    Hello: function () {
      window.alert("Hello, world!");
    },

    CommunicateUnityWeb: function (message) {
        var _message = UTF8ToString(message);
        window.parent.postMessage(JSON.stringify(_message), "*");
    },
});