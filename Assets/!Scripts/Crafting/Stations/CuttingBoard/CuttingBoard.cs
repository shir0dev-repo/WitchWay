using System;
using UnityEngine;

public class CuttingBoard : Singleton<CuttingBoard>
{
    public CuttableIngredientList list {  get; private set; }

    public bool CanCut = false;
    public Action OnCutComplete;

    private void OnEnable()
    {
        GameEvents.Crafting.OnToolSelected += Enable;
        GameEvents.Crafting.OnToolDeselected += Disable;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnToolSelected -= Enable;
        GameEvents.Crafting.OnToolDeselected -= Disable;
    }

    private void Enable(ToolType type)
    {
        if (type == ToolType.Knife)
            CanCut = true;
    }

    private void Disable(ToolType type)
    {
        if (type == ToolType.Knife)
            CanCut = false;
    }

    void Start()
    {
        list = GetComponentInChildren<CuttableIngredientList>();
        // the ingredient list is part of the cutting board object!
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
            if (list.GetPrefab(name.ToLower()) != null)
            {
                GameObject p = Instantiate(list.GetPrefab(name.ToLower()));
                p.transform.position = new Vector3(0, 1, 0);
                p.transform.parent = transform;
                p.name = name;
                // to make sure this works, the ingredient dropped has to have the
                // same name as the prefab it's referring too!

                //later, this will just ask for the name of the scriptable object
                CursorManager.Instance.ClearCursor();
                Destroy(collision.gameObject);
            }
            else { return; } // if there's an exception, the thing will return.
        }
    }
}
