using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParachuteGarrisonOnDeath : MonoBehaviour
{
    public InfantryGarrison Garrison;
    public Health Health;
    public GameObject ParachutePrefab;

    void Start()
    {
        Health.OnDeath += Health_OnDeath;
    }

    private void Health_OnDeath()
    {
        foreach (var slot in Garrison.Slots)
        {
            if (slot.childCount > 0)
            {
                GameObject obj = slot.GetChild(0).gameObject;
                Garrison.LeaveGarrison(obj);

                GameObject parachute = Instantiate(ParachutePrefab, slot.transform.position, slot.transform.rotation);
                Parachute chute = parachute.GetComponent<Parachute>();

                chute.Garrison.EnterGarrison(obj);
            }
        }
    }
}
