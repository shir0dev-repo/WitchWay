using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private string savePath => Application.persistentDataPath + "/playerSave.json";
    public SaveData saveData = new();

    public static SaveManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void CollectIngredient(string id)
    {
        if (!saveData.collectedIngredients.Contains(id))
        {
            saveData.collectedIngredients.Add(id);
            // SaveGame();
        }
    }
    public void CollectBottle(string id)
    {
        if (!saveData.collectedBottles.Contains(id))
        {
            saveData.collectedBottles.Add(id);
            //  SaveGame();
        }
    }
    public void LearnRecipe(string id)
    {
        if (!saveData.learnedRecipes.Contains(id))
        {
            saveData.learnedRecipes.Add(id);
            SaveGame();
        }
    }

    public bool hasIngredient(string id) => saveData.collectedIngredients.Contains(id);
    public bool hasBottle(string id) => saveData.collectedBottles.Contains(id);
    public bool hasRecipe(string id) => saveData.learnedRecipes.Contains(id);

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
    }
    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
    }
    /*public void ResetSave()       // if we wanted to be able to reset what the player has collected, might be needed for testing as well so you dont have to go through and delete json files
    {
        saveData = new SaveData();
        SaveGame();
    }*/

    void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }
}