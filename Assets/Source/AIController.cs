using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIController : MonoBehaviour, IController
{
    public bool Enabled { get => enabled; set => enabled = value; }

    [HideInInspector] public Faction Faction;
    public IControllable Controllable;

    private LayerMask _targetLayer;
    private TargetFinder _targetFinder = new TargetFinder();

    protected ITarget CurrentTarget { get; private set; }

    public float AcquireTargetRange;
    public float LooseTargetRange;
    public float AttackRange;

    public event Action<ITarget> OnTargetAcquired;
    public event Action<ITarget> OnTargetLost;

    public GameObject TurretObject;
    public ITurret Turret;

    public GameObject WeaponObject;
    public IWeapon Weapon;

    public float AimTolerance;

    private DamageMatrix.Damage _primaryWeaponDamageType;
    private Ticker _targetFindingTicker;
    private float _aimDelta;

    public AIControllerModifier[] Modifiers;

    protected virtual void Awake()
    {
        _targetFinder = new TargetFinder(go => CanEngage(go));
        _targetFindingTicker = new Ticker(0.25f, FindNewTarget);

        Controllable = GetComponentInChildren<IControllable>();
        if (TurretObject)
        {
            Turret = TurretObject.GetComponent<ITurret>();
        }
        if (WeaponObject)
        {
            Weapon = WeaponObject.GetComponent<IWeapon>();
        }
    }

    protected virtual void Start()
    {
        if (Weapon != null)
        {
            _primaryWeaponDamageType = Weapon.DamageType;
            _targetFinder.SetEvaluator((pos, go) =>
                TargetFinder.DefaultEvaluator(pos, go) +
                GetDamageFactor(go) * 10000000f);
        }

        foreach (AIControllerModifier modifier in Modifiers)
        {
            modifier.OnInitialized();
        }
    }

    public void SetTargetEvaluator(Func<Vector3, GameObject, float> evaluator) => _targetFinder.SetEvaluator(evaluator);
    public void SetTargetFilter(Predicate<GameObject> filter) => _targetFinder.SetFilter(filter);
    public void SetTargetLayerMask(LayerMask mask) => _targetLayer = mask;

    private float GetDamageFactor(GameObject target)
    {
        Health health = target.GetComponentInParent<Health>();
        if (health)
        {
            return DamageMatrix.GetDamageFactor(_primaryWeaponDamageType, health.ArmorType);
        }
        return 1f;
    }


    public void FindNewTarget()
    {
        GameObject target = _targetFinder.FindTarget(transform.position, AcquireTargetRange, _targetLayer);
        if (target)
        {
            CurrentTarget = new ColliderTarget(target);
            OnTargetAcquired?.Invoke(CurrentTarget);
        }
    }

    public void LooseTarget ()
    {
        OnTargetLost?.Invoke(CurrentTarget);
        CurrentTarget = null;
    }

    public void SetFaction(Faction faction)
    {
        Faction = faction;
        _targetLayer = faction.GetOtherLayerMasks();
    }

    protected Vector3 GetTargetLocalPosition() => CurrentTarget.GetPosition() - transform.position;
    protected float GetTargetSquareDistance() => Vector3.SqrMagnitude(GetTargetLocalPosition());

    protected virtual void Aim()
    {
        if (Turret != null)
        {
            Turret.AimTowards(CurrentTarget.GetPosition());
            _aimDelta = Turret.DeltaAngle(CurrentTarget.GetPosition());
        }
        else
        {
            Vector3 local = GetTargetLocalPosition();
            _aimDelta = Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg);
        }
    }

    protected virtual void Attack()
    {
        if (Weapon != null)
        {
            float sqrDist = GetTargetSquareDistance();
            if (sqrDist < AttackRange * AttackRange && _aimDelta <= AimTolerance)
            {
                Weapon.TryFire(CurrentTarget);
            }
        }
    }

    public virtual bool CanEngage(GameObject target)
        => Turret != null ? Turret.CanHit(target.transform.position) : true;

    protected virtual void FixedUpdate()
    {
        if (CurrentTarget.ExistsAndValid())
        {
            Aim();
            Attack();

            if (GetTargetSquareDistance() > LooseTargetRange * LooseTargetRange)
            {
                CurrentTarget = null;
            }
        }
        else
        {
            if (CurrentTarget != null && !CurrentTarget.IsValid())
            {
                FindNewTarget();
            }

            _targetFindingTicker.Tick();
        }
    }
}
