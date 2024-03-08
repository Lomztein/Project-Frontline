using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorldInteractable
{
    public void OnPointerEnter(WorldPointer pointer);
    public void OnPointerHover(WorldPointer pointer);
    public void OnPointerExit(WorldPointer pointer);

    public void OnPointerDown(WorldPointer pointer, int button);
    public void OnPointerUp(WorldPointer pointer, int button);
    public void OnPointerHeld(WorldPointer pointer, int button);
}
