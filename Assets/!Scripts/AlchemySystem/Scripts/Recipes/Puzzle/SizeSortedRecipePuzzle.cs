using System.Linq;
using UnityEngine;

[System.Serializable]
public class SizeSortedRecipePuzzle : RecipePuzzle
{
    public override SortingMode SortingMode => SortingMode.Size;
    
    public override bool IsSorted()
    {
        _ingredientTransforms = _ingredientTransforms.OrderBy(t => t.position.x).ToArray();
        Transform largest = _ingredientTransforms[0];
        
        for (int i = 1; i < _ingredientTransforms.Length; i++)
        {
            if (_ingredientTransforms[i].localScale.sqrMagnitude > largest.localScale.sqrMagnitude)
            {
                return false;
            }
            else if (Vector3.Distance(largest.position, _ingredientTransforms[i].position) > _distanceThreshold)
            {
                return false;
            }
            else
            {
                largest = _ingredientTransforms[i];
            }
        }

        return true;
    }
}
