using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class SelectionBoxVisual : MonoBehaviour {

    public Transform[] CornerPieces;
    public Bounds MinBounds;
    public GameObject CurrentTarget;
    public Bounds CurrentTargetBounds;

    private void LateUpdate()
    {
        if (CurrentTarget != null)
        {
            transform.position = CurrentTarget.transform.position + CurrentTargetBounds.center;
            transform.rotation = CurrentTarget.transform.rotation;
        }
    }

    public void Assign(GameObject go)
    {
        CurrentTargetBounds = UnityUtils.ComputeObjectColliderBounds(go);
        MinBounds.center = CurrentTargetBounds.center;
        CurrentTargetBounds.Encapsulate(MinBounds);
        Vector3[] corners = ComputeBoundsCorner(CurrentTargetBounds).ToArray();
        for (int i = 0; i < corners.Length;  i++)
        {
            CornerPieces[i].transform.localPosition = corners[i];
            CornerPieces[i].transform.position = new Vector3(
                CornerPieces[i].transform.position.x,
                Mathf.Max(CornerPieces[i].transform.position.y, 0),
                CornerPieces[i].transform.position.z);
        }
        CurrentTarget = go;
        CurrentTargetBounds.center -= go.transform.position;
    }

    private IEnumerable<Vector3> ComputeBoundsCorner(Bounds bounds)
    {
        yield return new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
        yield return new Vector3(-bounds.extents.x, -bounds.extents.y, +bounds.extents.z);
        yield return new Vector3(-bounds.extents.x, +bounds.extents.y, -bounds.extents.z);
        yield return new Vector3(-bounds.extents.x, +bounds.extents.y, +bounds.extents.z);
        yield return new Vector3(+bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
        yield return new Vector3(+bounds.extents.x, -bounds.extents.y, +bounds.extents.z);
        yield return new Vector3(+bounds.extents.x, +bounds.extents.y, -bounds.extents.z);
        yield return new Vector3(+bounds.extents.x, +bounds.extents.y, +bounds.extents.z);
    }
}
