using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public RectTransform TooltipTransform;
    public Transform TooltipParent;
    public Vector2 Offset;

    public GraphicRaycaster Raycaster;
    public EventSystem EventSystem;

    private IHasTooltip _currentTooltip;

    void Update()
    {
        UpdateTooltip();

        Vector2 flip = new Vector2();
        Vector2 pos = Input.mousePosition;
        Rect rect = TooltipTransform.rect;

        if (pos.x + rect.width > Screen.width)
        {
            flip.x = -rect.width + Offset.x * -2;
        }
        if (pos.y - rect.height < 0f)
        {
            flip.y = rect.height + Offset.y * 2;
        }

        TooltipTransform.position = pos + Offset + flip;
    }

    private void UpdateTooltip ()
    {
        PointerEventData data = new PointerEventData(EventSystem);
        data.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        bool changed = false;
        bool any = false;

        Raycaster.Raycast(data, results);

        foreach (RaycastResult result in results)
        {
            IHasTooltip tooltip = result.gameObject.GetComponent<IHasTooltip>();
            if (tooltip != null)
            {
                any = true;
                if (tooltip != _currentTooltip)
                {
                    changed = true;
                    ClearTooltip();
                }
                _currentTooltip = tooltip;
            }
        }

        if (any == false)
        {
            _currentTooltip = null;
        }

        if (_currentTooltip == null)
        {
            ClearTooltip();
        }
        else if (changed)
        {
            GameObject newTooltip = _currentTooltip.InstantiateTooltip();
            newTooltip.transform.SetParent(TooltipParent);
        }
    }

    private void ClearTooltip ()
    {
        foreach (Transform child in TooltipParent)
        {
            Destroy(child.gameObject);
        }
    }
}
