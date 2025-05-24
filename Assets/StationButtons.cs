using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class StationButtons : MonoBehaviour
{
    [SerializeField] private List<RectTransform> stationButtons;
    [SerializeField] private float dropAmount = 50f;
    [SerializeField] private float animationTime = 0.1f;

    private List<Vector2> originalPositions = new List<Vector2>();

    private void Start()
    {
        foreach (var btn in stationButtons)
        {
            originalPositions.Add(btn.anchoredPosition);
        }

        if (StationManager.Instance != null)
            StationManager.Instance.OnStationChanged.AddListener(OnStationChanged);
    }

    private void OnDestroy()
    {
        if (StationManager.Instance != null)
            StationManager.Instance.OnStationChanged.RemoveListener(OnStationChanged);
    }

    private void OnStationChanged(int index)
    {
        for (int i = 0; i < stationButtons.Count; i++)
        {
            if (i == index)
            {
                StopAllCoroutines();
                StartCoroutine(AnimateButton(i, originalPositions[i] + new Vector2(0, -dropAmount)));
            }
            else
            {
                stationButtons[i].anchoredPosition = originalPositions[i];
            }
        }
    }

    private IEnumerator AnimateButton(int i, Vector2 targetPos)
    {
        RectTransform button = stationButtons[i];
        Vector2 startPos = button.anchoredPosition;
        float t = 0f;

        while (t < animationTime)
        {
            t += Time.deltaTime;
            float progress = t / animationTime;
            button.anchoredPosition = Vector2.Lerp(startPos, targetPos, progress);
            yield return null;
        }

        button.anchoredPosition = targetPos;
    }
}
