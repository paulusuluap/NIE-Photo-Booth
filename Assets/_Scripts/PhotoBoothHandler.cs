using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Proyecto26;
using ZXing;
using ZXing.QrCode;

public class PhotoBoothHandler : MonoBehaviour
{
    #region APIs

    [Serializable]
    public class APIPhoto
    {
        public bool status;
        public string message;
        public Data data;
    }
    [Serializable]
    public class Data //will be used for QR Code
    {
        public string gallery_id;
        public string title;
        public string image;
        public string image_thumb;
        public string _id;
        public string description;
        public string date_created;
        public string date_modified;
    }

    public class Body
    {
        public string access_token;
        public string gallery_id;
        public string image;
    }

    #endregion


    #region Fields

    [SerializeField]
    string baseUrl = "https://photobooth.fxwebapps.com";
    [SerializeField]
    string submitEndpoint = "/api/photo/";
    [SerializeField]
    string galleryId = "652e11b09aff88f664db4122";
    [SerializeField]
    string accessToken = "PaulToken";
    public APIPhoto apiPhoto = new APIPhoto();
    [SerializeField]
    Image photoDisplayArea;
    [SerializeField]
    RawImage qrCodePattern;
    [SerializeField]
    Animator countdownAnimation;
    [SerializeField]
    Animator fadeInAnimation;
    [SerializeField]
    Animator fadeOutAnimation;
    [SerializeField]
    Animator fadingAnimation;
    [SerializeField]
    GameObject photoFrame;
    [SerializeField]
    GameObject qrCode;
    [SerializeField]
    WebHandler Web;
    [SerializeField]
    FilterController filter;
    [SerializeField]
    TextureRescale rescaler;

    [Header("Photo Resolution")]
    [SerializeField]
    int photoWidth = 3840;
    [SerializeField] 
    int photoHeight = 2160;


    private UIHandler ui;
    private Texture2D screenCapture;
    private byte[] textureBytes; 
    private int countdownTimer = 3;
    private bool isCountdownStart;
    private bool isEditorOpened;
    private float delayTime = 3f;

    public static Action OnInit;

    private string photoId;
    private static string frameId;
    public static string FrameId { set => frameId = value; }

    #endregion

    private void OnEnable()
    {
        //WebHandler.OnReceiveJson += RemovePhoto;
        WebHandler.OnReceiveJson += ConfigureSetting;
    }

    private void OnDisable()
    {
        //WebHandler.OnReceiveJson -= RemovePhoto;
        WebHandler.OnReceiveJson -= ConfigureSetting;
    }

    private void Start()
    {
        screenCapture = new Texture2D(photoWidth, photoHeight, TextureFormat.RGB24, false);
        ui = this.GetComponent<UIHandler>();
        //Debug.Log(Application.dataPath);
        ConfigureSetting();
        OnInit?.Invoke();
    }

    public void StartCountdown()
    {
        if (!isCountdownStart)
        {
            isCountdownStart = true;
#if UNITY_WEBGL && !UNITY_EDITOR
            //Web.SendMessageToIframeParent("take_photo");
            SendTriggerToIframe("take_photo");
#endif
            StartCoroutine(Countdown());
        }
    }

    private IEnumerator Countdown()
    {
        //Turn off ar effect selection and buttons
        ui.snapButton.SetActive(false);
        //ui.filterSelection.SetActive(false);
        ui.homeButton.SetActive(false);

        //wait until light is on
        yield return new WaitForSeconds(delayTime);

        ui.countdownCircle.SetActive(true);
        ui.countdownCircle.transform.localScale = Vector3.zero;
        ui.countdownCircle.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.InFlash);

        while (countdownTimer > 0)
        {
            ui.countdownCounter.text = countdownTimer.ToString();
            //countdownAnimation.Play("Countdown");

            yield return new WaitForSeconds(1f);

            countdownTimer--;
        }

        ui.countdownCircle.SetActive(false);
        ui.smileCircle.SetActive(true);

        yield return new WaitForSeconds(1f);

        ui.countdownCounter.text = string.Empty;

        //Flash, Turn off Frame
        //ui.themeFrame.SetActive(false);
        ui.smileCircle.SetActive(false);

        yield return new WaitForEndOfFrame();

