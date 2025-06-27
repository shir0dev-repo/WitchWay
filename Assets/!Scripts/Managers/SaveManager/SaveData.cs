using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public List<string> collectedIngredients = new();
    public List<string> collectedBottles = new();
    public List<string> learnedRecipes = new();
}
