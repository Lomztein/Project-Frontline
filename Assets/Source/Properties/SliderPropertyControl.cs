using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderPropertyControl : PropertyControl
{
    public Slider Slider;
    public Text Text;

    public override void Assign(IProperty property, IHasProperties parent)
    {
        base.Assign(property, parent);
        RangeProperty rp = property as RangeProperty;
        Slider.minValue = rp.Min;
        Slider.maxValue = rp.Max;
        Slider.wholeNumbers = rp.IntegersOnly;
        Slider.value = Convert.ToSingle(parent.GetProperty(property));
        Text.text = property.Name + ": " + Slider.value;

        Slider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(float value)
    {
        Parent.SetProperty(Property, value);
        Text.text = Property.Name + ": " + Slider.value;
        InvokeOnPropertyChanged(Property, Parent, value);
    }

    public override bool CanHandle(IProperty property)
        => property is RangeProperty;
}
