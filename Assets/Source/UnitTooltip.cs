using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class UnitTooltip
{
    private const string RESOURCE_PATH = "Tooltips/Unit";

    public static GameObject Create(Unit unit)
    {
        GameObject newTooltip = Object.Instantiate(Resources.Load<GameObject>(RESOURCE_PATH));
        newTooltip.transform.Find("Name").GetComponentInChildren<Text>().text = unit.Name + " - " + unit.Cost + "$";
        newTooltip.transform.Find("Description").GetComponentInChildren<Text>().text = unit.Description;
        string weaponInfo = WeaponInfoToString(unit);
        if (string.IsNullOrEmpty(weaponInfo))
        {
            newTooltip.transform.Find("Weapons").gameObject.SetActive(false);
        }
        else
        {
            newTooltip.transform.Find("Weapons").GetComponentInChildren<Text>().text = WeaponInfoToString(unit);
        }
        return newTooltip;
    }

    private static string WeaponInfoToString(Unit unit)
    {
        WeaponInfo[] info = unit.GetWeaponInfo();
        if (info.Length > 0)
        {
            var groups = info.GroupBy(x => x.Name);
            StringBuilder builder = new StringBuilder();
            foreach (var group in groups)
            {
                IWeapon weapon = group.First().GetComponent<IWeapon>();

                int count = group.Count();
                string prefix = count == 1 ? "" : count + "x ";
                string suffix = weapon != null ? $" - {weapon.GetDPS() * count} {weapon.DamageType}-type DPS" : "";
                builder.AppendLine($"<b>{prefix}{group.Key}</b>{suffix}");
                builder.AppendLine($"<i>{group.First().Description}</i>");
            }
            return builder.ToString().Trim();
        }
        return null;
    }
}