using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "New Match Settings", menuName = "Match Settings")]
public class MatchSettings : ScriptableObject
{
    public const string PATH_TO_DEFAULT = "Match Settings/Default";
    public BattlefieldInfo BattlefieldInfo;

    public static MatchSettings Current;

    private void OnEnable()
    {
        Current = Default();
    }

    public static void Reset()
    {
        Current = Default();
    }

    public static MatchSettings Default () 
        => Resources.Load<MatchSettings>(PATH_TO_DEFAULT);

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

    [System.Serializable]
    public class PlayerInfo
    {
        public string Name;

        public Faction Faction;
        public Team Team;

        public Vector2 Position;
        public int StartingCredits;

        public AIPlayerProfile AIProfile;
        public bool IsPlayer => AIProfile == null;
    }
}
