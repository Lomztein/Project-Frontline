using UnityEngine;

public interface ITurret
{
    void AimTowards(Vector3 position);
    bool CanHit(Vector3 target);
    float DeltaAngle(Vector3 target);
}