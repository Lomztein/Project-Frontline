using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngagedTracker : MonoBehaviour
{
    public float LastAttackTime { get; private set; }
    public float LastDamagedTime { get; private set; }

    private Health[] _healths;
    private IWeapon[] _weapons;

    private void Awake()
    {
        LastAttackTime = Time.time;
        LastDamagedTime = Time.time;

        _healths = GetComponentsInChildren<Health>();
        foreach (Health health in _healths)
        {
            health.OnDamageTaken += Health_OnDamageTaken;
        }
        _weapons = GetComponentsInChildren<IWeapon>();
        foreach (IWeapon weapon in _weapons)
        {
            weapon.OnFire += Weapon_OnFire;
        }
    }

    private void OnDestroy()
    {
        foreach (Health health in _healths)
        {
            health.OnDamageTaken -= Health_OnDamageTaken;
        }
        foreach (IWeapon weapon in _weapons)
        {
            weapon.OnFire -= Weapon_OnFire;
        }
    }

    private void Weapon_OnFire(IWeapon obj)
    {
        LastAttackTime = Time.time;
    }

    private void Health_OnDamageTaken(float obj)
    {
        LastDamagedTime = Time.time;
    }
}
