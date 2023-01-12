using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A type of weapon that pretty much just deploys another carried weapon, and then works through that.
/// </summary>
public class DeployWeapon : MonoBehaviour, IWeapon, ITurret
{
    public bool IsDeployed { get; private set; }

    public GameObject ChildWeaponObject;
    private IWeapon _childWeapon;
    private ITurret _childTurret;

    public IWeapon ChildWeapon => GetChildWeapon();
    public ITurret ChildTurret => GetChildTurret();

    public float DeployTime;
    public float UnengagedUndeployTime;
    public EngagedTracker EngagedTracker;
    private float _deployTime;

    public float Damage => ChildWeapon.Damage;
    public float Firerate => ChildWeapon.Firerate;
    public float Speed => ChildWeapon.Speed;

    public bool DisableBodyWhenDeployed;
    public MobileBody Body;

    private IWeapon GetChildWeapon ()
    {
        if (_childWeapon == null)
            _childWeapon = ChildWeaponObject.GetComponent<IWeapon>();
        return _childWeapon;
    }

    private ITurret GetChildTurret ()
    {
        if (_childTurret == null)
            _childTurret = ChildWeaponObject.GetComponent<ITurret>();
        return _childTurret;
    }

    public DamageMatrix.Damage DamageType => ChildWeapon.DamageType;

    public event Action<IWeapon> OnFire
    {
        add
        {
            ChildWeapon.OnFire += value;
        }

        remove
        {
            ChildWeapon.OnFire -= value;
        }
    }
    public event Action<IWeapon, Projectile> OnProjectile
    {
        add
        {
            ChildWeapon.OnProjectile += value;
        }

        remove
        {
            ChildWeapon.OnProjectile -= value;
        }
    }
    public event Action<IWeapon, Projectile, Collider, Vector3, Vector3> OnHit
    {
        add
        {
            ChildWeapon.OnHit += value;
        }

        remove
        {
            ChildWeapon.OnHit -= value;
        }
    }
    public event Action<IWeapon, Projectile, IDamagable> OnKill
    {
        add
        {
            ChildWeapon.OnKill += value;
        }

        remove
        {
            ChildWeapon.OnKill -= value;
        }
    }

    public Transform DeployParent;
    public Vector3 DeployLocalPosition;
    public Vector3 DeployLocalRotation;
    public Vector3 DeployLocalScale;

    private Transform _undeployedParent;
    private Vector3 _undeployedLocalPosition;
    private Vector3 _undeployedLocalRotation;
    private Vector3 _undeployedLocalScale;

    private void Awake()
    {
        _undeployedParent = ChildWeaponObject.transform.parent;
        _undeployedLocalPosition = ChildWeaponObject.transform.localPosition;
        _undeployedLocalRotation = ChildWeaponObject.transform.localRotation.eulerAngles;
        _undeployedLocalScale = ChildWeaponObject.transform.localScale;
        Undeploy();
    }

    private void FixedUpdate()
    {
        if (IsDeployed && Mathf.Max(EngagedTracker.LastAttackTime, _deployTime) + UnengagedUndeployTime < Time.time)
        {
            Undeploy();
        }
    }

    public bool CanFire()
    {
        if (IsDeployed)
        {
            return ChildWeapon.CanFire();
        }
        else
        {
            return true;
        }
    }

    public float GetDPS()
    {
        return ChildWeapon.GetDPSOrOverride();
    }

    public bool TryFire(ITarget intendedTarget)
    {
        if (IsDeployed)
        {
            return ChildWeapon.TryFire(intendedTarget);
        }
        else
        {
            return TryDeploy();
        }
    }

    private bool TryDeploy ()
    {
        if (!IsInvoking(nameof(Deploy)))
        {
            Invoke(nameof(Deploy), DeployTime);
            return true;
        }
        return false;
    }

    private void Deploy ()
    {
        ChildWeaponObject.transform.SetParent(DeployParent, true);
        ChildWeaponObject.transform.localPosition = DeployLocalPosition;
        ChildWeaponObject.transform.localEulerAngles = DeployLocalRotation;
        ChildWeaponObject.transform.localScale = DeployLocalScale;
        IsDeployed = true;
        _deployTime = Time.time;
        if (DisableBodyWhenDeployed)
        {
            Body.enabled = false;
        }
        (ChildWeapon as Behaviour).enabled = true;
        (ChildTurret as Behaviour).enabled = true;
    }

    private void Undeploy()
    {
        ChildWeaponObject.transform.SetParent(_undeployedParent, true);
        ChildWeaponObject.transform.localPosition = _undeployedLocalPosition;
        ChildWeaponObject.transform.localEulerAngles = _undeployedLocalRotation;
        ChildWeaponObject.transform.localScale = _undeployedLocalScale;
        IsDeployed = false;
        if (DisableBodyWhenDeployed)
        {
            Body.enabled = true;
        }
        (ChildWeapon as Behaviour).enabled = false;
        (ChildTurret as Behaviour).enabled = false;
    }

    public void AimTowards(Vector3 position)
    {
        if (ChildTurret != null && IsDeployed)
        {
            ChildTurret.AimTowards(position);
        }
    }

    public bool CanHit(Vector3 target)
    {
        if (ChildTurret != null && IsDeployed)
        {
            return ChildTurret.CanHit(target);
        }
        return true;
    }

    public float DeltaAngle(Vector3 target)
    {
        if (ChildTurret != null && IsDeployed)
        {
            return ChildTurret.DeltaAngle(target);
        }
        return 0f;
    }
}
