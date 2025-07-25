using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SaveManagerEditorWindow : EditorWindow
{
    SaveData saveData;

    [MenuItem("Window/Save Data Editor")]
    public static void ShowWindow()
    {
        GetWindow<SaveManagerEditorWindow>("Save Data Editor");
    }

    private void OnEnable()
    {
        LoadSaveData();
    }

    private void OnGUI()
    {
        if (saveData == null)
        {
            EditorGUILayout.HelpBox("Save data not loaded.", MessageType.Warning);
            if (GUILayout.Button("Reload"))
                LoadSaveData();
            return;
        }

        EditorGUILayout.LabelField("Edit Save Data", EditorStyles.boldLabel);

        CreateListLayout("Ingredients", saveData.collectedIngredients);
        CreateListLayout("Bottles", saveData.collectedBottles);
        CreateListLayout("Recipes", saveData.learnedRecipes);

        if (GUILayout.Button("Save"))
            SaveToFile();
    }

    public override void SaveChanges()
    {
        SaveToFile();
        base.SaveChanges();
    }

    private void LoadSaveData()
    {
        string filePath = SaveManager.SavePath;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
    }

    private void CreateListLayout(string label, in List<string> list)
    {
        EditorGUILayout.LabelField(label + ':');
        for (int i = 0; i < list.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            list[i] = EditorGUILayout.TextField($"Item {i + 1}", list[i]);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                list.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Entry"))
            list.Add("");

        EditorGUILayout.Space();
    }

    private void SaveToFile()
    {
        if (saveData == null) return;
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SaveManager.SavePath, json);
    }
}
