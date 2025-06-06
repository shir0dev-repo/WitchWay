using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using System.IO;
using TMPro;
using System;

public class SymbolPainter : MonoBehaviour
{
    private const string _SAVE_PATH = "Symbols/";

    [SerializeField] private Transform _gestureLRPrefab;
    [SerializeField] private Canvas _UICanvas;

    public static Action<string, float> OnGestureCompleted;
    public static Action OnLineDrawn;

    [Header("Debug")]
    [SerializeField] private bool _useDebug = true;
    [SerializeField] private TextMeshProUGUI _message;

    private readonly List<Gesture> _trainingSet = new();

    private List<Point> _points = new();
    private int strokeID = -1;
    private int _vertexCount = 0;

    private Vector3 _virtualKeyPosition = Vector3.zero;

    private readonly List<LineRenderer> _gestureLineRenderers = new();
    public LineRenderer CurrentGestureRenderer { get; private set; }

    private void Start()
    {
        InitGestures();
    }

    public void Enable(int craftingStationID)
    {
        bool active = craftingStationID == 2;
        _message.enabled = active;
        this.enabled = active;
    }

    public void Clear()
    {
        strokeID = -1;
        _vertexCount = 0;

        _message.text = "";
        CurrentGestureRenderer = null;

        foreach (var g in _gestureLineRenderers)
        {
            Destroy(g.gameObject);
        }

        _gestureLineRenderers.Clear();
        _points.Clear();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            _virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnLineDrawn?.Invoke();
        }

        if (_UICanvas.pixelRect.Contains(_virtualKeyPosition))
        {
            if (Input.GetMouseButtonDown(0))
            {
                ++strokeID;

                Transform tmpGesture = Instantiate(_gestureLRPrefab, transform.position, transform.rotation).transform;
                CurrentGestureRenderer = tmpGesture.GetComponent<LineRenderer>();
                _gestureLineRenderers.Add(CurrentGestureRenderer);
                _vertexCount = 0;
            }

            if (Input.GetMouseButton(0))
            {
                if (CurrentGestureRenderer == null) return;

                _points.Add(new Point(_virtualKeyPosition.x, -_virtualKeyPosition.y, strokeID));

                CurrentGestureRenderer.positionCount = ++_vertexCount;
                CurrentGestureRenderer.SetPosition(_vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(_virtualKeyPosition.x, _virtualKeyPosition.y, 10)));
            }
        }
    }

    public void ValidateGesture()
    {
        if (_points.Count <= 0) return;
        bool valid = RecognizeSymbol(out string gName, out float gScore);

        foreach (LineRenderer lr in _gestureLineRenderers)
        {
            if (lr == CurrentGestureRenderer)
            {
                CurrentGestureRenderer = null;
            }

            Destroy(lr.gameObject);
        }

        _gestureLineRenderers.Clear();
        _points.Clear();
        if (valid)
        {
            OnGestureCompleted?.Invoke(gName, gScore);
        }
    }

    public void RemoveLastLine()
    {
        strokeID--;
        int pCount = CurrentGestureRenderer.positionCount;
        _points.RemoveRange(_points.Count - pCount, pCount);
        int gIndex = _gestureLineRenderers.IndexOf(CurrentGestureRenderer);
        _gestureLineRenderers.Remove(CurrentGestureRenderer);
        if (gIndex -1 >= 0)
        {
            CurrentGestureRenderer = _gestureLineRenderers[gIndex - 1];
        }
        else
        {
            CurrentGestureRenderer = null;
        }
    }

    private void InitGestures()
    {
        if (_useDebug)
        {
            TextAsset[] gesturesXML = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
            foreach (TextAsset gestureXml in gesturesXML)
            {
                _trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));
            }
        }

        string path = Path.Combine(Application.streamingAssetsPath, _SAVE_PATH);
        string[] filePaths = Directory.GetFiles(path, "*.xml");
        foreach (string filePath in filePaths)
        {
            _trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
        }
    }

    public bool RecognizeSymbol(out string gestureName, out float score)
    {
        Gesture candidate = new Gesture(_points.ToArray());
        Result gestureResult = PointCloudRecognizer.Classify(candidate, _trainingSet.ToArray());

        _message.text = gestureResult.GestureClass + " " + $"{gestureResult.Score:F2}";
        strokeID = 0;

        gestureName = gestureResult.GestureClass;
        score = gestureResult.Score;
        return !gestureResult.GestureClass.Equals("null");
    }

    internal void ToggleMessage(bool toggle)
    {
        _message.enabled = toggle;
    }
}
