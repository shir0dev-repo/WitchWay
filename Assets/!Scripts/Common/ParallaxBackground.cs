using UnityEngine;
using UnityEngine.UI;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField, Tooltip("0 = static, 1 = full match.")]
    private float parallaxFactor = 0.5f;

    private RawImage _rawImage;
    private Vector2 _uvOffset = Vector2.zero;

    private void Awake()
    {
        _rawImage = GetComponent<RawImage>();
    }

    public void Move(Vector3 deltaMovement)
    {
        _uvOffset.x += (deltaMovement.x/14) * -parallaxFactor;
        _uvOffset.x %= 1f;
        _rawImage.uvRect = new Rect(_uvOffset, _rawImage.uvRect.size);

    }
}
