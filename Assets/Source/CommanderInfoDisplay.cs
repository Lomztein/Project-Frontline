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
        Text.text = $"{Commander.Name} of Team {Commander.TeamInfo.Name} | Credits: {Commander.Credits} | Income {(int)Commander.AverageIncomePerSecond} | Offense Factor {Commander.OffenseFactor.ToString("0.00")} | Defense Factor {Commander.DefenseFactor.ToString("0.00")}";
    }
}
