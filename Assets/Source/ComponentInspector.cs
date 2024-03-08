using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComponentInspector : ScriptableObject, IInspector
{
    public abstract bool CanInspect(object obj);
    public abstract GameObject InstantiateInspectUI(object obj);
    public abstract void UpdateInspectorUI(object obj, GameObject uiObject);
}
