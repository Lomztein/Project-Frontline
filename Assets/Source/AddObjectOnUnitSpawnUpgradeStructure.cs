using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AddObjectOnUnitSpawnUpgradeStructure : ChanceOnUnitSpawnUpgradeStructure
{
    public GameObject Object;
    public UnitTransformInfo[] UnitTransformInfos;
    public bool AddWeaponToAI;

    private Dictionary<string, UnitTransformInfo> _infoCache;

    protected override void ApplyUpgrade(Unit target)
    {
        if (_infoCache == null) BakePathCache(UnitTransformInfos);

        (Transform parent, Vector3 localPos, Vector3 localRot) = FindUnitTransform(target);
        if (parent != null)
        {
            GameObject newObject = Instantiate(Object, parent, false);
            newObject.transform.localPosition = localPos;
            newObject.transform.localRotation = Quaternion.Euler(localRot);
            _commander.TeamInfo.ApplyTeam(newObject);
            _commander.AssignCommander(newObject);

            IWeapon wep = newObject.GetComponentInChildren<IWeapon>();
            if (wep != null)
            {
                target.GetComponent<Unit>().AddWeapon(wep);
                if (AddWeaponToAI)
                {
                    target.GetComponent<AIController>().AddWeapon(newObject.GetComponentInChildren<IWeapon>());
                }
            }
        }
    }

    private void BakePathCache (UnitTransformInfo[] infos)
    {
        _infoCache = new Dictionary<string, UnitTransformInfo>();
        foreach (var info in infos)
        {
            if (string.IsNullOrEmpty(info.UnitIdentifier))
            {
                _infoCache.Add("", info);
            }
            else
            {
                _infoCache.Add(info.UnitIdentifier, info);
            }
        }
    }

    private UnitTransformInfo FindUnitTransformInfo (Unit target)
    {
        string identifier = target.Info.Identifier;
        if (string.IsNullOrEmpty(identifier))
        {
            return _infoCache[""];
        }else if (_infoCache.TryGetValue(identifier, out UnitTransformInfo value))
        {
            return _infoCache[identifier];
        }
        return _infoCache[""];
    }

    private (Transform parent, Vector3 localPos, Vector3 localRot) FindUnitTransform (Unit target)
    {
        UnitTransformInfo info = FindUnitTransformInfo(target);
        if (string.IsNullOrEmpty(info.Path))
        {
            return (target.transform, info.LocalPosition, info.LocalRotation);
        }
        return (target.transform.Find(info.Path), info.LocalPosition, info.LocalRotation);
    }

    [System.Serializable]
    public class UnitTransformInfo
    {
        public string UnitIdentifier;
        public string Path;
        public Vector3 LocalPosition;
        public Vector3 LocalRotation;
    }
}
