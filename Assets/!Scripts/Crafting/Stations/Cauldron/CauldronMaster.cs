using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CauldronMaster : Singleton<CauldronMaster>
{
    [SerializeField] CauldronController _controller;
    [SerializeField] List<WorldIngredient> _ingredients = new();

    private PotionData _targetPotion = null;

    public IngredientsInPot InsidePot { get; private set; }
    public CauldronEvents CauldronEvents { get; private set; }
    public CauldronMixingDuration Duration { get; private set; }

    public bool CurrentlyMixing { get; private set; } = false;

    private void OnEnable()
    {
        GameEvents.Crafting.OnToolSelected += BeginMixing;
        GameEvents.Crafting.OnToolDeselected += FinishMixing;
        GameEvents.Crafting.OnCauldronMixSequenceCompleted += FinishMixing;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnToolSelected -= BeginMixing;
        GameEvents.Crafting.OnToolDeselected -= FinishMixing;
        GameEvents.Crafting.OnCauldronMixSequenceCompleted -= FinishMixing;
    }

    protected override void Awake()
    {
        base.Awake();

        InsidePot = GetComponentInChildren<IngredientsInPot>();
        CauldronEvents = GetComponentInChildren<CauldronEvents>();
        Duration = GetComponentInChildren<CauldronMixingDuration>();
        if (_controller == null)
            _controller = FindFirstObjectByType<CauldronController>(FindObjectsInactive.Include);
    }

    void BeginMixing(ToolType tool)
    {
        if (tool != ToolType.Spoon) return;

        _targetPotion = FindTargetPotion();
        CurrentlyMixing = true;
        CameraManager.Instance.ZoomIn(40);
        _controller.gameObject.SetActive(true);
    }

    void FinishMixing()
    {
        CurrentlyMixing = false;
        if (ToolSelector.Instance != null)
        {
            ToolSelector.Instance.DeselectTool();
        }
    }

    void FinishMixing(ToolType tool)
    {
        if (tool != ToolType.Spoon) return;

        CurrentlyMixing = false;
        _controller.gameObject.SetActive(false);
        List<ModifiedIngredient> comps = _ingredients.Select(wg => wg.ModifiedState).ToList();
        
        FinalizeOutput();
        CameraManager.Instance.ResetZoom();
    }

    private bool WasMixingCompleted()
    {
        return false;
    }

    public PotionData FindTargetPotion()
    {
        PotionData result = null;
        RecipeSO recipe = RecipeBook.Instance.list.GetFirstRecipeFromListofMultiple(_ingredients);

        if (recipe == null) return result;

        Debug.Log("the closest recipe to the ingredients in the pot is " + recipe.ToString());

        if (recipe.IsValidRecipe(_ingredients.Select(ing => ing.ModifiedState).ToList()))
        {
            if (recipe.IsDiscovered)
            {
                result = recipe.Output;
                Debug.Log("win epic!" + '\n' + "the outputted potion is: " + result.ToString());
            }
            else
            {
                result = RecipeBook.Instance.MysteriousPotion;
                Debug.Log("you haven't discovered this recipe yet!" + '\n' + "the outputted potion is: " + result.ToString());
            }

            
        }
        else
        {
            Debug.Log("NOOOOOOOOOOOOOOOOOOO");
            CauldronMaster.Instance.InsidePot.ReturnRejectedIngredients();
        }

        return result;
    }

    private void FinalizeOutput()
    {
        if (WasMixingCompleted())
        {
            foreach (WorldIngredient ing in _ingredients)
            {
                Destroy(ing.gameObject);
            }

            _ingredients.Clear();
        }
        else
        {
            Debug.Log("Potion completed!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out WorldIngredient ing)) return;

        _ingredients.Add(ing);
        GameEvents.Crafting.OnItemPlacedInCauldron?.Invoke(ing);
        other.gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out WorldIngredient ing)) return;

        _ingredients.Remove(ing);
    }
}
