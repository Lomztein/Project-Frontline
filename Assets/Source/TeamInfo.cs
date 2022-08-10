using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Team", menuName = "Team")]
public class TeamInfo : ScriptableObject
{
    private const int LayerStart = 16;
    private const int LayerMaskStart = 1 << LayerStart;
    public const int LayerAllTeams =
        LayerMaskStart
        | LayerMaskStart << 1
        | LayerMaskStart << 2
        | LayerMaskStart << 3
        | LayerMaskStart << 4
        | LayerMaskStart << 5
        | LayerMaskStart << 6
        | LayerMaskStart << 7;

    private const int ProjectileLayerStart = 24;
    private const int ProjectileLayerMaskStart = 1 << ProjectileLayerStart;
    public const int ProjectileLayerAllTeams =
    ProjectileLayerMaskStart
    | ProjectileLayerMaskStart << 1
    | ProjectileLayerMaskStart << 2
    | ProjectileLayerMaskStart << 3
    | ProjectileLayerMaskStart << 4
    | ProjectileLayerMaskStart << 5
    | ProjectileLayerMaskStart << 6
    | ProjectileLayerMaskStart << 7;

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
    public static int Invert(int mask) => LayerAllTeams & ~mask;

    public int GetLayer() => LayerStart + Id;
    public int GetLayerMask() => GetLayerMask(Id);
    public int GetOtherLayerMasks() => LayerAllTeams & ~GetLayerMask();

    public static int ProjectileGetLayerMask(int factionId) => ProjectileLayerMaskStart << factionId;
    public static int ProjectileInvert(int mask) => ProjectileLayerAllTeams & ~mask;

    public int ProjectileGetLayer() => ProjectileLayerStart + Id;
    public int ProjectileGetLayerMask() => ProjectileGetLayerMask(Id);
    public int ProjectileGetOtherLayerMasks() => ProjectileLayerAllTeams & ~ProjectileGetLayerMask();
}
