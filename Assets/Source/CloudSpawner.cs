using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject Prefab;
    public Vector3 SizeVariance;
    public Vector2 SpeedVariance;
    public float CloudAreaRadius;
    public Vector3 WindVelocity;
    public int InitialClouds;
    public float HeightVariance;

    public int SpawnChanceDenominator;

    private void Start()
    {
        for (int i = 0; i < InitialClouds; i++)
        {
            Cloud cloud = Spawn();
            float dist = Random.Range(0f, CloudAreaRadius * 2f);
            cloud.transform.Translate(new Vector3(0f, 0f, dist));
            cloud.End((CloudAreaRadius * 2f - dist) / cloud.Movement.magnitude);
        }
    }

    private void FixedUpdate()
    {
        if (Random.Range(0, SpawnChanceDenominator) == 0)
        {
            Spawn();
        }
    }

    private Vector3 GetSpawnPosition ()
    {
        float angle = Mathf.Atan2(WindVelocity.y, WindVelocity.x) * Mathf.Rad2Deg + 90f;
        Vector3 offside = Quaternion.Euler(0f, angle, 0f) * WindVelocity;
        return transform.position - WindVelocity * CloudAreaRadius + offside * Random.Range(-CloudAreaRadius, CloudAreaRadius) + Vector3.up * Random.Range(-HeightVariance, HeightVariance);
    }

    private Cloud Spawn()
    {
        float dist = CloudAreaRadius * 2f;
        float speed = Random.Range(SpeedVariance.x, SpeedVariance.y);
        float angle = Mathf.Atan2(WindVelocity.y, WindVelocity.x) * Mathf.Rad2Deg + 180f;
        float life = dist / speed;
        Cloud cloud = Instantiate(Prefab, GetSpawnPosition(), Quaternion.Euler(0f, 0f, angle)).GetComponent<Cloud>();
        cloud.Movement = WindVelocity.normalized * speed;
        cloud.End(life);
        return cloud;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, CloudAreaRadius);
        Gizmos.DrawRay(transform.position, WindVelocity);
    }
}
