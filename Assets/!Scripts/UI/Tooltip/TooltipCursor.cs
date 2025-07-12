using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipCursor : Singleton<TooltipCursor>
{
    [SerializeField] private GameObject _uiParent;
    [SerializeField] private TextMeshProUGUI _hoveredItemName;
    [SerializeField] private Image _hoveredSprite;
    public StationsInvBoxItem HoveredItem { get; set; } = null;
    
    private bool _shouldFollowCursor = false;

    protected override void Awake()
    {
        base.Awake();
        _uiParent.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.Crafting.OnIngredientUIHover += OnHoverOverUIItem;
        GameEvents.Crafting.OnIngredientUIUnhovered += OnUIItemUnhovered;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnIngredientUIHover -= OnHoverOverUIItem;
        GameEvents.Crafting.OnIngredientUIUnhovered -= OnUIItemUnhovered;
    }

    private void Update()
    {
        if (_shouldFollowCursor)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnHoverOverUIItem(ModifiedIngredient ingredient)
    {
        if (CursorManager.Instance != null && CursorManager.Instance.HasObjectFollowingCursor) return;

        _hoveredSprite.sprite = ingredient.GetUIRepresentation();
        _hoveredItemName.text = ingredient.GetStringRepresentation();
        _shouldFollowCursor = true;
        _uiParent.SetActive(true);
    }

    public void OnUIItemUnhovered()
    {
        _uiParent.SetActive(false);
        _shouldFollowCursor = false;
        _hoveredSprite.sprite = null;
        _hoveredItemName.text = string.Empty;
    }
}
