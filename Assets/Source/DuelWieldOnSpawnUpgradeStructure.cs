using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DuelWieldOnSpawnUpgradeStructure : ChanceOnUnitSpawnUpgradeStructure
{
    public string CommonRootName = "spine.003";
    public char SideSplitCharacter = '.';
    public string LeftSideSuffix = "L";
    public string RightSideSuffix = "R";

    protected override void ApplyUpgrade(Unit target)
    {
        var weapons = target.GetWeapons();
        if (weapons.Any())
        {
            IWeapon weapon = weapons.First();
            if (weapon is Component component)
            {
                var infTurret = component.GetComponent<InfantryWeaponTurret>();
                if (infTurret)
                {
                    Transform opposite = FindOppositeBodyPart(infTurret.Pivot);
                    if (opposite)
                    {
                        CopyWeaponTo(target, component.gameObject, opposite);
                    }
                }
            }
        }
    }

    private void CopyWeaponTo (Unit unit, GameObject weaponObj, Transform parent)
    {
        GameObject newWeapon = Instantiate(weaponObj, weaponObj.transform.parent);
        newWeapon.transform.localScale += Vector3.right * newWeapon.transform.localScale.x * -2;
        var turret = newWeapon.GetComponent<InfantryWeaponTurret>();
        if (turret)
        {
            turret.Pivot = parent;
            unit.GetComponentInChildren<CompositeTurret>().AddTurret(turret);
        }
        unit.GetComponent<AIController>().AddWeapon(newWeapon.GetComponent<IWeapon>());
    }

    private Transform FindOppositeBodyPart (Transform original)
    {
        Transform root = FindCommonRoot(original, out string pathToOriginal);
        string oppositeSideName = GetOppositeSideName(pathToOriginal);

        if (root == null || oppositeSideName == null)
        {
            return null;
        }

        return root.Find(oppositeSideName);
    }

    private string GetOppositeSideName(string path)
    {
        if (path.EndsWith(LeftSideSuffix))
            return path.Replace(SideSplitCharacter + LeftSideSuffix, SideSplitCharacter + RightSideSuffix);
        else if (path.EndsWith(RightSideSuffix))
            return path.Replace(SideSplitCharacter + RightSideSuffix, SideSplitCharacter + LeftSideSuffix);
        else return null;
    }

    private Transform FindCommonRoot(Transform original, out string pathToOriginal)
    {
        Transform parent = original.parent;
        pathToOriginal = original.name;
        while (parent)
        {
            if (IsCommonRoot(parent))
            {
                return parent;
            }
            pathToOriginal = parent.name + "/" + pathToOriginal;
            parent = parent.parent;
        }
        return null;
    }

    private bool IsCommonRoot(Transform part) => part.name.Equals(CommonRootName);
}
