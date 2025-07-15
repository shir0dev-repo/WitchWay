using UnityEngine;
using System.IO;
using Unity.VisualScripting.FullSerializer;

public static class TextureIO
{
    public static void SaveTexture2DToFile(Texture2D texture, string filePath, string fileName = "")
    {
        string path;

        if (!string.IsNullOrWhiteSpace(fileName))
        {
            path = Path.Combine(filePath, fileName) + ".png";
        }
        else
        {
            path = filePath + ".png";
        }
        
        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }

        File.WriteAllBytes(path, texture.EncodeToPNG());
        Debug.Log($"Saved Texture2D to {path}.");
    }

    public static void SaveRenderTextureToFile(RenderTexture renderTexture, string filePath, string fileName = "")
    {
        Texture2D texture = ConvertRenderTextureToTexture2D(renderTexture);

        SaveTexture2DToFile(texture, filePath, fileName);

        if (Application.isPlaying)
        {
            Object.Destroy(texture);
        }
        else
        {
            Object.DestroyImmediate(texture);
        }
    }

    public static Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
    {
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false, true);

        RenderTexture oldRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = oldRT;

        return texture;
    }
}
