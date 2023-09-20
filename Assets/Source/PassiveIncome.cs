using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveIncome : MonoBehaviour, ICommanderComponent
{
    public Commander Commander;
    public Unit Unit;
    public int IncomePerSecond;

    public void AssignCommander(Commander commander)
    {
        Commander = commander;
    }

    private void Start()
    {
        InvokeRepeating(nameof(MakeMadDosh), 1f, 1f);
        Unit = GetComponent<Unit>();
    }

    private void MakeMadDosh ()
    {
        if (Commander)
        {
            Commander.Earn(IncomePerSecond);
            if (Unit) Unit.ChangeStat("CreditsEarned", IncomePerSecond);
        }
    }
}
