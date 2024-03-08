using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickableWorldObject : MonoBehaviour
{
    public UnityEvent MouseEnter;
    public UnityEvent MouseExit;
    public UnityEvent MouseDown;

    private void OnMouseEnter()
    {
        MouseEnter?.Invoke();
    }

    private void OnMouseExit()
    {
        MouseExit?.Invoke();
    }

    private void OnMouseDown()
    {
        MouseDown?.Invoke();
    }
}
