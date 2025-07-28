using System;
using UnityEngine;

public class Siphon : Singleton<Siphon>
{
    [Header("Sliders")]
    // TODO: Find way to show pressure in game, not through slider
    [SerializeField] SliderBar _pressureSlider;

    public SiphonPressure Pressure => _pressure;
    [SerializeField] private SiphonPressure _pressure = new(0.0f, 100.0f);

    [Header("Bottle Fill")]
    public float FillAmount { get; private set; }
    [SerializeField] private float _maxStreamRate = 15.0f;
    private float _streamRate = 0.0f;

    private bool _isTargetQKey = true;

    private void Initialize()
    {
        _pressure.Reset();
        _streamRate = 0.0f;
        _pressureSlider.SetValue(0.0f);
    }

    private void OnEnable()
    {
        GameEvents.Crafting.OnBottleFilled += EndMinigame;
        GameEvents.Crafting.OnBottlePlacedInBottler -= StartMinigame;
    }
    private void OnDisable()
    {
        GameEvents.Crafting.OnBottleFilled -= EndMinigame;
        GameEvents.Crafting.OnBottlePlacedInBottler += StartMinigame;
    }

    void Start()
    {
        Initialize();
        _pressureSlider.gameObject.SetActive(false);
        this.enabled = false;
    }

    void Update()
    {
        if (FillAmount >= 100)
        {
            GameEvents.Crafting.OnBottleFilled?.Invoke();
            return;
        }

        _pressure.Update(Time.deltaTime);
        KeyCode targetKey = _isTargetQKey ? KeyCode.Q : KeyCode.E;
        if (Input.GetKeyDown(targetKey))
        {
            _isTargetQKey = !_isTargetQKey;
            _pressure.Increase();
        }
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            _pressure.Increase();
        }*/
        else
        {
            _pressure.Decrease();
        }

        _pressure.DoEase();
        _pressureSlider.SetValue(_pressure.Value);

        FillBottle();
    }

    private void FillBottle()
    {
        UpdateStreamRate();
        if (Mathf.Approximately(_streamRate, 0.0f)) return;

        FillAmount = Mathf.Clamp(FillAmount + Time.smoothDeltaTime * _streamRate, 0.0f, 100.0f);
        GameEvents.Crafting.OnBottleFillChanged?.Invoke(FillAmount * 0.01f);
    }

    private void UpdateStreamRate()
    {
        float pressurePercent = Mathf.InverseLerp(_pressure.MinValue, _pressure.MaxValue, _pressure.Value);
        _streamRate = pressurePercent * _maxStreamRate;
    }

    void StartMinigame(Bottle bottle)
    {
        Initialize();
        _pressureSlider.gameObject.SetActive(true);
        GameEvents.Crafting.OnBottleRemovedFromBottler += EndMinigame;
    }

    void EndMinigame(Bottle _)
    {
        EndMinigame();
        GameEvents.Crafting.OnBottleRemovedFromBottler -= EndMinigame;
    }

    void EndMinigame()
    {
        _pressureSlider.gameObject.SetActive(false);
        this.enabled = false;
    }
}
