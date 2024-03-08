using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldButton : MonoBehaviour, IWorldInteractable
{
    public UnityEvent<WorldPointer, int> PointerDown;
    public UnityEvent<WorldPointer> PointerEnter;
    public UnityEvent<WorldPointer> PointerExit;
    public UnityEvent<WorldPointer, int> PointerHeld;
    public UnityEvent<WorldPointer> PointerHover;
    public UnityEvent<WorldPointer, int> PointerUp;

    public void OnPointerDown(WorldPointer pointer, int button)
    {
        PointerDown?.Invoke(pointer, button);
    }

    public void OnPointerEnter(WorldPointer pointer)
    {
        PointerEnter?.Invoke(pointer);
    }

    public void OnPointerExit(WorldPointer pointer)
    {
        PointerExit?.Invoke(pointer);
    }

    public void OnPointerHeld(WorldPointer pointer, int button)
    {
        PointerHeld?.Invoke(pointer, button);
    }

    public void OnPointerHover(WorldPointer pointer)
    {
        PointerHover?.Invoke(pointer);
    }

    public void OnPointerUp(WorldPointer pointer, int button)
    {
        PointerUp?.Invoke(pointer, button);
    }
}
