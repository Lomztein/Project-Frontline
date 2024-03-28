using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public string Name;
    public uint Id;

    public Faction Faction;
    public TeamInfo Team;
    public Dictionary<GameObject, bool> UnitAvailable = new Dictionary<GameObject, bool>();
    public bool IsObserver => Faction == null;

    public int SpawnIndex { get => IsObserver ? -1 : _spawnIndex; set => _spawnIndex = value; }
    [SerializeField] private int _spawnIndex;

    public int StartingCredits;
    public float Handicap;

    public AIPlayerProfile AIProfile;
    public bool HasAI => AIProfile != null;

    public bool IsPlayer => PlayerInputDeviceId != -1;
    public PlayerHandler.InputType PlayerInputType = PlayerHandler.InputType.MouseAndKeyboard;
    public int PlayerInputDeviceId = -1;

    public string GenerateDefaultName()
        => "{Type} of {Faction}";

    public override string ToString()
    {
        return Name;
    }
}
