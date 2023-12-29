using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAutoswitcher : MonoBehaviour
{
    public CameraSelector Selector;
    public bool EnableAutoselect;
    public int[] AutoselectableIndices;
    public Vector2 SwitchTimeMinMax;

    private void Start()
    {
        Switch();
    }

    public void SetAutoselect(bool value)
    {
        EnableAutoselect = value;
    }

    private void Switch()
    {
        if (EnableAutoselect)
        {
            int newCamera = AutoselectableIndices[Random.Range(0, AutoselectableIndices.Length)];
            Selector.SelectCamera(newCamera);
        }

        Invoke(nameof(Switch), Random.Range(SwitchTimeMinMax.x, SwitchTimeMinMax.y));
    }
}
