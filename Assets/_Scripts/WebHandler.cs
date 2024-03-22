using UnityEngine;
using System;
using System.Runtime.InteropServices;
using TMPro;

public class WebHandler : MonoBehaviour
{
    #region JSON Body
    public class WebMessage
    {
        public string trigger;
        public string base_url;
        public string galleryId;
        public string name;
        public string description;
        public string url_album;
        public string url_kiosk;
        public string take_photo_delay;
    }
    public class WebPhotoEditor
    {
        public string trigger;
        public string photoId;
        public string frameId;
    }
    #endregion

    #region JSONObjects

    public WebMessage webMessage = new WebMessage();
    public WebPhotoEditor webPhotoEditor = new WebPhotoEditor();

    #endregion

    #region JS internal functions

    [DllImport("__Internal")]
    private static extern void CommunicateUnityWeb(string message);

    [DllImport("__Internal")]
    private static extern void _JS_WebCamVideo_Start(int deviceId);

    #endregion

    #region Fields

    public TextMeshProUGUI parentMessage;

    #endregion

    #region Events

    public static Action<string> OnReceiveJson;

    #endregion

    public void SendMessageToIframeParent(string trigger, string photoId = "", string frameId = "")
    {
        webPhotoEditor.trigger = trigger;
        if(photoId != string.Empty && frameId != string.Empty)
        {
            webPhotoEditor.photoId = photoId;
            webPhotoEditor.frameId = frameId;
        }
        var messageJson = JsonUtility.ToJson(webPhotoEditor, true);

#if UNITY_WEBGL && !UNITY_EDITOR
        CommunicateUnityWeb(messageJson);
#endif
    }

    private void Start()
    {
        parentMessage.text = string.Empty;
    }

    public void ReceiveJson(string message)
    {
        print("message : " + message);

        //TODO : Deserialize JSON message
        JsonUtility.FromJsonOverwrite(message, webMessage);

        OnReceiveJson?.Invoke(webMessage.trigger);
    }
}
