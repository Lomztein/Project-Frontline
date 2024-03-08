using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class UnitTooltip
{
    private const string RESOURCE_PATH = "Tooltips/Unit";

    public static GameObject Create(Unit unit, Commander owner)
    {
        GameObject newTooltip = Object.Instantiate(Resources.Load<GameObject>(RESOURCE_PATH));
        newTooltip.transform.Find("Name").GetComponentInChildren<Text>().text = $"<b>{unit.Name}</b>" + " - " + (owner == null ? unit.BaseCost.ToString() : unit.GetCost(owner)) + "$";
        string desc = string.IsNullOrEmpty(unit.Info.ShortDescription) ? unit.Info.Description : unit.Info.ShortDescription;        
        newTooltip.transform.Find("Description").GetComponentInChildren<Text>().text = unit.Description;
        string purchaseNotes = owner == null ? null : owner.GetCanAffordAndPurchaseDescription(unit.gameObject);
        var notes = newTooltip.transform.Find("PurchaseNotes");
        if (string.IsNullOrWhiteSpace(purchaseNotes))
        {
            notes.gameObject.SetActive(false);
        }
        else
        {
            notes.Find("Text").GetComponent<Text>().text = purchaseNotes;
        }
        string weaponInfo = EquipmentInfoString(unit, owner);
        newTooltip.transform.Find("Equipment").GetComponentInChildren<Text>().text = weaponInfo;
        return newTooltip;
    }

    private static string EquipmentInfoString(Unit unit, Commander commander)
    {
        StringBuilder builder = new StringBuilder();
        Health[] healths = unit.GetComponentsInChildren<Health>();
        IWeapon[] weapons = unit.GetWeapons().ToArray();

        foreach (Health health in healths)
        {
            IEnumerable<DamageModifier> strongAgainst = commander != null ? DamageModifierUtils.GetLowestAgainst(health.Modifier, DamageModifierUtils.GetAvailableWeaponModifiers(commander.Target)) : new DamageModifier[0];
            IEnumerable<DamageModifier> weakAgainst = commander != null ? DamageModifierUtils.GetHighestAgainst(health.Modifier, DamageModifierUtils.GetAvailableWeaponModifiers(commander.Target)) : new DamageModifier[0];

            builder.AppendLine($"<b>{health.MaxHealth} {health.Modifier.Name} HP</b>");
            builder.AppendLine($"\tStrong against: {string.Join(", ", strongAgainst)}");
            builder.AppendLine($"\tWeak against: {string.Join(", ", weakAgainst)}");
        }

        foreach (IWeapon weapon in weapons)
        {
            IEnumerable<DamageModifier> strongAgainst = commander != null ? DamageModifierUtils.GetHighestAgainst(weapon.Modifier, DamageModifierUtils.GetAvailableArmorModifiers(commander.Target)) : new DamageModifier[0];
            IEnumerable<DamageModifier> weakAgainst = commander != null ? DamageModifierUtils.GetLowestAgainst(weapon.Modifier, DamageModifierUtils.GetAvailableArmorModifiers(commander.Target)) : new DamageModifier[0];

            builder.AppendLine($"<b>{weapon.GetDPSOrOverride()} {weapon.Modifier.Name} DPS</b>");
            builder.AppendLine($"\tStrong against: {string.Join(", ", strongAgainst)}");
            builder.AppendLine($"\tWeak against: {string.Join(", ", weakAgainst)}");
        }

        return builder.ToString().Trim();
    }
}
