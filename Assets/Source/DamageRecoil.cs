using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRecoil : MonoBehaviour
{
    public Health Health;
    public VehicleBodyRecoilAnimator Anim;
    public float Sturdyness;

    void Start()
    {
        Health.OnTakeDamage += Health_OnDamageTaken;
    }

    private void Health_OnDamageTaken(Health health, DamageInfo info)
    {
        Anim.Recoil(info.Direction * info.Damage / Sturdyness);
    }
}
