using UnityEngine;

public interface IObjectPool
{
    GameObject GetObject(Vector3 position, Quaternion rotation);
    void Dispose();
}