using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;

namespace Shir0dev.SpriteMaker
{
    [System.Serializable]
    public class SpriteTarget
    {
        [Header("Object")]
        public GameObject TargetPrefab;
        public Vector3 Position;
        public Vector3 EulerRotation;
        public Vector3 Scale = Vector3.one;

        [Header("IO")]
        public string OutputFileName;
    }

    public class SpriteMaker : MonoBehaviour
    {
        public class UnprocessedTexture
        {
            public Texture2D Texture;
            public string FileName;
        }

        [SerializeField] private SpriteTarget[] _spritedObjects;

        [Header("IO")]
        [SerializeField] private RenderTexture _renderTexture;
        [SerializeField] private Color _backgroundColour;
        [SerializeField, Range(0, 1)] private float _tolerance = 0.1f;
        [SerializeField] private string _filePath;

        private ConcurrentQueue<UnprocessedTexture> _texturesToProcess = new();

        private void Start()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                _filePath = Application.streamingAssetsPath;
            }

            RenderTexture.active = _renderTexture;
        }

        [ContextMenu("Create Sprites")]
        public void CreateSprites()
        {
            if (string.IsNullOrEmpty(_filePath)) _filePath = Application.streamingAssetsPath;

            Camera cam = Camera.main;
            CameraClearFlags oldFlags = cam.clearFlags;
            cam.clearFlags = CameraClearFlags.Color;
            RenderTexture oldRT = RenderTexture.active;
            
            foreach (SpriteTarget target in _spritedObjects)
            {
                GameObject obj = Instantiate(target.TargetPrefab, target.Position, Quaternion.Euler(target.EulerRotation));
                obj.transform.localScale = target.Scale;
                if (obj.TryGetComponent(out Rigidbody rgbd))
                {
                    DestroyImmediate(rgbd);
                }

                RenderTexture.active = cam.targetTexture;
                Camera.main.Render();
                
                Texture2D texture = TextureIO.ConvertRenderTextureToTexture2D(cam.targetTexture);
                _texturesToProcess.Enqueue(new UnprocessedTexture() { Texture = texture, FileName = target.OutputFileName });
                DestroyImmediate(obj);
                
                RenderTexture.active = oldRT;
            }

            RenderTexture.active = oldRT;
            cam.clearFlags = oldFlags;

            if (_texturesToProcess.Count < 1)
            {
                Debug.LogWarning("No sprites could be created!");
                return;
            }

            ProcessTextures();
        }

        private void ProcessTextures()
        {
            while (_texturesToProcess.TryDequeue(out UnprocessedTexture tex))
            {
                ProcessTexture(tex, _backgroundColour, _tolerance, _filePath);
            }
        }

        private static void ProcessTexture(UnprocessedTexture unprocessed, Color backgroundColor, float tolerance, string filePath)
        {
            Texture2D texture = unprocessed.Texture;
            int width = texture.width, height = texture.height;
            float sqrTolerance = tolerance * tolerance;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = texture.GetPixel(x, y);
                    Vector3 diff = new Vector3(
                        Mathf.Abs(pixelColor.r - backgroundColor.r),
                        Mathf.Abs(pixelColor.g - backgroundColor.g),
                        Mathf.Abs(pixelColor.b - backgroundColor.b));

                    // Color is similar enough to background color, set alpha to 0.
                    if (diff.sqrMagnitude <= sqrTolerance)
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }

            texture.Apply();
            TextureIO.SaveTexture2DToFile(texture, filePath, unprocessed.FileName);
        }
    }
}