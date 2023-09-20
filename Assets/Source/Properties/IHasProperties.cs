using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasProperties
{
    public IEnumerable<IProperty> GetProperties();

    public bool SetProperty(IProperty property, object value);

    public object GetProperty(IProperty property);
}
