using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceObjectOnSpawnUpgradeStructure : ChanceOnUnitSpawnUpgradeStructure
{
    public string ObjectToRemotePath;
    public GameObject ObjectToSpawn;

    protected override void ApplyUpgrade(Unit target)
    {
        Transform toRemove = target.transform.Find(ObjectToRemotePath);
        if (toRemove)
        {
            AIController controller = target.GetComponent<AIController>();
            ITurret turret = toRemove.GetComponent<ITurret>();
            IWeapon weapon = toRemove.GetComponent<IWeapon>();

            GameObject newObject = Instantiate(ObjectToSpawn, toRemove.parent);
            newObject.transform.SetLocalPositionAndRotation(toRemove.localPosition, toRemove.localRotation);
            ITurret newTurret = newObject.GetComponent<ITurret>();
            IWeapon newWeapon = newObject.GetComponent<IWeapon>();
            newObject.name = toRemove.name;

            // If replaced part was turret / weapon in AI, then replace.
            if (turret != null && controller.Turret == turret)
            {
                controller.Turret = newTurret;
            }
            if (weapon != null & controller.Weapons.Remove(weapon))
            {
                controller.Weapons.Add(newWeapon);
            }

            if (weapon != null) target.RemoveWeapon(weapon);
            if (newWeapon != null) target.AddWeapon(newWeapon);

            WeaponRecoil recoil = target.GetComponentInChildren<WeaponRecoil>();
            if (newWeapon != null && recoil.Weapon as IWeapon == weapon)
            {
                recoil.SetWeapon(newWeapon);
                if (newWeapon is Weapon wep)
                {
                    recoil.Muzzle = wep.Muzzle;
                }
                if (newWeapon is WeaponGroup wepGroup)
                {
                    recoil.Muzzle = wepGroup.transform;
                }
            }

            if (target.Commander)
            {
                target.Commander.AssignCommander(newObject);
            }
            if (target.TeamInfo)
            {
                target.TeamInfo.ApplyTeam(newObject);
            }

            Destroy(toRemove.gameObject);
        }
    }
}
