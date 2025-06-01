using UnityEngine;

public class BottleDetection : MonoBehaviour
{
    public delegate void StartMinigame();
    public static StartMinigame bottlePlaced;

    public delegate void FinishMinigame();
    public static FinishMinigame filledBottle;

    Siphon siphon;
    BottleLevel bottle;

    [SerializeField] GameObject prefab;

    void OnEnable()
    {
        bottlePlaced += startMiniGame;
        filledBottle += InstantiateFinishedPotion;
    }
    void OnDisable()
    {
        bottlePlaced -= startMiniGame;
        filledBottle -= InstantiateFinishedPotion;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EmptyBottle bottle))
        {
            bottlePlaced?.Invoke();
            Destroy(other.gameObject);
            // when there's more animations we can replace this destroy
        }
    }

    void startMiniGame()
    {
        siphon = GetComponentInChildren<Siphon>();
        bottle = GetComponentInChildren<BottleLevel>();

        siphon.enabled = true;
        bottle.enabled = true;
    }

    void InstantiateFinishedPotion()
    {
        GameObject p = Instantiate(prefab);
        p.transform.position = Vector3.zero;
        p.transform.localScale = new Vector3(3,3,3);

        // placeholder stuff, i'd like to be able to instantiate normally
    }
}