        //Capture photo after countdown ends
        CapturePhoto();
    }

    public void CapturePhoto()
    {
        StartCoroutine(CapturePhotoCoroutine());
    }

    private IEnumerator CapturePhotoCoroutine()
    {
        yield return new WaitForEndOfFrame();

        //Take photo
        screenCapture.ReadPixels(new Rect(0, 0, photoWidth, photoHeight), 0, 0, false);
        screenCapture.Apply();

        Texture2D scaledText = rescaler.ResizeTexture(screenCapture, 3840, 2160);

        textureBytes = ImageConversion.EncodeToPNG(scaledText);
        string encodePhotoBytes = Convert.ToBase64String(textureBytes);

        //ScreenShot();
        ui.loadingPanel.SetActive(true);

        //Show taken photo
        //ShowPhoto();

        //Stop webcam
        Webcam.Instance.PauseCamera();

        //Send Post request to Server
        PostPhoto(encodePhotoBytes);
    }

    void ScreenShot()
    {
        // capture screen shot on left mouse button down

        string folderPath = "Assets/Screenshots/"; // the path of your project folder

        if (!Directory.Exists(folderPath)) // if this path does not exist yet
             Directory.CreateDirectory(folderPath);  // it will get created

        var screenshotName =
                                "Screenshot_" +
                                DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + // puts the current time right into the screenshot name
                                ".png"; // put youre favorite data format here
        ScreenCapture.CaptureScreenshot(Path.Combine(folderPath, screenshotName), 2); // takes the sceenshot, the "2" is for the scaled resolution, you can put this to 600 but it will take really long to scale the image up
        //File.WriteAllBytes(Path.Combine(folderPath, screenshotName), textureBytes);
        Debug.Log(folderPath + screenshotName); // You get instant feedback in the console
    }

    private void PostPhoto(string encodedPhotoData)
    {
        var currentRequest = new RequestHelper
        {
            Uri = baseUrl + submitEndpoint,
            Body = new Body
            {
                access_token = accessToken,
                gallery_id = galleryId,
                image = encodedPhotoData,
            }
        };

        RestClient.Post<APIPhoto>(currentRequest).Then(response =>
        { 
            if (response.status)
            {
                //print("Post Request Successfull : " + response.status);
                apiPhoto = response;
                photoId = apiPhoto.data._id;

                //Do the loading
                DOVirtual.Int(0, 3, 4, OnLoadingDone).SetEase(Ease.Linear);
            }
            else
            {
                print("Post Request ERROR");
            }
        }
        );
    }
    
    protected void OnLoadingDone(int time)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Webcam.Instance.ActivateCamera(false);
        if(!isEditorOpened)
        {
            Web.SendMessageToIframeParent("editor", photoId, frameId);
            print($"OnLoadingDone => photoId {photoId}");
            isEditorOpened = true;
        }                    
#elif UNITY_EDITOR
        //ShowPhoto();
        //ui.HandleObject(ui.photoEditing);
        ConfigureSetting();
#endif
    }

    private void ShowPhoto()
    {
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0.0f, 0.0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100.0f);
        photoDisplayArea.sprite = photoSprite;

        //Set Barcode Texture
        //qrCodePattern.texture = GenerateQR(baseUrl + $"/description?id={photoId}&group={group}");
        
        //ui.frameEnd.SetActive(true);
        photoFrame.SetActive(true);
        //qrCode.SetActive(true);
        Webcam.Instance.ActivateCamera(false);
        //ui.frameEnd.transform.GetChild(filter.frameId).gameObject.SetActive(true);
    }

    /// <summary>
    /// Clear photo data
    /// </summary>
    /// <param name="message"></param>
    public void ConfigureSetting(string trigger = "")
    {
        //print("ConfigureSetting : " + trigger);

#if UNITY_EDITOR
        //qrCode.SetActive(false);
        //qrCodePattern.texture = null;
#endif
        //Reset
        apiPhoto = null;
        countdownTimer = 3;
        isCountdownStart = false;
        isEditorOpened = false;
        ui.ResetUis();
        //filter.ResetFilter();
        photoFrame.SetActive(false);
        photoDisplayArea.sprite = null;

        switch (trigger)
        {
            case "home": Webcam.Instance.ActivateCamera(false); break;
            case "retake": Webcam.Instance.ActivateCamera(true);break;
            default: //Dynamic gallery ID generated by web
                baseUrl = Web.webMessage.base_url;
                galleryId = Web.webMessage.galleryId;
                delayTime = (float)Convert.ToInt32(Web.webMessage.take_photo_delay) > 0f ?
                            (float)Convert.ToInt32(Web.webMessage.take_photo_delay) : delayTime;

                print($"base {baseUrl}, gallery {galleryId}, delayTime {delayTime}");
                break;
        }
    }

    public void SendTriggerToIframe(string trigger)
    {
        Web.SendMessageToIframeParent(trigger);
        print("SendTriggerToIframe/trigger: " + trigger);
    }

    Texture2D FlipTexture(Texture2D original)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);

        int xN = original.width;
        int yN = original.height;


        for (int i = 0; i < xN; i++)
        {
            for (int j = 0; j < yN; j++)
            {
                flipped.SetPixel(xN - i - 1, j, original.GetPixel(i, j));
            }
        }
        flipped.Apply();

        return flipped;
    }


    //TODO : Generate QR Code in Unity
    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    public Texture2D GenerateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }
}