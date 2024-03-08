using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPointer : MonoBehaviour
{
    public Transform Visualizer;
    public IWorldInteractable CurrentInteractable;
    public LayerMask LayerMask;
    public bool[] ButtonState = new bool[2];

    public Vector3 WorldPosition;

    public bool PrimaryState => ButtonState[0];
    public bool SecondaryState => ButtonState[1];
    
    public void Point(Ray ray)
    {
        IWorldInteractable newInteractable = null;
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask);
        Array.Sort(hits, (x, y) => (int)Mathf.Sign(x.distance - y.distance));
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Shield"))
            {
                continue;
            }
            WorldPosition = hit.point;
            newInteractable = hit.collider.GetComponent<IWorldInteractable>();
            if (Visualizer)
            {
                Visualizer.transform.position = hit.point;
                Visualizer.transform.rotation = Quaternion.LookRotation(hit.normal);
            }
            break;
        }
        UpdateInteractable(newInteractable);
    }

    public void SetVisualizerEnabled(bool enabled)
    {
        Visualizer.gameObject.SetActive(enabled);
    }

    public bool IsVisualizerEnabled() 
        => Visualizer.gameObject.activeSelf;

    public void SetButtonAmount(int amount)
        => ButtonState = new bool[amount];

    public void UpdateButtonState(bool[] state)
    {
        if (state.Length != ButtonState.Length)
        {
            throw new ArgumentException("New state must be same length as set state.");
        }

        if (CurrentInteractable != null)
        {
            for (int i = 0; i < ButtonState.Length; i++)
            {
                if (ButtonState[i] == false && state[i] == true)
                {
                    CurrentInteractable.OnPointerDown(this, i);
                }
                if (ButtonState[i] == true && state[i] == false)
                {
                    CurrentInteractable.OnPointerUp(this, i);
                }
                if (ButtonState[i] == true && state[i] == true)
                {
                    CurrentInteractable.OnPointerHeld(this, i);
                }
            }
        }

        ButtonState = state;
    }

    private void UpdateInteractable(IWorldInteractable newInteractable)
    {
        if (newInteractable == null)
        {
            if (CurrentInteractable != null)
            {
                CurrentInteractable.OnPointerExit(this);
            }
        }

        if (newInteractable != null)
        {
            if (CurrentInteractable != null)
            {
                if (CurrentInteractable != newInteractable)
                {
                    newInteractable.OnPointerEnter(this);
                    CurrentInteractable.OnPointerExit(this);
                }
                else
                {
                    CurrentInteractable.OnPointerHover(this);
                }
            }
            else
            {
                newInteractable.OnPointerEnter(this);
            }
        }

        CurrentInteractable = newInteractable;
    }
}
