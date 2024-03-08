using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UICameraViewport : MonoBehaviour
{
    public RectTransform Viewport;
    public RawImage UnitViewImage;
    public RenderTexture RenderTexture;
    public Camera Camera;
    public Camera UICamera;

    public void SetRenderTexture(RenderTexture newRenderTexture)
    {
        RenderTexture = newRenderTexture;
        StartCoroutine(SyncTextureSizeToViewport(newRenderTexture));

    }

    private IEnumerator SyncTextureSizeToViewport(RenderTexture newRenderTexture)
    {
        yield return null;
        RenderTexture.width = (int)Viewport.rect.size.x;
        RenderTexture.height = (int)Viewport.rect.size.y;
        Camera.targetTexture = RenderTexture;
        UnitViewImage.texture = RenderTexture;
        Camera.targetTexture = RenderTexture;
        UnitViewImage.texture = newRenderTexture;
    }

    public Vector3 ScreenPointToViewportPoint(Vector3 point)
    {
        Vector3[] corners = new Vector3[4];
        Viewport.GetWorldCorners(corners);
        corners.Select(x => UICamera.WorldToScreenPoint(x)).ToArray();
        Rect rect = new Rect(
            corners[0].x,
            corners[0].y,
            corners[2].x - corners[0].x,
            corners[2].y - corners[0].y
            );

        Vector3 viewportPoint = new Vector3(
            Mathf.InverseLerp(rect.xMin, rect.xMax, point.x),
            Mathf.InverseLerp(rect.yMin, rect.yMax, point.y));
        return viewportPoint;
    }

    public Ray ScreenPointToRay(Vector3 point)
        => Camera.ViewportPointToRay(ScreenPointToViewportPoint(point));
}
