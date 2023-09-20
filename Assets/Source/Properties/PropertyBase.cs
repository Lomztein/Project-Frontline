using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyBase : IProperty
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    public PropertyBase(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
