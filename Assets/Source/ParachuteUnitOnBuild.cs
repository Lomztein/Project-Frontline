using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParachuteUnitOnBuild : MonoBehaviour
{
    public UnitFactoryWeapon Factory;
    public GameObject ParachutePrefab;


    // Start is called before the first frame update
    void Start()
    {
        Factory.OnUnitSpawned += Factory_OnUnitSpawned;
    }

    private void Factory_OnUnitSpawned(UnitFactoryWeapon arg1, GameObject arg2)
    {
        StartCoroutine(DelayedParachute(arg2));
    }

    private IEnumerator DelayedParachute(GameObject unit)
    {
        yield return new WaitForEndOfFrame();
        GameObject parachute = Instantiate(ParachutePrefab, transform.position, transform.rotation);
        Parachute chute = parachute.GetComponent<Parachute>();

        chute.Garrison.EnterGarrison(unit);
    }
}
