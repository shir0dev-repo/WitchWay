using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField, Tooltip("How much this layer moves in response to the crafting station. 0 = static, 1 = full match.")]
    private float parallaxFactor = 0.5f;

    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.position;
    }

    public void Move(Vector3 deltaMovement)
    {
        transform.position = transform.position + deltaMovement * parallaxFactor;
    }
}
