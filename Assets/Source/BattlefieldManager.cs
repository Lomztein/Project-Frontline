using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldManager : MonoBehaviour
{
    // Watch me write a god class :sunglasses:

    public Commander[] Commanders;

    private void Start()
    {
        foreach (Commander commander in Commanders)
        {
            commander.OnFortressDestroyed += Commander_OnFortressDestroyed;
            commander.OnEliminated += Commander_OnEliminated;
        }        
    }

    private void Commander_OnFortressDestroyed(Commander obj)
    {
        Debug.Log("Commander " + obj.Name + " f.");
    }

    private void Commander_OnEliminated(Commander obj)
    {
        Debug.Log("Commander " + obj.Name + " eliminated.");
    }
}
