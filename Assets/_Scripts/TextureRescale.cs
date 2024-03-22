using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class TextureRescale : MonoBehaviour
{
    public Material material;

    public static Texture2D RenderMaterial(ref Material material, Vector2Int resolution, string filename = "")
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(resolution.x, resolution.y);
        Graphics.Blit(null, renderTexture, material);

        Texture2D texture = new Texture2D(resolution.x, resolution.y, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(Vector2.zero, resolution), 0, 0);
        texture.Apply();
#if UNITY_EDITOR
        // optional, if you want to save it:
        //if (filename.Length != 0)
        //{
        //    byte[] png = texture.EncodeToPNG();
        //    File.WriteAllBytes(Path.Combine("Assets/Screenshots/", filename), png);
        //    AssetDatabase.Refresh();
        //}
#endif
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture;
    }

    public Texture2D ResizeTexture(Texture2D originalTexture, int newWidth, int newHeight)
    {
        /*material.SetTexture("_MainTex", originalTexture);*/  // or whichever the main texture property name is
        material.mainTexture = originalTexture;  // or can do this instead with some materials
        return RenderMaterial(ref material, new Vector2Int(newWidth, newHeight));
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        image.texture = ResizeTexture(test, 3840, 2160);
    //    }
    //}
}
