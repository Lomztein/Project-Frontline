using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour, ITeamComponent
{
    public LayerMask TargetLayer;

    public float TriggerRange;
    public float JumpDelay;
    public float JumpVelocity;
    public float ExplodeDelay;

    public float ExplosionRange;
    public float ExplosionDamage;
    public DamageMatrix.Damage ExplosionDamageType;

    public Effect TriggerEffect;
    public Effect ExplodeEffect;
    public Collider Collider;

    public Vector3 Gravity = new Vector3(0f, -9.81f, 0f);
    private bool _flying;

    private Vector3 _velocity;
    private Vector3 _angularVelocity;
    private Ticker _ticker;

    public float EffectDestroyDelay;

    private void Start()
    {
        _ticker = new Ticker(20, Check);
    }

    public void SetTeam(TeamInfo team)
    {
        TargetLayer = team.GetOtherLayerMasks();
        gameObject.layer = team.ProjectileGetLayer();
    }

    private void Check ()
    {
        if (!_flying && Physics.CheckSphere(transform.position, TriggerRange, TargetLayer))
        {
            StartCoroutine(JumpAndExplode());
        }
    }

    private void FixedUpdate()
    {
        if (_flying)
        {
            _velocity += Gravity * Time.fixedDeltaTime;
            transform.position += _velocity * Time.fixedDeltaTime;
            transform.Rotate(_angularVelocity * Time.fixedDeltaTime);
        }
        else
        {
            _ticker.Tick();
        }
    }

    private IEnumerator JumpAndExplode ()
    {
        yield return new WaitForSeconds(JumpDelay);
        Collider.enabled = true;
        TriggerEffect.Play();

        _velocity += Vector3.up * JumpVelocity;
        _angularVelocity = new Vector3(Random.Range(-60, 60), Random.Range(-10, 10), Random.Range(-60, 60));
        _flying = true;

        yield return new WaitForSeconds(ExplodeDelay);
        Explode();
    }

    private void Explode ()
    {
        ExplodeEffect.Play();
        var hits = Physics.OverlapSphere(transform.position, ExplosionRange, TargetLayer);
        foreach (var hit in hits)
        {
            var health = hit.GetComponentInParent<Health>();
            health.TakeDamage(new DamageInfo(ExplosionDamage, ExplosionDamageType, hit.transform.position, (hit.transform.position - transform.position).normalized));
        }
        TriggerEffect.transform.SetParent(null);
        ExplodeEffect.transform.SetParent(null);

        Destroy(gameObject);
        Destroy(TriggerEffect.gameObject, EffectDestroyDelay);
        Destroy(ExplodeEffect.gameObject, EffectDestroyDelay);
    }
}
