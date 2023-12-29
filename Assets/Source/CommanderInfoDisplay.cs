using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommanderInfoDisplay : MonoBehaviour
{
    public Text Text;
    public Commander Commander;

    private void Start()
    {
        // Player is bold
        if (!(Commander is AICommander)) Text.fontStyle = FontStyle.Bold;
    }

    private void FixedUpdate()
    {
        string colorHex = ColorUtility.ToHtmlStringRGBA(Commander.TeamInfo.Color); ;
        Text.text = $"<color=#{colorHex}>{Commander.Name} | Credits: {Commander.Credits} | Income {(int)Commander.AverageIncomePerSecond} | Offense Factor {Commander.OffenseFactor.ToString("0.00")} | Defense Factor {Commander.DefenseFactor.ToString("0.00")}</color>";
    }
}
