using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsDisplay : MonoBehaviour
{
    public Commander Commander;
    public string Prefix = "Credits: ";
    public string Suffix = "$";
    public Text Text;

    void Update()
    {
        Text.text = Prefix + Commander.Credits.ToString() + Suffix;
    }
}
