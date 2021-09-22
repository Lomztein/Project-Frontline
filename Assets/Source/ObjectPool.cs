using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPool : IObjectPool
{
    private static List<ObjectPool> _freePools = new List<ObjectPool>();
    private static Transform _globalParent;

    public GameObject Prefab;
    private List<IPoolObject> _objects = new List<IPoolObject>();

    public ObjectPool(GameObject prefab)
    {
        Prefab = prefab;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void OnStartUp ()
    {
        SceneManager.sceneUnloaded += SceneUnloaded;
    }

    private static void SceneUnloaded(Scene arg0)
    {
        DisposePools();
    }

    private Transform GetGlobalParent ()
    {
        if (!_globalParent)
        {
            _globalParent = new GameObject("_PoolGlobalParent").transform;
        }
        return _globalParent.transform;
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        IPoolObject pobj = null;

        foreach (var obj in _objects)
        {
            if (obj.IsAvailable)
            {
                pobj = obj;
                break;
            }
        }

        if (pobj == null)
        {
            pobj = Object.Instantiate(Prefab, GetGlobalParent()).GetComponent<IPoolObject>();
            pobj.OnInstantiated();
            _objects.Add(pobj);
        }

        GameObject go = (pobj as Component).gameObject;
        go.transform.position = position;
        go.transform.rotation = rotation;

        pobj.OnEnabled();
        return pobj.GameObject;
    }

    public void Dispose ()
    {
        foreach (var obj in _objects)
        {
            if (obj as Object != null)
            {
                obj.Dispose();
            }
        }
        _objects.Clear();
    }

    public static ObjectPool GetPool(GameObject prefab)
    {
        ObjectPool p = null;
        foreach (var pool in _freePools)
        {
            if (pool.Prefab == prefab)
            {
                p = pool;
                break;
            }
        }

        if (p != null)
        {
            //_freePools.Remove(p);
            return p;
        }
        else
        {
            p = new ObjectPool(prefab);
            _freePools.Add(p);
            return p;
        }
    }

    public static void FreePool (ObjectPool pool)
    {
        if (Application.isPlaying)
        {
            //_freePools.Add(pool);
        }
    }

    public static void DisposePools()
    {
        foreach (var pool in _freePools)
        {
            pool.Dispose();
        }
        _freePools.Clear();
        if (_globalParent)
        {
            Object.Destroy(_globalParent.gameObject);
        }
    }
}
