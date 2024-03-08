using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIDragListener : MonoBehaviour, IDragHandler
{
    public UnityEvent<Vector2> OnUIDrag;

    public void OnDrag(PointerEventData eventData)
    {
        OnUIDrag.Invoke(new Vector3(eventData.delta.y, eventData.delta.x));
    }
}
