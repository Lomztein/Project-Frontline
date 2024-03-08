using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public RectTransform TooltipTransform;
    public Transform TooltipParent;
    public Vector2 Offset;

    public GraphicRaycaster Raycaster;
    public EventSystem EventSystem;
    public PlayerHandler PlayerHandler;

    private IHasTooltip _currentTooltip;
    private IHasTooltip _forcedTooltip;
    private Vector2 _forcedTooltipPosition;
    public bool IsForced => _forcedTooltip != null;

    void Update()
    {
        UpdateTooltip();

        Vector2 flip = new Vector2();
        Vector2 pos = GetTooltipPosition();
        Rect rect = TooltipTransform.rect;

        if (pos.x + rect.width > Screen.width)
        {
            flip.x = -rect.width + Offset.x * -2;
        }
        if (pos.y - rect.height < 0f)
        {
            flip.y = rect.height + Offset.y * 2;
        }

        TooltipTransform.localPosition = pos + Offset + flip;
    }

    public void ForceTooltip(IHasTooltip tooltip)
        => ForceTooltip(tooltip, GetTooltipPosition());

    public void ForceTooltip(IHasTooltip tooltip, Vector2 screenPosition)
    {
        _forcedTooltip = tooltip;
        _forcedTooltipPosition = screenPosition;
    }

    public void ResetForcedTooltip()
    {
        _forcedTooltip = null;
        _forcedTooltipPosition = Vector2.zero;
    }

    private Vector2 GetTooltipPosition()
    {
        if (_forcedTooltip != null)
        {
            return _forcedTooltipPosition;
        }

        if (PlayerHandler)
        {
            return PlayerHandler.ScreenPointToPlayerScreenPoint(PlayerHandler.GetPointerScreenPosition());
        }
        else
        {
            return Mouse.current.position.ReadValue();
        }
    }

    private void UpdateTooltip ()
    {
        PointerEventData data = new PointerEventData(EventSystem);
        data.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();

        bool changed = false;
        bool any = false;

        Raycaster.Raycast(data, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent<IHasTooltip>(out var tooltip))
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

        if (_forcedTooltip != null)
        {
            if (_currentTooltip != _forcedTooltip)
            {
                changed = true;
                ClearTooltip();
            }
            any = true;
            _currentTooltip = _forcedTooltip;
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
            newTooltip.transform.localScale = Vector3.one;
            newTooltip.transform.localPosition = Vector3.zero;
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
