using System.Collections.Generic;
using UnityEngine;

public class ChanceIngredientSpawn : MonoBehaviour
{
    [SerializeField] private List<ChanceIngredient> _possibleIngredients = new();
    private GameObject _ingredientObj;

    private void Awake()
    {
        for (int i = 0; i < _possibleIngredients.Count; i++)
        {
            int rand = Random.Range(0, 100);

            if (rand <= _possibleIngredients[i].Chance)
            {
                _ingredientObj = _possibleIngredients[i].Ingredient.WorldPrefab;
            }
        }

        if (_ingredientObj != null)
            Instantiate(_ingredientObj, transform);
    }
}
