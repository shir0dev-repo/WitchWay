using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CauldronMaster : Singleton<CauldronMaster>
{
    [Header("References")]
    [SerializeField] CauldronController _controller;
    [SerializeField] private CauldronVisuals _visuals;

    [Space]
    [SerializeField] List<WorldIngredient> _ingredients = new();
    private readonly List<WorldIngredient> _ingredientsToAdd = new();

    [Header("Visuals")]
    [SerializeField] private float _ingredientCorrectionDuration = 0.2f;
    [SerializeField] private float _ingredientFlightDuration = 0.6f;
    [SerializeField] private Transform _ingredientAddStartPoint;
    [SerializeField] private Transform _ingredientAddApexPoint;

    private PotionData _targetPotion = null;

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

        _targetPotion = null;
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
        }

        if (_visuals != null)
            _visuals.SetTargetPropertyBlock(recipe.CauldronEffects);
        GameEvents.Crafting.OnMixedPotionRequested?.Invoke(result);
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
    }

    private void TryAddIngredient(IFollowCursor cursor)
    {
        bool wasAdded = false;
        Transform targetTransform = null;
        Debug.Log(cursor.GetType().FullName);
        if (cursor is WorldIngredient wg || cursor is Transform t && t.TryGetComponent(out wg))
        {
            Debug.Log("bello");
            _ingredients.Add(wg);
            wasAdded = true;
            targetTransform = wg.transform;
        }
        
        else if (cursor is IngredientSegment segment)
        {
            Debug.Log(" b e ll o");
            wg = segment.GetComponentInParent<WorldIngredient>();
            if (wg != null)
            {
                _ingredients.Add(wg);
                foreach (IngredientSegment s in segment.GrabSimilar(segment.transform.parent))
                {
                    if (s.TryGetComponent(out Rigidbody rb))
                    {
                        rb.isKinematic = true;
                    }
                }

                wasAdded = true;
                targetTransform = wg.transform;
            }
        }

        if (wasAdded)
        {
            StartCoroutine(IngredientFlairCoroutine(targetTransform));
        }

        GameEvents.Crafting.OnObjectRemovedFromCursor -= TryAddIngredient;
    }

    private IEnumerator IngredientFlairCoroutine(Transform targetIngredient)
    {
        if (targetIngredient == null) yield break;

        float progress = 0;
        float timer = 0;
        float inv_duration = 1.0f / _ingredientCorrectionDuration;
        Vector3 startPosition = targetIngredient.position;
        Vector3 endPosition = _ingredientAddStartPoint.position;
        Rigidbody[] rbs = null;
        if (targetIngredient.TryGetComponent(out WorldIngredient wg))
        {
            rbs = wg.Rigidbodies;
        }
        else
        {
            rbs = targetIngredient.GetComponentsInChildren<Rigidbody>();
        }
        
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
        }

        while (progress < 1.0f)
        {
            progress = timer * inv_duration;
            timer += Time.deltaTime;
            targetIngredient.position = Vector3.Lerp(startPosition, endPosition, progress);
            yield return new WaitForEndOfFrame();
        }

        progress = timer = 0.0f;

        targetIngredient.position = endPosition;
        startPosition = targetIngredient.position;
        endPosition = _ingredientAddApexPoint.position;
        inv_duration = 1.0f / _ingredientFlightDuration * 0.75f;

        while (progress < 1.0f)
        {
            progress = timer * inv_duration;
            timer += Time.deltaTime;
            targetIngredient.position = Vector3.Lerp(startPosition, endPosition, EaseOutExpo(progress));
            yield return new WaitForEndOfFrame();
        }

        progress = timer = 0.0f;
        inv_duration = 1.0f / _ingredientFlightDuration * 0.25f;
        endPosition = transform.position;

        Sequence seq = DOTween.Sequence();
        
        if (rbs != null)
        {
            foreach (var rb in rbs)
            {
                seq.Append(rb.DOMove(endPosition, _ingredientFlightDuration * 0.25f));
            }
        }
        else
        {
            seq.Append(targetIngredient.transform.DOMove(endPosition, _ingredientFlightDuration * 0.25f));
        }

        seq.Join(targetIngredient.transform.DOScale(0.0f, _ingredientFlightDuration * 0.25f))
            .onComplete += () => targetIngredient.gameObject.SetActive(false);
        seq.Play();
    }

    private static float EaseOutExpo(float x)
    {
        return Mathf.Approximately(x, 1.0f) ? 1 : 1 - Mathf.Pow(2, -10 * x);
    }

    private void OnTriggerEnter(Collider other)
    {
        WorldIngredient ing = other.GetComponentInParent<WorldIngredient>();
        if (ing != null)
        {
            if (!_ingredientsToAdd.Contains(ing) && !_ingredients.Contains(ing))
            {
                _ingredientsToAdd.Add(ing);
                GameEvents.Crafting.OnObjectRemovedFromCursor += TryAddIngredient;
                GameEvents.Crafting.OnItemPlacedInCauldron?.Invoke(ing);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out WorldIngredient ing)) return;
        if (_ingredientsToAdd.Contains(ing))
        {
            _ingredients.Remove(ing);
            _ingredientsToAdd.Remove(ing);
            GameEvents.Crafting.OnObjectRemovedFromCursor -= TryAddIngredient;
        }
    }
}
