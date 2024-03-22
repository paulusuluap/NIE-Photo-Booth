using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Webcam : MonoBehaviour
{
    public static Webcam Instance;
    public RawImage camImage = default;
    private WebCamTexture _texture;
    private WebCamDevice[] devices;
    private int currentCameraIndex;
    public bool reset;
    public bool isMirrored = true;

    private void Awake()
    {
        Instance = this;
#if UNITY_EDITOR
        devices = WebCamTexture.devices;
        _texture = new WebCamTexture();
#endif
        if (isMirrored)
            camImage.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        StartCoroutine(Start());
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
        {
            SwapCamera();
        }
    }

    IEnumerator Start()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            devices = WebCamTexture.devices;
            for (int cameraIndex = 0; cameraIndex < devices.Length; ++cameraIndex)
            {
                Debug.Log(devices[cameraIndex].name);
                //Debug.Log(devices[cameraIndex].isFrontFacing);
            }
        }
        else
        {
            Debug.Log("no webcams found");
        }
    }

    public void SwapCamera()
    {
        if(devices.Length > 0)
        {
            currentCameraIndex++;
            currentCameraIndex %= devices.Length;

            if(_texture != null)
            {
                ActivateCamera(false);
                ActivateCamera(true);
            }
        }
    }

    public void PauseCamera()
    {
        _texture.Stop();
    }

    public void ActivateCamera(bool state)
    {
        if (devices == null) return;
        
        switch(state)
        {
            case true:
                if (devices.Length > 0)
                {
                    if(_texture != null && _texture.isPlaying)
                        _texture.Stop();

                    WebCamDevice device = WebCamTexture.devices[currentCameraIndex];

                    _texture = new WebCamTexture(device.name, 3840, 2160, 30);

                    camImage.texture = _texture;
                    _texture.Play(); 
                }
                break;
            default: 
                if(_texture != null)
                {
                    _texture.Stop();
                    camImage.texture = null;
                    _texture = null;
                }
                break;
        }
    }

    public bool IsCameraPlaying()
    {
        return _texture.isPlaying;
    }

    //Editor only
    //private void OnValidate()
    //{
    //    if (!reset)
    //    {
    //        WebCamDevice[] devices = WebCamTexture.devices;
    //        foreach (var dev in devices)
    //        {
    //            print(dev.name);
    //            if (dev.name.Contains("Tiny"))
    //            {
    //                webcamName = dev.name;
    //            }
    //        }
    //        reset = true;
    //    }
    //}
}