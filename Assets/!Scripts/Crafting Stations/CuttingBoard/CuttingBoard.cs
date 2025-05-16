using System;
using UnityEngine;

public class CuttingBoard : MonoBehaviour
{
    public static CuttingBoard Instance { get; private set; }

    public bool CanCut = false;
    public Action OnCutComplete;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 250, 50, 150, 25), "Toggle Cutmode"))
        {
            ChangeCuttingAbility();
        }
    }
    public void ChangeCuttingAbility() 
    { // changed this into function so it can be called in other scripts
        CanCut = !CanCut;
    }
}
