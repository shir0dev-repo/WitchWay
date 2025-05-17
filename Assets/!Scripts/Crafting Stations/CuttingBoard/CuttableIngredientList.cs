using System.Collections.Generic;
using UnityEngine;

public class CuttableIngredientList : MonoBehaviour
{
    [SerializeField]
    List<GameObject> ListOfPrefabs = new List<GameObject>();
    // drag and drop prefabs into the inspector

    public Dictionary<string, GameObject> CutIngredientListWithNames = new Dictionary<string, GameObject>();
    
    void Start()
    {
         foreach (var item in ListOfPrefabs)
        {
            CutIngredientListWithNames.Add(item.name, item);
            // when you call this dictionary, just get the name of the prefab you want
        }
    }

    public GameObject GetPrefab(string name)
    {
        if (CutIngredientListWithNames.TryGetValue(name, out GameObject prefab))
        {
            return prefab;
            // now uses trygetvalue() to catch exceptions
        }
        else
        {
            Debug.LogWarning($"Prefab with name '{name}' not found in the dictionary!");
            return null;
        }
    }
}
