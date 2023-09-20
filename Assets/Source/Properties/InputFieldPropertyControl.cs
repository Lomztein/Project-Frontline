using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class InputFieldPropertyControl : PropertyControl
{
    public Text Text;
    public TMP_InputField Input;

    public override void Assign(IProperty property, IHasProperties parent)
    {
        base.Assign(property, parent);
        var ap = property as AlphanumericProperty;
        Text.text = property.Name;
        Input.contentType = ap.ContentType;
        Input.text = parent.GetProperty(property).ToString();
        Input.onValueChanged.AddListener(OnChanged);
    }

    public void OnChanged (string val)
    {
        Parent.SetProperty(Property, val);
        InvokeOnPropertyChanged(Property, Parent, val);
    }

    public override bool CanHandle(IProperty property)
        => property is AlphanumericProperty;
}
