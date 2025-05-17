using System;
using UnityEngine;

public class CuttingBoard : MonoBehaviour
{
    public static CuttingBoard Instance { get; private set; }
    public CuttableIngredientList list {  get; private set; }

    public bool CanCut = false;
    public Action OnCutComplete;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
    }
    void Start()
    {
        list = GetComponentInChildren<CuttableIngredientList>();
        // the ingredient list is part of the cutting board object!
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
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ingredient")
        {
            string name = collision.gameObject.name;

            GameObject p = Instantiate(list.GetPrefab(name));
            p.transform.position = new Vector3(0, 1, 0);
            // to make sure this works, the ingredient dropped has to have the
            // same name as the prefab it's referring too!
            Destroy(collision.gameObject);
        }
    }
}
