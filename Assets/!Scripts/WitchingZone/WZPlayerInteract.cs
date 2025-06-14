using UnityEngine;
using UnityEngine.InputSystem;

public class WZPlayerInteract : MonoBehaviour
{
    [Header("Interact Controls")]
    [SerializeField] private InputAction interactAction;
    [Space(5)]
    [SerializeField] private InputAction dragAction;

    [Header("Dialogue Controls")]
    [SerializeField] private InputAction optionChangeAction; //wasd and arrow keys
    [Space(5)]
    [SerializeField] private InputAction selectAction;

    [Header("Misc Controls")]
    [SerializeField] private InputAction showIngrediantsAction;
    [Space(5)]
    [SerializeField] private InputAction recipeBookAction;
    [Space(5)]
    [SerializeField] private InputAction pauseAction;
}
