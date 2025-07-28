using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TemperaturePotUI : MonoBehaviour
{
    public float TargetLow { get; private set; } = 0;
    public float TargetHigh { get; private set; } = 0;

    [Header("Visuals")]
    [SerializeField] private Slider _temperatureBar;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Gradient _temperatureGradient;

    [Space]
    [SerializeField] private RectTransform _heatingTargetIcon;
    [SerializeField] private RectTransform _coolingTargetIcon;
    [SerializeField] private float _targetMoveAnimDuration = 0.3f;

    private void OnEnable()
    {
        GameEvents.Crafting.OnItemSuccessfullyFrozen += DisableUI;
        GameEvents.Crafting.OnItemSuccessfullyHeated += DisableUI;
        GameEvents.Crafting.OnItemPlacedInTemperaturePot += Setup;
        GameEvents.Crafting.OnTemperaturePotProgressChanged += UpdateProgressSlider;
        GameEvents.Crafting.OnPotTemperatureChanged += UpdateTemperatureSlider;
        GameEvents.Crafting.OnTemperatureTargetChanged += UpdateTargetIcon;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnItemSuccessfullyFrozen -= DisableUI;
        GameEvents.Crafting.OnItemSuccessfullyHeated -= DisableUI;
        GameEvents.Crafting.OnItemPlacedInTemperaturePot -= Setup;
        GameEvents.Crafting.OnTemperaturePotProgressChanged -= UpdateProgressSlider;
        GameEvents.Crafting.OnPotTemperatureChanged -= UpdateTemperatureSlider;
        GameEvents.Crafting.OnTemperatureTargetChanged -= UpdateTargetIcon;
    }

    private void Setup(WorldIngredient ingredient)
    {
        _heatingTargetIcon.gameObject.SetActive(true);
        _coolingTargetIcon.gameObject.SetActive(true);
        _temperatureBar.gameObject.SetActive(true);
        _progressBar.gameObject.SetActive(true);
        UpdateTemperatureSlider(0.0f);
    }

    private void DisableUI(WorldIngredient _)
    {
        _heatingTargetIcon.gameObject.SetActive(false);
        _coolingTargetIcon.gameObject.SetActive(false);
        _temperatureBar.gameObject.SetActive(false);
        _progressBar.gameObject.SetActive(false);

        UpdateTargetIcon(0, PotTemperature.HeatingState.Heating);
        UpdateTargetIcon(0, PotTemperature.HeatingState.Cooling);
    }

    private void UpdateTargetIcon(float targetTemperature, PotTemperature.HeatingState state)
    {
        RectTransform targetIcon = state switch
        {
            PotTemperature.HeatingState.Heating => _heatingTargetIcon,
            PotTemperature.HeatingState.Cooling => _coolingTargetIcon,
            _ => throw new InvalidOperationException()
        };

        StartCoroutine(AnimateTargetCoroutine(targetTemperature, targetIcon));
    }

    private void UpdateProgressSlider(float progress)
    {
        _progressBar.value = progress;
    }

    private void UpdateTemperatureSlider(float temperature)
    {
        float normalized = Mathf.InverseLerp(PotTemperature.MIN_TEMP, PotTemperature.MAX_TEMP, temperature);
        _temperatureBar.value = temperature;
        if (_temperatureBar.fillRect.TryGetComponent(out Image img))
        {
            img.color = _temperatureGradient.Evaluate(normalized);
        }
    }

    private IEnumerator AnimateTargetCoroutine(float targetTemperature, RectTransform targetIcon)
    {
        Vector2 startPos = targetIcon.transform.localPosition;

        float width = _temperatureBar.GetComponent<RectTransform>().rect.width;
        float percent = Mathf.InverseLerp(_temperatureBar.minValue, _temperatureBar.maxValue, targetTemperature);
        Vector2 finalPos = new Vector2((percent - 0.5f) * width, startPos.y);

        float timer = 0;
        while (timer <= _targetMoveAnimDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / _targetMoveAnimDuration;
            targetIcon.transform.localPosition = Vector2.Lerp(startPos, finalPos, progress);
            yield return new WaitForEndOfFrame();
        }
    }
}
