using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipCursor : Singleton<TooltipCursor>
{
    [SerializeField] private GameObject _uiParent;
    [SerializeField] private TextMeshProUGUI _hoveredItemName;
    [SerializeField] private Image _hoveredSprite;
    private bool _shouldFollowCursor = false;

    protected override void Awake()
    {
        base.Awake();
        _uiParent.SetActive(false);
    }

    private void Update()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity))
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.red);
            if (hit.collider)
            {
                Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.TryGetComponent(out StationsInvBoxItem item))
                {
                    OnHoverOverUIItem(item.Ingredient);
                }
            }
        }
        else
        {
            OnUIItemUnhovered();
        }

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
