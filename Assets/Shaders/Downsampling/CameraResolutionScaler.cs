using UnityEngine;

[ExecuteInEditMode]
public class CameraResolutionScaler : MonoBehaviour
{
    public enum Resolution
    {
        [Tooltip("960x544")] High,
        [Tooltip("720x408")] Mid,
        [Tooltip("640x363")] Low
    }

    public bool EnableInternalResolution = true;
    public Resolution InternalResolution;
    public Resolution CurrentResolution;

    private new Camera camera;
    private Rect originalRect;
    private float renderDivisor;
    private RenderTexture renderTexture;
    private Rect scaledRect;
    private int width;
    private int height;

    private void Awake()
    {
        camera = Camera.main;
        

        originalRect = camera.pixelRect;

        switch (CurrentResolution)
        {
            case Resolution.High:
                width = 960;
                height = 544;
                QualitySettings.vSyncCount = 2;
                break;
            case Resolution.Mid:
                width = 720;
                height = 408;
                QualitySettings.vSyncCount = 2;
                break;
            case Resolution.Low:
                width = 640;
                height = 368;
                QualitySettings.vSyncCount = 1;
                break;
        }

        //if (!Application.isEditor) 
            Screen.SetResolution(width, height, true);
    }

    private void OnDisable()
    {
        camera.pixelRect = originalRect;
    }

    private void OnDestroy()
    {
        camera.pixelRect = originalRect;
    }

    private void OnPreRender()
    {
        if (EnableInternalResolution)
        {
            renderDivisor = (InternalResolution) switch
            {
                Resolution.High => 1.2f,
                Resolution.Mid => 1.5f,
                Resolution.Low => 1.6f,
                _ => 1.0f
            };

            originalRect = camera.pixelRect;
            scaledRect.Set(0, 0, width / renderDivisor, height / renderDivisor);
            camera.pixelRect = scaledRect;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (EnableInternalResolution)
        {
            Graphics.Blit(source, destination, null, 0);
            camera.pixelRect = originalRect;
            Graphics.Blit(destination, destination);
        }
    }
}
