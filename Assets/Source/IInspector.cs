using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInspector
{
    public bool CanInspect(object obj);

    public GameObject InstantiateInspectUI(object obj);

    public void UpdateInspectorUI(object obj, GameObject uiObject);
}