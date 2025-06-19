using System;
using UnityEngine;

public class CuttingBoard : Singleton<CuttingBoard>
{
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
    public void ChangeCuttingAbility()
    { // changed this into function so it can be called in other scripts
        CanCut = !CanCut;
    }
    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            if (collision.gameObject.tag == "Ingredient")
            {
                string name = collision.gameObject.name;
                IngredientSO z = collision.gameObject.GetComponent<WorldIngredient>().BaseIngredient;
                if (z.CanBeCut == false) return;
                GameObject cutPF = z.CutWorldPrefab;
                if (cutPF != null)
                {
                    GameObject p = Instantiate(cutPF);
                    p.transform.position = new Vector3(0, 1, 0);
                    p.transform.parent = transform;
                    p.name = name;

                    CuttableIngredient ig = p.GetComponent<CuttableIngredient>();
                    // to make sure this works, the ingredient dropped has to have the
                    // same name as the prefab it's referring too!

                    //later, this will just ask for the name of the scriptable object
                    Destroy(collision.gameObject);
                    if (CursorManager.Instance != null)
                        CursorManager.Instance.ClearCursor(false);
                }
            }
        }
        catch
        {
            Debug.Break();
        }
    }
}
