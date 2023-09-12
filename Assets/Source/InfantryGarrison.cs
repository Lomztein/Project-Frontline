using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class InfantryGarrison : MonoBehaviour
{
    public GarrisonSlot[] Slots;
    public bool HideGarrisoned;
    public bool EvacuateOnDestroy = true;
    public float OccupantRangeMultiplier;

    public int SlotCount => Slots.Length;
    public int GarrisonCount => Slots.Count(x => x.IsOccupied);
    public int AvailableCount => Slots.Count(x => x.IsAvailable);
    public int AvailableEmplacements => Slots.Count(x => x.IsAvailable && x.HasEmplacement);

    public bool CanGarrison(GameObject unit)
    {
        Unit info = unit.GetComponentInChildren<Unit>();

        return unit.GetComponentInChildren<InfantryBody>() != null
            && info.Info.UnitType == UnitInfo.Type.Infantry
            && !info.Info.Tags.Contains("CantGarrison")
            && (!info.Info.Tags.Contains("EmplaceGarrison") || (info.Info.Tags.Contains("EmplaceGarrison") && AvailableEmplacements > 0))
            && unit.transform.root != transform.root; // For the time being simply support infantry bodies only. Later perhaps refactor into an IGarrisonable interface.
    }

    public void EnterGarrison(GameObject unit)
    {
        Assert.IsTrue(CanGarrison(unit), "Tried to garrison unit that cannot be garrisoned.");
        Unit info = unit.GetComponentInChildren<Unit>();

        if (AvailableCount > 0)
        {
            InfantryBody body = unit.GetComponentInChildren<InfantryBody>();
            var ai = unit.GetComponent<AIController>();
            ai.AttackRange *= OccupantRangeMultiplier;
            ai.AcquireTargetRange *= OccupantRangeMultiplier;
            ai.LooseTargetRange *= OccupantRangeMultiplier;

            GarrisonSlot slot = GetFirstEmptySlot(info.Info.Tags.Contains("EmplaceGarrison"));
            unit.transform.SetParent(slot.GarrionParent);
            unit.transform.position = slot.GarrionParent.position;
            unit.transform.rotation = slot.GarrionParent.rotation;
            slot.Occupant = unit;
            body.enabled = false;

            IEmplacable emplacableWeapon = info.GetWeapons().Where(x => x is IEmplacable).FirstOrDefault() as IEmplacable;
            if (emplacableWeapon != null)
            {
                emplacableWeapon.Emplace(slot.EmplacementParent);
            }

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
        if (gameObject.scene.isLoaded)  // Check if OnDestroy is called because the object was destroyed or due to scene unloading.
        {
            if (EvacuateOnDestroy)
            {
                EvacuateAll();
            }
            else
            {
                KillEveryone();
            }
        }
    }

    public void LeaveGarrison (GameObject unit)
    {
        GarrisonSlot slot = GetSlotWithUnit(unit);
        if (slot != null)
        {
            InfantryBody body = unit.GetComponentInChildren<InfantryBody>();
            var ai = unit.GetComponent<AIController>();
            ai.AttackRange /= OccupantRangeMultiplier;
            ai.AcquireTargetRange /= OccupantRangeMultiplier;
            ai.LooseTargetRange /= OccupantRangeMultiplier;

            body.enabled = true;
            unit.transform.SetParent(null);
            unit.transform.SetPositionAndRotation(
                new Vector3(unit.transform.position.x, transform.root.position.y, unit.transform.position.z),
                Quaternion.Euler(0f, unit.transform.eulerAngles.y, 0f));
            slot.Occupant = null;

            IEmplacable emplacableWeapon = unit.GetComponent<Unit>().GetWeapons().Where(x => x is IEmplacable).FirstOrDefault() as IEmplacable;
            if (emplacableWeapon != null)
            {
                emplacableWeapon.Detatch();
            }

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

    private GarrisonSlot GetFirstEmptySlot(bool requireEmplace)
    {
        if (requireEmplace)
        {
            return Slots.FirstOrDefault(x => x.IsAvailable && x.HasEmplacement);
        }
        else
        {
            return Slots.FirstOrDefault(x => x.IsAvailable);
        }
    }

    private GarrisonSlot GetSlotWithUnit(GameObject unit)
        => Slots.FirstOrDefault(x => x.Occupant == unit);

    public void EvacuateAll ()
    {
        foreach (GarrisonSlot slot in Slots)
        {
            if (slot.IsOccupied)
            {
                GameObject child = slot.Occupant;
                LeaveGarrison(child);
            }
        }
    }

    public void KillEveryone()
    {
        foreach (GarrisonSlot slot in Slots)
        {
            if (slot.IsOccupied)
            {
                GameObject child = slot.Occupant;
                Health health = child.GetComponent<Unit>().Health;
                health.TakeDamage(new DamageInfo(health.CurrentHealth * 2f, DamageModifier.One, transform.position, transform.forward, this, health));
            }
        }
    }

    [System.Serializable]
    public class GarrisonSlot
    {
        public Transform GarrionParent;
        public Transform EmplacementParent;

        public GameObject Occupant;

        public bool IsOccupied => Occupant != null;
        public bool IsAvailable => Occupant == null;

        public bool HasEmplacement => EmplacementParent != null;
    }

}
