using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldProjector : MonoBehaviour
{
    public float ShieldSize;
    public Transform ShieldTransform;
    public Renderer ShieldRenderer;
    public Renderer InsideRenderer;
    public Health ShieldHealth;
    public float ShieldSizeMultiplier;

    private Material _shieldMaterial;
    public Vector2 ShieldMaterialStrenghtMinMax;

    private float _lastDamageTime;
    public float DamageFlashTime;
    public float DamageFlashAmount;

    public float HealDelay;
    public float HealRate;
    public float ReviveTime;
    public float ShieldLerpTime;

    private float _shieldTargetSize;

    private void Start()
    {
        _shieldMaterial = Instantiate(ShieldRenderer.material);
        ShieldRenderer.material = _shieldMaterial;
        InsideRenderer.material = _shieldMaterial;

        ShieldHealth.OnTakeDamage += ShieldHealth_OnTakeDamage;
        ShieldHealth.OnDeath += ShieldHealth_OnDeath;

        ReviveShield();
    }

    private void ShieldHealth_OnDeath()
    {
        _shieldTargetSize = 0f;
        Invoke(nameof(ReviveShield), ReviveTime);
    }

    private void ReviveShield ()
    {
        ShieldHealth.Revive();
        _shieldTargetSize = ShieldSize;
    }

    private void ShieldHealth_OnTakeDamage(DamageInfo obj)
    {
        _lastDamageTime = Time.time;
    }

    private void FixedUpdate()
    {
        float since = Time.time - _lastDamageTime;
        float factor = 1 - Mathf.Clamp01(since / DamageFlashTime);
        float damageFlashFactor = Mathf.Lerp(0f, DamageFlashAmount, factor);

        _shieldMaterial.SetFloat("Strength", Mathf.Lerp(ShieldMaterialStrenghtMinMax.x, ShieldMaterialStrenghtMinMax.y, ShieldHealth.CurrentHealth / ShieldHealth.MaxHealth) + damageFlashFactor);
        ShieldTransform.localScale = Vector3.Lerp(ShieldTransform.localScale, Vector3.one * _shieldTargetSize * ShieldSizeMultiplier, ShieldLerpTime * Time.fixedDeltaTime);
        if (Time.time > _lastDamageTime + HealDelay)
        {
            ShieldHealth.Heal(HealRate * Time.fixedDeltaTime);
        }
    }
}
