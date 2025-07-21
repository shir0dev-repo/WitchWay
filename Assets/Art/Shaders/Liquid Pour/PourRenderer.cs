using UnityEngine;

public class PourRenderer : MonoBehaviour
{
    [SerializeField] private int _pourThresholdDegrees = 45;
    [SerializeField] private Transform _origin = null;
    [SerializeField] private GameObject _streamPrefab = null;
    private bool _isPouring = false;
    private Stream _currentStream = null;

    private void Update()
    {
        bool pouring = CalculatePourAngle() < _pourThresholdDegrees;
        if (_isPouring != pouring)
        {
            _isPouring = pouring;

            if (_isPouring)
                StartPour();
            else
                EndPour();
        }
    }

    private void StartPour()
    {
        Debug.Log("Starting Pour!");
        _currentStream = CreateStream();
        _currentStream.gameObject.SetActive(true);
        _currentStream.Begin();
    }

    private void EndPour()
    {
        Debug.Log("Ending Pour.");
        if (_currentStream != null)
            _currentStream.gameObject.SetActive(false);
    }

    private float CalculatePourAngle()
    {
        return transform.up.y * Mathf.Rad2Deg;
    }

    private Stream CreateStream()
    {
        if (_currentStream != null)
            return _currentStream;

        GameObject streamObj = Instantiate(_streamPrefab, _origin.position, Quaternion.identity, transform);
        return streamObj.GetComponent<Stream>();
    }
}
