using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class AIController : MonoBehaviour, IController
{
    public bool Enabled { get => enabled; set => enabled = value; }

    [HideInInspector] public TeamInfo Team;

    public LayerMask TargetLayer { get; private set; }
    private TargetFinder _targetFinder = new TargetFinder();

    protected ITarget CurrentTarget { get; private set; }
    protected bool ForcedTarget { get; private set; }

    public float AcquireTargetRange;
    public float LooseTargetRange;
    public float AttackRange;

    public event Action<ITarget> OnTargetAcquired;
    public event Action<ITarget> OnTargetLost;

    public GameObject TurretObject; // TODO: Refactor these to lists.
    public ITurret Turret;

    public List<GameObject> WeaponObjects;
    public List<IWeapon> Weapons;

    public float AimTolerance;
    public bool LeadTarget;
    private Vector3 _targetLastPosition;

    private DamageModifier _primaryWeaponDamageModifier;
    private Ticker _targetFindingTicker;
    private float _aimDelta;
    public float TargetSearchFrequency = 2f;

    public AIControllerModifier[] Modifiers;

    protected virtual void Awake()
    {
        _targetFinder = new TargetFinder(go => CanEngage(go));
        _targetFindingTicker = new Ticker(1f / TargetSearchFrequency, FindNewTarget);

        if (TurretObject)
        {
            Turret = TurretObject.GetComponent<ITurret>();
        }

        Weapons = WeaponObjects.Select(x => x.GetComponent<IWeapon>()).ToList();
    }

    public void AddWeapon (IWeapon weapon)
    {
        if (weapon is Component component)
        {
            WeaponObjects.Add(component.gameObject);
        }
        Weapons.Add(weapon);
    }

    protected virtual void Start()
    {
        if (Weapons.Count > 0)
        {
            _primaryWeaponDamageModifier = Weapons[0].Modifier;
            _targetFinder.SetEvaluator((pos, go) =>
                TargetFinder.DefaultEvaluator(pos, go) +
                GetDamageFactor(go) * 1000000f);
        }

        foreach (AIControllerModifier modifier in Modifiers)
        {
            modifier.OnInitialized(this);
        }
    }

    public void SetTargetEvaluator(Func<Vector3, GameObject, float> evaluator) => _targetFinder.SetEvaluator(evaluator);
    public void AppendTargetEvaluator(Func<Vector3, GameObject, float> evaluator) => _targetFinder.AppendEvaluator(evaluator);

    public void SetTargetFilter(Predicate<GameObject> filter) => _targetFinder.SetFilter(filter);
    public void AppendTargetFilter(Predicate<GameObject> filter) => _targetFinder.AppendFilter(filter);

    public void SetTargetLayerMask(LayerMask mask) => TargetLayer = mask;

    protected float GetDamageFactor(GameObject target)
    {
        Health health = target.GetComponentInParent<Health>();
        if (health)
        {
            return _primaryWeaponDamageModifier.GetValue(health.Modifier);
        }
        return 1f;
    }


    public void FindNewTarget()
    {
        GameObject target = _targetFinder.FindTarget(transform.position, AcquireTargetRange, TargetLayer);
        if (target)
        {
            CurrentTarget = new ColliderTarget(target);
            OnTargetAcquired?.Invoke(CurrentTarget);
            _targetLastPosition = CurrentTarget.GetCenter();
        }
    }

    public void LooseTarget ()
    {
        OnTargetLost?.Invoke(CurrentTarget);
        CurrentTarget = null;
    }

    public void SetTeam(TeamInfo faction)
    {
        Team = faction;
        TargetLayer = faction.GetOtherLayerMasks();
    }

    protected Vector3 GetTargetLocalPosition() => CurrentTarget.GetCenter() - transform.position;
    protected Vector3 PositionToLocalPosition(Vector3 position) => position - transform.position;
    protected float GetTargetSquareDistance() => Vector3.SqrMagnitude(GetTargetLocalPosition());

    protected virtual void Aim()
    {
        if (Turret != null)
        {
            Vector3 targetPosition = CurrentTarget.GetCenter();
            if (LeadTarget && Weapons.Count > 0)
            {
                Vector3 vel = (targetPosition - _targetLastPosition) / Time.fixedDeltaTime;
                float dist = Vector3.Distance(targetPosition, transform.position);
                targetPosition += vel * (dist / Weapons[0].Speed + Time.fixedDeltaTime); // Add fixedDeltaTime to offset turrets always being a single tick behind.

                _targetLastPosition = CurrentTarget.GetCenter();
            }

            Turret.AimTowards(targetPosition);
            _aimDelta = Turret.DeltaAngle(CurrentTarget.GetCenter());
        }
        else
        {
            Vector3 local = GetTargetLocalPosition();
            _aimDelta = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg));
        }
    }

    public ITarget GetTarget() => CurrentTarget;

    protected virtual void Attack()
    {
        float sqrDist = GetTargetSquareDistance();
        if (sqrDist < AttackRange * AttackRange && _aimDelta <= AimTolerance)
        {
            foreach (var weapon in Weapons)
            {
                weapon.TryFire(CurrentTarget);
            }
        }
    }

    public virtual bool CanEngage(GameObject target)
    {
        bool canHit = CanHitOrNoTurret(target.transform.position);
        Unit unit = target.GetComponentInParent<Unit>();
        if (unit)
        {
            return canHit && !unit.Info.Tags.Contains("Ignore");
        }
        return canHit;
    }

    public virtual bool CanHitOrNoTurret(Vector3 position)
        => Turret == null || Turret.CanHit(position);

    public void ForceTarget (ITarget target)
    {
        SetTarget(target);
        ForcedTarget = true;
    }

    public void SetTarget(ITarget target)
    {
        CurrentTarget = target;
    }

    protected virtual void FixedUpdate()
    {
        if (CurrentTarget.ExistsAndValid())
        {
            Aim();
            Attack();

            if ((GetTargetSquareDistance() > LooseTargetRange * LooseTargetRange) && ForcedTarget != true || !CanHitOrNoTurret(CurrentTarget.GetCenter()))
            {
                CurrentTarget = null;
                FindNewTarget();
            }
        }
        else if (CurrentTarget != null)
        {
            ForcedTarget = false;
            CurrentTarget = null;
            FindNewTarget();
        }else
        {
            ForcedTarget = false;
            _targetFindingTicker.Tick();
        }
    }
}
