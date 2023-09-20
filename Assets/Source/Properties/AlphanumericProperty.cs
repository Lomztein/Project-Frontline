using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlphanumericProperty : PropertyBase
{
    public TMP_InputField.ContentType ContentType;

    public AlphanumericProperty(string name, string description, TMP_InputField.ContentType contentType) : base(name, description)
    {
        ContentType = contentType;
    }
}
