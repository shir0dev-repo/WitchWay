using UnityEngine;

[System.Serializable]
public class DepthChangeSections
{
    public Rect screenRect;
    public float depthValue;
}

[CreateAssetMenu(fileName = "DepthSectionsConfig", menuName = "DepthSectionsConfig")]
public class DepthSectionsConfig : ScriptableObject
{
    public DepthChangeSections[] screenSections;
}
