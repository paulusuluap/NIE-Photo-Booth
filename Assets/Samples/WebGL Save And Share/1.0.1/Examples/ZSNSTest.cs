using UnityEngine;
using Zappar.Additional.SNS;

public class ZSNSTest : MonoBehaviour
{
    [Range(0f,1f)]
    public float encodeQuality = 0.75f;

    private void Start()
    {
        ZSaveNShare.Initialize();
    }

    public void TakeSnapshot()
    {
        ZSaveNShare.RegisterSNSCallbacks(OnSaved, OnShared, OnPromptClosed);
        StartCoroutine(ZSaveNShare.TakeSnapshot(OnSnapshotCaptured, encodeQuality));
    }

    public void OnSnapshotCaptured()
    {
        Debug.Log("Open prompt");
        ZSaveNShare.OpenSNSSnapPrompt();
    }

    public void OnSaved()
    {
        Debug.Log("Prompt saved");
    }

    public void OnShared()
    {
        Debug.Log("Prompt shared");
    }

    public void OnPromptClosed()
    {
        Debug.Log("Save and share prompt closed");
        ZSaveNShare.DeregisterSNSCallbacks(OnSaved, OnShared, OnPromptClosed);
    }
}
