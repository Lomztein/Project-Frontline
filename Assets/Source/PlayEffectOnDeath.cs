using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffectOnDeath : MonoBehaviour
{
    public Health Health;
    public Effect Effect;

    private void Awake()
    {
        Health.OnDeath += Health_OnDeath;
    }

    private void Health_OnDeath(Health obj)
    {
        Effect.Play();
    }
}
