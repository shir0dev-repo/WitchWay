using UnityEngine;

public class Slider_WithPointer : SliderBar
{
    public GameObject pointer;
    public void SetPointerLocation(float value)
    {
        Vector2 prevLocation = pointer.transform.localPosition;
        RectTransform transform = gameObject.GetComponent<RectTransform>();

        value = Mathf.Clamp(value, slider.minValue, slider.maxValue);
        float normal = Mathf.InverseLerp(slider.minValue, slider.maxValue, value);
        
        float width = transform.rect.width;
        float newX = (normal-0.5f) * width;

        pointer.transform.localPosition = new Vector2(newX, prevLocation.y);
    }
}
