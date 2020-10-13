using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Faction
{
    private const int LayerMaskStart = 1 << 24;
    private const int LayerStart = 24;
    private const int AllFactions =
        LayerMaskStart
        | LayerMaskStart << 1
        | LayerMaskStart << 2
        | LayerMaskStart << 3
        | LayerMaskStart << 4
        | LayerMaskStart << 5
        | LayerMaskStart << 6
        | LayerMaskStart << 7;

    public int Id;
    public string Name;
    public Color Color;

    public static GameObject Instantiate(Faction faction, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject obj = Object.Instantiate(prefab, position, rotation);
        FactionObject factObj = obj.GetComponent<FactionObject>();
        factObj.SetFaction(faction);
        return obj;
    }

    public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation) => Instantiate(this, prefab, position, rotation);

    public static int GetLayerMask(int factionId) => LayerMaskStart << factionId;
    public int GetLayer() => LayerStart + Id;
    public int GetLayerMask() => GetLayerMask(Id);
    public int GetOtherLayerMasks() => AllFactions & ~GetLayerMask();
}
