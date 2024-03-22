using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FilterController : MonoBehaviour
{
    public enum Frame
    {
        None,
        singapore,
        christmas,
        chinese,
        party,
        study,
        sea
    }

    public UIHandler ui;
    public Transform[] filterContainer;
    public List<Toggle> toggles = new List<Toggle>();
    public int filterAmount = 2;
    public Camera zapparCam;
    public RectTransform filterRect;
    public RectTransform filterButton;
    public int frameId;

    private List<GameObject> filters = new List<GameObject>();
    private bool isMenuOpened;

    private void Start()
    {
#if UNITY_WEBGL
        WebHandler.OnReceiveJson += ResetFilter;
#endif
        Init();
    }

    private void OnDestroy()
    {
#if UNITY_WEBGL
        WebHandler.OnReceiveJson -= ResetFilter;
#endif
    }

    /// <summary>
    /// Function for each filter selection buttons
    /// </summary>
    /// <param name="index"></param>
    public void SelectFilter(int index)
    {
        //Deactivate all filter first
        foreach (GameObject go in filters)
            go.SetActive(false);

        //Deactivate all frames
        foreach (Transform go in ui.themeFrame.transform)
            go.gameObject.SetActive(false);

        //Set frame ID
        PhotoBoothHandler.FrameId = FrameType(index);
        frameId = index;

        //if no filter, turn camera off
        if (index == 0)
        {
            //zapparCam.enabled = false;
            ui.themeFrame.SetActive(false);
            return;
        }

        //Activate the picked one
        //foreach (Transform container in filterContainer)
        //{
        //    container.GetChild(index-1).gameObject.SetActive(true);
        //}

        //zapparCam.enabled = true;
        ui.themeFrame.SetActive(true);
        ui.themeFrame.transform.GetChild(index - 1).gameObject.SetActive(true);
    }

    /// <summary>
    /// Triggered by webGL when coming back from editor
    /// </summary>
    void ResetFilter(string message = "")
    {
        CloseFilterMenu();
        SelectFilter(0);
        ResetToggles();
    }

    /// <summary>
    /// In General
    /// </summary>
    public void ResetFilter()
    {
        CloseFilterMenu();
        SelectFilter(0);
        ResetToggles();
    }

    private void ResetToggles()
    {
        foreach(Toggle tog in toggles)
        {
            tog.isOn = false;
        }
        toggles[0].isOn = true;
    }

    void Init()
    {
        //Assign toggles function
        if (toggles.Count > 0)
        {
            foreach (Toggle toggle in toggles)
            {
                toggle.onValueChanged.AddListener(delegate { SelectFilter(toggle.transform.GetSiblingIndex()); });
            }
        }

        //Collect all filter gameobjects
        //foreach (Transform container in filterContainer)
        //{
        //    foreach(Transform filter in container)
        //    {
        //        filters.Add(filter.gameObject);
        //    }
        //}

        ResetFilter();
    }

    public void OpenFilterMenu()
    {
        if(!isMenuOpened)
        {
            isMenuOpened = true;
            Sequence seq = DOTween.Sequence();
            seq.Append(filterButton.DOAnchorPosX(-7f, 0.15f).SetEase(Ease.InQuint))
               .Append(filterRect.DOAnchorPosX(-330f, 0.35f).SetEase(Ease.InOutBack));
        }
        else
        {
            isMenuOpened = false;
            Sequence seq = DOTween.Sequence();
            seq.Append(filterRect.DOAnchorPosX(-122f, 0.35f).SetEase(Ease.OutBounce))
               .Append(filterButton.DOAnchorPosX(-48.2f, 0.15f).SetEase(Ease.InQuint));
        }
    }

    public void CloseFilterMenu()
    {
        isMenuOpened = false;
        filterRect.DOAnchorPosX(-122f, 0f);
    }

    string FrameType(int index) => 
        index switch
        {
            0 => Frame.None.ToString(),
            1 => Frame.singapore.ToString(),
            2 => Frame.christmas.ToString(),
            3 => Frame.chinese.ToString(),
            4 => Frame.party.ToString(),
            5 => Frame.study.ToString(),
            6 => Frame.sea.ToString(),
        };
}
