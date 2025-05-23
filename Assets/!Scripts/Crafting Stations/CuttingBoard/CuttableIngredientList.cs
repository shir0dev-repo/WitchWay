using System.Collections.Generic;
using UnityEngine;

public class CuttableIngredientList : MonoBehaviour
{
    [SerializeField]
    List<GameObject> ListOfPreCutPrefabs = new List<GameObject>();
    [SerializeField]
    List<GameObject> ListOfCutPrefabs = new List<GameObject>();
    // drag and drop prefabs into the inspector

    public Dictionary<string, GameObject> PreCutIngredientListWithNames = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> CutIngredientListWithNames = new Dictionary<string, GameObject>();
    
    void Start()
    {
        foreach (var item in ListOfPreCutPrefabs)
        {
            PreCutIngredientListWithNames.Add(item.name.ToLower(), item);
            // when you call this dictionary, just get the name of the prefab you want
        }
        foreach (var item in ListOfCutPrefabs)
        {
            CutIngredientListWithNames.Add(item.name.ToLower(), item);
        }
    }

    public GameObject GetPrefab(string name)
    {
        if (PreCutIngredientListWithNames.TryGetValue(name, out GameObject prefab))
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
    public GameObject GetChoppedPrefab(string name)
    {
        if (CutIngredientListWithNames.TryGetValue(name, out GameObject prefab))
        {
            return prefab;
        }
        else
        {
            Debug.LogWarning($"Prefab with name '{name}' not found in the dictionary!");
            return null;
        }
    }
}
