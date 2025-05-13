using TMPro;
using UnityEngine;

public class PuzzleHandler : MonoBehaviour
{
    public GradientSortedRecipePuzzle Puzzle = new();
    [SerializeField] private TextMeshProUGUI _sortedTextUGUI;

    private void Update()
    {
        _sortedTextUGUI.text = "Sorted: " + Puzzle.IsSorted().ToString();
    }
}
