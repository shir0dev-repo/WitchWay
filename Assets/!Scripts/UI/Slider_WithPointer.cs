using UnityEngine;

public class Slider_WithPointer : SliderBar
{
    public GameObject pointer;
    public void SetPointerLocation(float value)
    {
        float previousY = pointer.transform.localPosition.y;
        RectTransform rect = gameObject.GetComponent<RectTransform>();

        value = Mathf.Clamp(value, slider.minValue, slider.maxValue);
        float normal = Mathf.InverseLerp(slider.minValue, slider.maxValue, value);
        
        float width = rect.rect.width;
        float newX = (normal-0.5f) * width;

        pointer.transform.localPosition = new Vector2(newX, previousY);
    }
}
