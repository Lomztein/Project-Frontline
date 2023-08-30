using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "New Match Settings", menuName = "Match Settings")]
public class MatchSettings : ScriptableObject
{
    public const string PATH_TO_DEFAULT = "Match Settings/Default";
    public MapInfo MapInfo;

    private static MatchSettings _current;
    public static MatchSettings Current { get => GetCurrent(); set => _current = value; }

    [SerializeField]
    private List<Mutator> _mutators;
    public IEnumerable<Mutator> Mutators => _mutators;

    public void AddMutator(Mutator mutator) => _mutators.Add(mutator);
    public void RemoveMutator(Mutator mutator) => _mutators.Remove(mutator);
    public void ClearMutators() => _mutators = new List<Mutator>();

    public bool SupportsUnitType(UnitInfo.Type unitType)
    {
        if (unitType == UnitInfo.Type.Naval) return false;
        return true;
    }

    public static MatchSettings GetCurrent ()
    {
        if (_current == null)
            _current = Default();
        return _current;
    }

    public static MatchSettings Default ()
    {
        var settings = Instantiate(Resources.Load<MatchSettings>(PATH_TO_DEFAULT));
        foreach (var player in settings.Players)
        {
            player.Name = NameGenerator.GenerateName();
        }
        return settings;
    }

    [SerializeField]
    private List<PlayerInfo> _players = new List<PlayerInfo>();
    public PlayerInfo[] Players => _players.ToArray();

    public void AddPlayer(PlayerInfo info)
    {
        if (info.IsPlayer == true)
        {
            // Assert that there isn't already a profile for the actual player.
            Assert.IsNull(_players.FirstOrDefault(x => x.IsPlayer == true));
        }
        _players.Add(info);
    }

    public void RemovePlayer(PlayerInfo info) => _players.Remove(info);
    public void ClearPlayers() => _players.Clear();

    [System.Serializable]
    public class PlayerInfo
    {
        public string Name;

        public Faction Faction;
        public TeamInfo Team;
        public Dictionary<GameObject, bool> UnitAvailable = new Dictionary<GameObject, bool>();

        public Vector2 Position;
        public int StartingCredits;
        public float Handicap;

        public AIPlayerProfile AIProfile;
        public bool IsPlayer => AIProfile == null;
    }
}
