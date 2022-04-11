using System.Linq;
using UnityEngine;

public class WeaponProjectileChangeOnSpawnUpgradeStructure : ChanceOnUnitSpawnUpgradeStructure
{
    public WeaponProjectileUpgrade[] Upgrades;

    // A bug is likely going to occur where, if this is placed before an armory, then infantry will only have one of their weapons upgraded.

    protected override void ApplyUpgrade(Unit target)
    {
        Weapon[] weapons = target.GetComponentsInChildren<Weapon>(); // Need to get *all* concrete Weapon objects. Hopefully performance doesn't hurt too much.
        foreach (var weapon in weapons)
        {
            var upgrade = Upgrades.FirstOrDefault(x => x.ProjectileType == weapon.ProjectilePrefab);
            if (upgrade != null)
            {
                ApplyWeaponUpgrade(weapon, upgrade);
                AIController controller = target.GetComponent<AIController>();
                if (controller.Weapons[0] is Weapon concrete && concrete == weapon)
                {
                    controller.LeadTarget = false; // If the main weapon is changed to coil, don't lead the target.
                }
            }
        }
    }

    private void ApplyWeaponUpgrade(Weapon weapon, WeaponProjectileUpgrade upgrade)
    {
        weapon.ProjectilePrefab = upgrade.ReplaceType;
        weapon.Damage *= upgrade.DamageMultiplier;
        weapon.Speed *= upgrade.SpeedMultiplier;
        weapon.Firerate *= upgrade.FirerateMultiplier;
        weapon.Amount = (int)(weapon.Amount * upgrade.AmountMultiplier);
    }

    [System.Serializable]
    public class WeaponProjectileUpgrade
    {
        public GameObject ProjectileType;
        public GameObject ReplaceType;

        public float DamageMultiplier = 1f;
        public float SpeedMultiplier = 1f;
        public float FirerateMultiplier = 1f;
        public float AmountMultiplier = 1f;
    }
}
