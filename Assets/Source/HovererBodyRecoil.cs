using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovererBodyRecoil : MonoBehaviour
{
    public HovererBody Body;

    public Health Health;
    public float Sturdyness;

    public GameObject WeaponObject;
    public Transform Muzzle;
    public float Recoil;

    private void Start()
    {
        if (!Body)
            Body = GetComponent<HovererBody>();
        if (Health)
            Health.OnDamageTaken += Health_OnTakeDamage;
        if (WeaponObject)
            WeaponObject.GetComponent<IWeapon>().OnFire += HovererBodyRecoil_OnFire;
    }

    private void Health_OnTakeDamage(Health arg1, DamageInfo arg2)
    {
        Body.Impact(arg2.DamageDone / Sturdyness * arg2.Direction);
    }

    private void HovererBodyRecoil_OnFire(IWeapon obj)
    {
        var muzzle = Muzzle;
        if (muzzle == null)
            muzzle = WeaponObject.transform;
        Body.Impact(Recoil * muzzle.forward * -1f);
    }
}
