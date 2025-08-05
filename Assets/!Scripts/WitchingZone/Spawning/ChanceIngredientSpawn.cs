using System.Collections.Generic;
using UnityEngine;

public class ChanceIngredientSpawn : MonoBehaviour
{
    [SerializeField] private List<ChanceIngredient> _possibleIngredients = new();
    private GameObject _ingredientObj;

    private void Awake()
    {
        float scale = 1.0f;

        for (int i = 0; i < _possibleIngredients.Count; i++)
        {
            int rand = Random.Range(0, 100);

            if (rand <= _possibleIngredients[i].Chance)
            {
                _ingredientObj = _possibleIngredients[i].Ingredient.WorldPrefab;
                scale = _possibleIngredients[i].ObjectScale;
                break;
            }
        }

        if (_ingredientObj != null)
        {
            GameObject go = Instantiate(_ingredientObj, transform);
            go.transform.localScale = Vector3.one * scale;
        }
    }
}
