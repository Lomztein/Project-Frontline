using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PropertyControl : MonoBehaviour
{
    private const string RESOURCE_PATH = "PropertyControls";
    protected IProperty Property { get; private set; }
    protected IHasProperties Parent { get; private set; }

    public abstract bool CanHandle(IProperty property);

    public event Action<IProperty, IHasProperties, object> OnPropertyChanged;
    protected void InvokeOnPropertyChanged(IProperty property, IHasProperties parent, object value)
        => OnPropertyChanged?.Invoke(property, parent, value);

    public virtual void Assign(IProperty property, IHasProperties parent)
    {
        Property = property;
        Parent = parent;
    }

    public static GameObject GetControlPrefab(IProperty property)
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>(RESOURCE_PATH);
        foreach (GameObject prefab in prefabs)
        {
            if (prefab.GetComponent<PropertyControl>().CanHandle(property))
            {
                return prefab;
            }
        }
        throw new InvalidOperationException($"No control available for property type {property.GetType().FullName}");
    }
}
