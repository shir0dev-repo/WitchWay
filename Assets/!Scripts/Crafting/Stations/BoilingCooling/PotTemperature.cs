using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;

public class PotTemperature : Singleton<PotTemperature>
{
    public const float MIN_TEMP = -50;
    public const float MAX_TEMP = 50;

    public enum HeatingState { None, Heating, Cooling };

    private TemperatureTarget _coolingTarget;
    private TemperatureTarget _heatingTarget;

    private bool _isCooking = false;
    private bool _temperatureChangedThisFrame = false;

    public FailState_BurnCool FailState { get; private set; }

    [Header("Mechanic Settings")]
    [SerializeField] private float _progressIncrement = 5.0f;
    [SerializeField] private float _progressDecrement = 4.0f;

    [Space]
    [SerializeField] private float _temperatureIncrement = 10.0f;
    [SerializeField] private float _temperatureDecrement = 5.0f;

    [Space]
    [SerializeField] private float _allowedTemperatureDeviance = 5.0f;
    [SerializeField] private float _targetChangeDuration = 3.0f;
    private float _targetChangeTimer = 0.0f;


    [Header("References")]
    [SerializeField] private TemperatureButtons _heatingButton;
    [SerializeField] private TemperatureButtons _coolingButton;

    public float Temperature { get; private set; } = 0;
    public float Progress { get; private set; } = 0;

    public WorldIngredient GetWorldIngredient() { return _targetIngredient; }
    private WorldIngredient _targetIngredient;

    public bool IsCurrentlyBurning { get; set; }

    [Header("Audio")]
    public EventReference heatingSound;
    public EventReference coolingSound;
    public EventReference constantTempSound;
    public EventReference successSound;

    private void Setup()
    {
        Progress = 0;
        Temperature = 0;

        _heatingTarget = new(MAX_TEMP, _allowedTemperatureDeviance);
        _coolingTarget = new(MIN_TEMP, _allowedTemperatureDeviance);

        _heatingButton.gameObject.SetActive(true);
        _coolingButton.gameObject.SetActive(true);
    }

    protected override void Awake()
    {
        base.Awake();

        FailState = GetComponent<FailState_BurnCool>();
    }

    void Update()
    {
        if (!_isCooking) return;

        UpdateProgress();
        UpdateTargets();
        if (_temperatureChangedThisFrame)
        {
            _temperatureChangedThisFrame = false;
        }
        else
        {
            EqualOutTemp();
        }
        if (SoundManager.Instance != null)
        {
            if (SoundManager.Instance.IsLooping("ConstantTempSound"))
            {
                float normalizedTemp = Mathf.InverseLerp(-50f, 50f, Temperature);
                SoundManager.Instance.SetParameterByName("ConstantTempSound", "temperature", normalizedTemp);
            }
            else
                SoundManager.Instance.PlayLoop("ConstantTempSound", constantTempSound, transform.position);
        }
    }

    private void UpdateTargets()
    {
        _targetChangeTimer += Time.deltaTime;
        if (_targetChangeTimer > _targetChangeDuration)
        {
            _targetChangeTimer -= _targetChangeDuration;

            float heatTarget = Random.Range(15, MAX_TEMP);
            _heatingTarget.SetTarget(heatTarget);
            GameEvents.Crafting.OnTemperatureTargetChanged?.Invoke(heatTarget, HeatingState.Heating);

            float coolTarget = Random.Range(MIN_TEMP, -15);
            
            _coolingTarget.SetTarget(coolTarget);
            GameEvents.Crafting.OnTemperatureTargetChanged?.Invoke(coolTarget, HeatingState.Cooling);
        }

        _heatingTarget.Update(Temperature);
        _coolingTarget.Update(Temperature);
    }

