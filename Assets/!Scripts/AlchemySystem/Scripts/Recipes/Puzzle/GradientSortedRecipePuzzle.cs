using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GradientSortedRecipePuzzle : RecipePuzzle
{
    public override SortingMode SortingMode => SortingMode.Gradient;
    private Dictionary<Transform, Color> _colorLookup = null;

    public override bool IsSorted()
    {

        if (_colorLookup == null) InitColorLookup();
        if (_colorLookup.Count < 2) return false;

        _ingredientTransforms = _ingredientTransforms.OrderBy(t => t.position.x).ToArray();

        List<float> hues = _ingredientTransforms.Select(t =>
        {
            Color c = _colorLookup[t];
            Color.RGBToHSV(c, out float h, out _, out _);
            return h;
        }).ToList();

        float firstH = hues[0];

        List<float> normalized = hues.Select(h => (h - firstH + 1) % 1).ToList();
        bool increasing = true, decreasing = true;

        for (int i = 1; i < normalized.Count; i++)
        {
            if (normalized[i] < normalized[i - 1])
            {
                increasing = false;
                break;
            }
        }

        for (int i = 1; i < normalized.Count; i++)
        {
            if (normalized[i] > normalized[i - 1])
            {
                decreasing = false;
                break;
            }
        }

        return increasing || decreasing;
    }

    private void InitColorLookup()
    {
        _colorLookup = new Dictionary<Transform, Color>();

        for (int i = 0; i < _ingredientTransforms.Length; i++)
        {
            if (_ingredientTransforms[i].TryGetComponent(out Renderer r))
            {
                _colorLookup.Add(_ingredientTransforms[i], r.material.color);
            }
            else
            {
                Debug.LogError("fuck you");
                throw new InvalidOperationException("fuck you again");
            }
        }
    }
}
