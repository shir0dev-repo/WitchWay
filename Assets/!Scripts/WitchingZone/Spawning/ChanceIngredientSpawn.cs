using System.Collections.Generic;
using UnityEngine;

public class ChanceIngredientSpawn : MonoBehaviour
{
    [SerializeField] private List<ChanceIngredient> _possibleIngredients = new();
    private GameObject _ingredientObj;

    private void Start()
    {
        float scale = 1.0f;
        IngredientSO selectedIngredient = null;

        for (int i = 0; i < _possibleIngredients.Count; i++)
        {
            int rand = Random.Range(0, 100);

            if (rand <= _possibleIngredients[i].Chance)
            {
                selectedIngredient = _possibleIngredients[i].Ingredient;
                _ingredientObj = selectedIngredient.WorldPrefab;
                scale = _possibleIngredients[i].ObjectScale;
                break;
            }
        }

        if (_ingredientObj != null)
        {
            GameObject go = Instantiate(_ingredientObj, transform);
            if (go.TryGetComponent(out WZWorldIngredient ing))
                ing.ingredient = selectedIngredient;
            else
                go.AddComponent<WZWorldIngredient>().ingredient = selectedIngredient;

            go.transform.localScale = Vector3.one * scale;
            GameObject room = WitchingZoneGenerator.Instance.GetRoom(go.transform.position).gameObject;
            go.transform.SetParent(room.transform);
            go.tag = "Ingredient";
        }
    }
}
