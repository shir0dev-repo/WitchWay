using UnityEngine;

/// <summary>
///     Holds a Transform and its corresponding snapping point.
/// </summary>
[System.Serializable]
public class SnappableObject 
{
    [Tooltip("The object being snapped to the snap point.")]
    public Transform Object;

    [Tooltip("The point the object will snap to.")]
    public Transform[] SnapPoints;

    [Space]

    [Tooltip("The offset applied when snapping the object to the snap point.")]
    public Vector3 SnapOffset;

    [Tooltip("The minimum distance required to snap the object to the snap point.")]
    public float SnapDistance;

    /// <summary>
    ///     The closest Transform to <see cref="Object"/> within <see cref="SnapPoints"/>.
    /// </summary>
    /// <remarks>
    ///     This is set from within the <see cref="IsWithinSnapRange"/> method.
    /// </remarks>
    public Transform ClosestSnapPoint { get; private set; } = null;

    /// <summary>
    ///     Calculates the distance between the <see cref="Object"/> and each of its <see cref="SnapPoints"/>.
    /// </summary>
    /// <remarks>
    ///     This method is called within <see cref="SnapToPoint(bool)"/>, when <paramref name="ignoreDistance"/> is false.
    /// </remarks>
    /// <returns>
    ///     <see langword="true"/> when the distance between <see cref="Object"/> and <see cref="SnapPoints"/> + <see cref="SnapOffset"/><br/>
    ///     is less than or equal to <see cref="SnapDistance"/>.
    /// </returns>
    public bool IsWithinSnapRange()
    {
        ClosestSnapPoint = FindClosestSnapPoint(out float sqrDistance);
        if (Mathf.Approximately(sqrDistance, float.MaxValue))
        {
            Debug.LogWarning("FindClosestSnapPoint returned a distance of approx. 3.4e38, which probably means there are no snap points assigned!");
            return false;
        }

        return sqrDistance <= SnapDistance * SnapDistance;
    }

    /// <summary>
    ///     Finds the closest Transform in <see cref="SnapPoints"/> to <see cref="Object"/>.
    /// </summary>
    /// <remarks>
    ///     Since <paramref name="sqrDistance"/> may return <see cref="float.MaxValue"/>,
    ///     it is vital to use <see cref="Mathf.Approximately(float, float)"/> to ensure floating point errors are avoided.
    /// </remarks>
    /// <param name="sqrDistance">
    ///     The squared distance of the snap point to the object. If no snap points are checked, this will return <see cref="float.MaxValue"/>.
    /// </param>
    /// <returns>
    ///     The closest Transform within <see cref="SnapPoints"/> to <see cref="Object"/>.<br/>
    ///     Will return null if <see cref="SnapPoints"/>.Length is 0.
    /// </returns>
    private Transform FindClosestSnapPoint(out float sqrDistance)
    {
        Transform closest = null;
        sqrDistance = float.MaxValue;
        foreach (Transform snap in SnapPoints)
        {
            float d = (snap.position + SnapOffset - Object.position).sqrMagnitude;
            if (d < sqrDistance)
            {
                sqrDistance = d;
                closest = snap;
            }
        }

        return closest;
    }

    /// <summary>
    ///     Snaps the <see cref="Object"/> to <see cref="SnapPoints"/>.
    /// </summary>
    /// <remarks>
    ///     If <paramref name="ignoreDistance"/> is true, the object will snap no matter the distance to the snap point.
    /// </remarks>
    /// <param name="ignoreDistance">
    ///     Whether or not to ignore the distance between the object and its snap point.
    /// </param>
    public void SnapToPoint(bool ignoreDistance = false)
    {
        if (ignoreDistance || IsWithinSnapRange())
            Object.position = ClosestSnapPoint.position + SnapOffset;
    }
}