    private void UpdateProgress()
    {
        if (_heatingTarget.IsWithinDeviance(Temperature) || _coolingTarget.IsWithinDeviance(Temperature))
        {
            Progress = Mathf.Clamp(Progress + _progressIncrement * Time.deltaTime, 0, 100);
        }
        else
        {
            Progress = Mathf.Clamp(Progress - _progressDecrement * Time.deltaTime, 0, 100);
        }

        if (Progress < 100)
        {
            GameEvents.Crafting.OnTemperaturePotProgressChanged?.Invoke(Progress);
        }
        else
        {
            _isCooking = false;

            float timeHeated = _heatingTarget.TimeSpentAtTarget;
            float timeCooled = _coolingTarget.TimeSpentAtTarget;

            if (timeHeated >= timeCooled)
            {
                _targetIngredient.ModifiedState.Heat();
                GameEvents.Crafting.OnItemSuccessfullyHeated?.Invoke(_targetIngredient);
            }
            else
            {
                _targetIngredient.ModifiedState.Freeze();
                GameEvents.Crafting.OnItemSuccessfullyFrozen?.Invoke(_targetIngredient);
            }

            _targetIngredient = null;
            _heatingButton.gameObject.SetActive(false);
            _coolingButton.gameObject.SetActive(false);
        }
    }

    public void ModifyTemperature(HeatingState state)
    {
        switch (state)
        {
            case HeatingState.Heating:
                Temperature = Mathf.Clamp(Temperature + _temperatureIncrement * Time.deltaTime, MIN_TEMP, MAX_TEMP);
                GameEvents.Crafting.OnPotTemperatureChanged?.Invoke(Temperature);
                _temperatureChangedThisFrame = true;
                break;
            case HeatingState.Cooling:
                Temperature = Mathf.Clamp(Temperature - _temperatureIncrement * Time.deltaTime, MIN_TEMP, MAX_TEMP);
                GameEvents.Crafting.OnPotTemperatureChanged?.Invoke(Temperature);
                _temperatureChangedThisFrame = true;
                break;
            default:
                break;
        }
    }

    private void PlaceInPot(IFollowCursor cursorObj)
    {
        if (cursorObj is WorldIngredient wIng)
        {
            if (wIng == _targetIngredient)
            {
                GameEvents.Crafting.OnItemPlacedInTemperaturePot?.Invoke(wIng);
                Setup();
                _isCooking = true;
                //wIng.gameObject.SetActive(false);
            }
        }

        GameEvents.Crafting.OnObjectRemovedFromCursor -= PlaceInPot;
    }

    void EqualOutTemp()
    {
        float toMiddle = Mathf.Clamp01(Mathf.Abs(Temperature) * 0.01f);
        float value = Mathf.Lerp(5, 0, toMiddle);
        // toMiddle is the temp's percentage away from zero, no matter if its pos or neg
        // since this is called every frame, make value small

        if (Temperature > 0)
        {
            Temperature -= value * Time.deltaTime;
            if (Temperature < 0.001f) Temperature = 0;
            // snapping to zero to prevent infinite calculations
        }
        else if (Temperature < 0)
        {
            Temperature += value * Time.deltaTime;
            if (Temperature > 0.001f) Temperature = 0;
        }
        else { Temperature = 0; }

        if (Temperature > 0) { Temperature = Mathf.Clamp(Temperature, 0, 50); }
        else { Temperature = Mathf.Clamp(Temperature, -50, 0); }

        GameEvents.Crafting.OnPotTemperatureChanged?.Invoke(Temperature);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent(out WorldIngredient ing))
        {
            if (!ing.BaseIngredient.CanBeFrozen || !ing.BaseIngredient.CanBeHeated)
                return;
        }

        _targetIngredient = ing;
        if (CursorManager.Instance != null)
        {
            Transform cursorObj = CursorManager.Instance.AttachedObject;
            if (cursorObj != null && cursorObj == other.transform)
                GameEvents.Crafting.OnObjectRemovedFromCursor += PlaceInPot;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out WorldIngredient ing)) return;
        else if (ing != _targetIngredient) return;

        _targetIngredient = null;
        GameEvents.Crafting.OnObjectRemovedFromCursor -= PlaceInPot;

    }
}
