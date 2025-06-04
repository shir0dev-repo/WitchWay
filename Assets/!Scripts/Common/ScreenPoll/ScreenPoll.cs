using UnityEngine;
using UnityEngine.Events;

public class ScreenPoll : MonoBehaviour
{
    public static ScreenPoll Instance { get; private set; }
    public UnityEvent<int, int> OnResolutionChanged;

    [SerializeField, Range(1, 10)] private int _screenPollFrameCount = 2;

    private int _currPollCount = 0;
    
    private int _lastWidth;
    private int _lastHeight;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SetWidthAndHeight();

        OnResolutionChanged?.Invoke(Screen.width, Screen.height);
        ShouldPoll();
    }

    private void Update()
    {
        
        if (!ShouldPoll()) return;

        if (DidScreenSizeChange())
        {
            OnResolutionChanged?.Invoke(Screen.width, Screen.height);
            SetWidthAndHeight();
        }
    }

    private void SetWidthAndHeight()
    {
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;
    }

    private bool ShouldPoll()
    {
        _currPollCount = (_currPollCount + 1) % _screenPollFrameCount;
        return _currPollCount == 0;
    }

    private bool DidScreenSizeChange()
    {
        return _lastWidth != Screen.width || _lastHeight != Screen.height;
    }
}
