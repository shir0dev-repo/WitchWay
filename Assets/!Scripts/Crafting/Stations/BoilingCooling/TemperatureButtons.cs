using DG.Tweening;
using System;
using UnityEngine;

public class TemperatureButtons : MonoBehaviour
{
    [SerializeField] PotTemperature.HeatingState _heatingState;
    PotTemperature pot;
    [SerializeField]
    bool isHeating;

    [SerializeField] float baseValue = 1f;
    [SerializeField, Range(1, 2)] private float _scaleMultiplier = 1.5f;

    private bool mouseOverStart = true;
    private void Start()
    {
        pot = PotTemperature.Instance;

        gameObject.SetActive(false);
    }
    private void OnMouseOver()
    {
        float currTemp = pot.Temperature;
        float toMiddle = Mathf.Clamp01(Mathf.Abs(currTemp) * 0.01f);
        float ease = Easing(toMiddle);
        float t = Mathf.Lerp(10, 0, ease);
        // toMiddle is the current temp away from zero
        // since this is called every frame and is multiplied by basevalue, t is small

        float value = baseValue * t * Time.smoothDeltaTime;

        if (mouseOverStart)
        {
            if (SoundManager.Instance != null)
            {
                var soundToPlay = isHeating ? pot.heatingSound : pot.coolingSound;
                SoundManager.Instance.PlayOneShot(soundToPlay, Camera.main.transform.position);
            }

            transform.DOScale(_scaleMultiplier, 0.15f);
            mouseOverStart = false;
        }

        pot.ModifyTemperature(_heatingState);
    }
    private void OnMouseExit()
    {
        mouseOverStart = true;

        transform.DOScale(1.0f, 0.15f);
    }
    void StartMinigame()
    {
        gameObject.SetActive(true);
    }
    void EndMinigame()
    {
        gameObject.SetActive(false);
    }
    float Easing(float x)
    {
        return x < 0.5f ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
}
