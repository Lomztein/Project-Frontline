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

    private void Health_OnDeath(Health health)
    {
        foreach (var slot in Garrison.Slots)
        {
            if (slot.IsOccupied)
            {
                GameObject obj = slot.Occupant;
                Garrison.LeaveGarrison(obj);

                GameObject parachute = Instantiate(ParachutePrefab, slot.GarrionParent.position, slot.GarrionParent.rotation);
                Parachute chute = parachute.GetComponent<Parachute>();

                chute.Garrison.EnterGarrison(obj);
            }
        }
    }
}
