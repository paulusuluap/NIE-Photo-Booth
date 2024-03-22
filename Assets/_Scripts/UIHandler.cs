using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance;

    #region GameObjects

    public GameObject snapButton;
    public GameObject landingPage;
    public GameObject dataConsentPage;
    public GameObject photoBooth;
    public GameObject photoEditing;
    public GameObject tutorialParent;
    public GameObject tutorial_1;
    public GameObject tutorial_2;
    public GameObject filterSelection;
    public GameObject themeFrame;
    public GameObject loadingPanel;
    public GameObject frameEnd;

    public GameObject countdownCircle;
    public GameObject smileCircle;
    public GameObject homeButton;

    #endregion

    #region UIs

    public CanvasGroup fadePanel;
    public Slider loadingBar;
    public TextMeshProUGUI loadingPercentage;
    public TextMeshProUGUI countdownCounter;

    #endregion

    #region Fields

    public WebHandler Web;

    #endregion


    private void Awake() => Instance = this;

    private void OnEnable()
    {
        WebHandler.OnReceiveJson += ControlPage;
    }

    private void OnDisable()
    {
        WebHandler.OnReceiveJson -= ControlPage;
    }
    private void Start()
    {
        HandleObject(landingPage);
    }

    //private void LateUpdate()
    //{
    //    if (!loadingPanel.activeSelf) 
    //        return;

    //    loadingPercentage.text = (loadingBar.value * 100f).ToString("F0") + "%";
    //}

    public void HandleObject(GameObject obj)
    {
        landingPage.SetActive(landingPage == obj);
        dataConsentPage.SetActive(dataConsentPage == obj);
        photoBooth.SetActive(photoBooth == obj);
        photoEditing.SetActive(photoEditing == obj);

        if(landingPage.activeInHierarchy)
        {
            //reset tutorial
            tutorial_1.SetActive(true);
            //tutorial_2.SetActive(false);
            countdownCounter.text = "";

#if UNITY_WEBGL && !UNITY_EDITOR
            //Send message to web
            Web.SendMessageToIframeParent("home");
#endif
        }
        else if(photoBooth.activeInHierarchy)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            //Send message to web
            Web.SendMessageToIframeParent("camera");
#endif
        }
    }

    private void ControlPage(string message)
    {
        switch (message)
        {
            case "home":
                HandleObject(landingPage);
                break;
            case "retake":
                HandleObject(photoBooth);
                break;
        }
    }

    public void ResetUis()
    {
        countdownCounter.text = "";
        fadePanel.alpha = 0;
        snapButton.SetActive(true);
        homeButton.SetActive(true);
        //filterSelection.SetActive(true);
        loadingPanel.SetActive(false);
        countdownCircle.SetActive(false);
        smileCircle.SetActive(false);
        //frameEnd.SetActive(false);
        //loadingBar.value = 0;
        //loadingPercentage.text = "0";

        //foreach(Transform t in frameEnd.transform)
        //    t.gameObject.SetActive(false);
    }
}
