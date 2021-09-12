using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveIncome : MonoBehaviour, ICommanderComponent
{
    public Commander Commander;
    public int IncomePerSecond;

    public void AssignCommander(Commander commander)
    {
        Commander = commander;
    }

    private void Start()
    {
        InvokeRepeating(nameof(MakeMadDosh), 1f, 1f);
    }

    private void MakeMadDosh ()
    {
        if (Commander)
        {
            Commander.Earn(IncomePerSecond);
        }
    }
}
