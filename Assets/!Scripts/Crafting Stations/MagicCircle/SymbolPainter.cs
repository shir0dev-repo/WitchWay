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

    public static Action<AlchemicalSymbol> OnSymbolPainted;

    [Header("Debug")]
    [SerializeField] private bool _useDebug = true;
    [SerializeField] private TextMeshProUGUI _message;

    private readonly List<Gesture> _trainingSet = new();

    private List<Point> _points = new();
    private int strokeID = -1;
    private int _vertexCount = 0;

    private Vector3 _virtualKeyPosition = Vector3.zero;
    private Rect _drawArea;

    private readonly List<LineRenderer> _gestureLineRenderers = new();
    private LineRenderer _currentGestureRenderer;

    private void Start()
    {
        _drawArea = new Rect(0, 0, Screen.width, Screen.height);
        InitGestures();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            _virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (_drawArea.Contains(_virtualKeyPosition))
        {
            if (Input.GetMouseButtonDown(0))
            {
                ++strokeID;

                Transform tmpGesture = Instantiate(_gestureLRPrefab, transform.position, transform.rotation).transform;
                _currentGestureRenderer = tmpGesture.GetComponent<LineRenderer>();
                _gestureLineRenderers.Add(_currentGestureRenderer);
                _vertexCount = 0;
            }

            if (Input.GetMouseButton(0))
            {
                _points.Add(new Point(_virtualKeyPosition.x, -_virtualKeyPosition.y, strokeID));

                _currentGestureRenderer.positionCount = ++_vertexCount;
                _currentGestureRenderer.SetPosition(_vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(_virtualKeyPosition.x, _virtualKeyPosition.y, 10)));
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            bool valid = Recognize(AlchemicalSymbol.Necromancy);
            foreach (LineRenderer lr in _gestureLineRenderers)
            {
                if (lr == _currentGestureRenderer)
                {
                    _currentGestureRenderer = null;
                }

                Destroy(lr.gameObject);
            }

            _gestureLineRenderers.Clear();
            _points.Clear();
            if (valid)
            {
                OnSymbolPainted?.Invoke(AlchemicalSymbol.Necromancy);
            }
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

    public bool Recognize(AlchemicalSymbol targetSymbol)
    {
        Gesture candidate = new Gesture(_points.ToArray());
        Result gestureResult = PointCloudRecognizer.Classify(candidate, _trainingSet.ToArray());

        _message.text = gestureResult.GestureClass + " " + $"{gestureResult.Score:F2}";
        AlchemicalSymbol drawnSymbol = GetSymbolFromName(gestureResult.GestureClass);
        strokeID = 0;
        return !gestureResult.GestureClass.Equals("null") && gestureResult.Score > 0.75f && drawnSymbol == targetSymbol;
    }

    private AlchemicalSymbol GetSymbolFromName(string name)
    {
        return (name) switch
        {
            "necromancy" => AlchemicalSymbol.Necromancy,
            _ => AlchemicalSymbol.Evocation
        };
    }
}
