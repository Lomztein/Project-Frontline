using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject Prefab;
    public Vector2 SpeedVariance;
    public float CloudAreaRadius;
    public Vector3 WindVelocity;

    public float HeightVariance;
    public int SpawnChanceDenominator;

    private void Start()
    {
        WindVelocity = Random.onUnitSphere;
        WindVelocity = new Vector3(WindVelocity.x, 0f, WindVelocity.z);

        float avarageSpeed = Mathf.Lerp(SpeedVariance.x, SpeedVariance.y, 0.5f);
        float time = CloudAreaRadius * 2f / avarageSpeed;
        float spawnsPerSec = SpawnChanceDenominator * Time.fixedDeltaTime;
        int initialClouds = (int)(time / spawnsPerSec);
        
        for (int i = 0; i < initialClouds; i++)
        {
            Cloud cloud = Spawn(GetSpawnPositionAlongPath(out float remainingDist));
            cloud.End(remainingDist / cloud.Movement.magnitude);
        }
    }

    private void FixedUpdate()
    {
        if (Random.Range(0, SpawnChanceDenominator) == 0)
        {
            Spawn(GetSpawnPosition());
        }
    }

    private Vector3 GetSpawnPosition ()
    {
        Vector3 vel = WindVelocity.normalized;
        Quaternion rot = Quaternion.AngleAxis(-90f, Vector3.up);
        Vector3 offside = rot * vel;
        return transform.position - vel * CloudAreaRadius + offside * Random.Range(-CloudAreaRadius, CloudAreaRadius);
    }

    private Vector3 GetSpawnPositionAlongPath (out float remainingDist)
    {
        Vector3 start = GetSpawnPosition();
        float dist = Random.Range(0f, CloudAreaRadius * 2f);
        remainingDist = CloudAreaRadius * 2f - dist;
        return start + WindVelocity.normalized * dist;
    }

    private Cloud Spawn(Vector3 pos)
    {
        float dist = CloudAreaRadius * 2f;
        float speed = Random.Range(SpeedVariance.x, SpeedVariance.y);
        float angle = Quaternion.FromToRotation(Vector3.forward, WindVelocity.normalized).eulerAngles.y;
        float life = dist / speed;
        Cloud cloud = Instantiate(Prefab, pos, Quaternion.Euler(0f, angle, 0f)).GetComponent<Cloud>();
        cloud.Movement = WindVelocity.normalized * speed;
        cloud.End(life);
        return cloud;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, CloudAreaRadius);
        Gizmos.DrawRay(transform.position, WindVelocity);
        for (int i = 0; i < 1000; i++)
        {
            Gizmos.DrawSphere(GetSpawnPositionAlongPath(out float _), 5f);
        }
    }
}
