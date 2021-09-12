using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Faction", menuName = "Faction")]
public class TeamInfo : ScriptableObject
{
    private const int LayerMaskStart = 1 << 24;
    private const int LayerStart = 24;
    private const int AllTeams =
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

    public Texture2D Palette;
    [SerializeField] private Material _baseMaterial;

    private Material _teamMaterialCache;
    public Material TeamMaterial => GetTeamMaterial ();

    private Material GetTeamMaterial()
    {
        if (_teamMaterialCache == null)
        {
            _teamMaterialCache = Instantiate(_baseMaterial);
            _teamMaterialCache.mainTexture = Palette;
        }
        return _teamMaterialCache;
    }

    public static GameObject Instantiate(TeamInfo faction, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject obj = Object.Instantiate(prefab, position, rotation);
        ApplyTeam(obj, faction);
        return obj;
    }

    public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation) => Instantiate(this, prefab, position, rotation);

    public void ApplyTeam(GameObject go) => ApplyTeam(go, this);

    public static void ApplyTeam(GameObject obj, TeamInfo faction)
    {
        foreach (Transform transform in obj.GetComponentsInChildren<Transform>())
        {
            transform.gameObject.layer = faction.GetLayer();
        }

        var components = obj.GetComponentsInChildren<ITeamComponent>();
        foreach (var component in components)
        {
            component.SetTeam(faction);
        }
    }


    public static int GetLayerMask(int factionId) => LayerMaskStart << factionId;
    public static int Invert(int mask) => AllTeams & ~mask;
    public int GetLayer() => LayerStart + Id;
    public int GetLayerMask() => GetLayerMask(Id);
    public int GetOtherLayerMasks() => AllTeams & ~GetLayerMask();
}
