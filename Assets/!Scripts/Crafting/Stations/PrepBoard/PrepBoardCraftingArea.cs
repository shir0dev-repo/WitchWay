using UnityEngine;

public class PrepBoardCraftingArea : Singleton<PrepBoardCraftingArea>
{
    [SerializeField] private GameObject _mortarOutline;
    [SerializeField] private LayerMask _mortarMask;
    [SerializeField] private Vector3 _mortarActivePosition = Vector3.zero;

    private MortarStation _mortar;
    private CuttingBoard _cuttingBoard;

    private void Start()
    {
        _mortar = MortarStation.Instance;
        _cuttingBoard = CuttingBoard.Instance;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CursorManager.CastScreenRay(Input.mousePosition, out RaycastHit hit))//, _mortarMask)) {
            {
                if (hit.transform == _mortar.transform && !_mortar.enabled)
                    EnableMortar();
                else if (_mortar.enabled && !_mortar.HasIngredient && hit.transform == _mortarOutline.transform)
                    DisableMortar();
            }
        }

        if (_mortar.enabled)
        {
            if (ToolSelector.Instance != null && ToolSelector.Instance.CurrentlySelected == null)
            {
                if (Input.GetMouseButtonDown(1) && CursorManager.CastScreenRay(Input.mousePosition, out RaycastHit hit, _mortarMask))
                {
                    if (hit.transform == _mortar.transform) DisableMortar();
                }
            }
        }
    }

    public void EnableMortar()
    {
        _cuttingBoard.enabled = false;
        _cuttingBoard.GetComponent<Collider>().isTrigger = true;

        _mortar.transform.position = _mortarActivePosition;
        _mortar.enabled = true;
        _mortarOutline.SetActive(true);

    }

    public void DisableMortar()
    {
        _mortarOutline.SetActive(false);
        _mortar.transform.position = _mortarOutline.transform.position;
        _mortar.enabled = false;

        _cuttingBoard.enabled = true;
        _cuttingBoard.GetComponent<Collider>().isTrigger = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(_mortarActivePosition, 0.15f);
    }
}
