using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class InfantryGarrison : MonoBehaviour
{
    public Transform[] Slots;
    public bool HideGarrisoned;
    public bool EvacuateOnDestroy = true;

    public int SlotCount => Slots.Length;
    public int GarrisonCount => Slots.Count(x => x.childCount != 0);
    public int AvailableCount => Slots.Count(x => x.childCount == 0);

    public bool CanGarrison(GameObject unit)
        => unit.GetComponentInChildren<InfantryBody>() != null
        && unit.GetComponentInChildren<Unit>().Info.UnitType == UnitInfo.Type.Infantry
        && unit.transform.root != transform.root; // For the time being simply support infantry bodies only. Later perhaps refactor into an IGarrisonable interface.

    public void EnterGarrison(GameObject unit)
    {
        Assert.IsTrue(CanGarrison(unit), "Tried to garrison unit that cannot be garrisoned.");

        if (AvailableCount > 0)
        {
            InfantryBody body = unit.GetComponentInChildren<InfantryBody>();
            Transform slot = GetFirstEmptySlot();
            unit.transform.SetParent(slot);
            unit.transform.position = slot.position;
            unit.transform.rotation = slot.rotation;
            body.enabled = false;

            if (HideGarrisoned)
            {
                unit.transform.localScale = Vector3.one * 0.1f;
            }
        }
        else
        {
            throw new InvalidOperationException("Cannot garrison a unit as the garrison is already full.");
        }

    }

    private void OnDestroy()
    {
        if (EvacuateOnDestroy && gameObject.scene.isLoaded) // Check if OnDestroy is called because the object was destroyed or due to scene unloading.
        {
            EvacuateAll();
        }
    }

    public void LeaveGarrison (GameObject unit)
    {
        if (IsGarrisoned(unit))
        {
            InfantryBody body = unit.GetComponentInChildren<InfantryBody>();
            body.enabled = true;
            unit.transform.SetParent(null);
            unit.transform.SetPositionAndRotation(
                new Vector3(unit.transform.position.x, transform.root.position.y, unit.transform.position.z),
                Quaternion.Euler(0f, unit.transform.eulerAngles.y, 0f));

            if (HideGarrisoned)
            {
                unit.transform.localScale = Vector3.one;
            }
        }
        else
        {
            throw new InvalidOperationException("This unit is not garrisoned here.");
        }
    }

    private Transform GetFirstEmptySlot()
        => Slots.FirstOrDefault(x => x.childCount == 0);

    private Transform GetFirstFilledSlot()
        => Slots.FirstOrDefault(x => x.childCount != 0);

    private bool IsGarrisoned(GameObject unit)
        => Slots.Any(x => x.childCount > 0 && x.GetChild(0).gameObject == unit);

    public void EvacuateAll ()
    {
        foreach (Transform slot in Slots)
        {
            if (slot.childCount > 0)
            {
                GameObject child = slot.GetChild(0).gameObject; // A slot should only ever have a single child.
                LeaveGarrison(child);
            }
        }
     }
}
