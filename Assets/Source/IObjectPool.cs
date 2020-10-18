using UnityEngine;

public interface IObjectPool
{
    GameObject GetObject();
    void Dispose();
}