using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


[CreateAssetMenu(fileName = "New Match Settings", menuName = "Match Settings")]
public class MatchSetup : ScriptableObject
{
    public const string PATH_TO_DEFAULT = "Match Settings/Default";
    private static uint _playerIndex;
    public MapInfo MapInfo;

    private static MatchSetup _current;
    public static MatchSetup Current { get => GetCurrent(); set => _current = value; }

    public static event Action<MatchSetup> OnUpdated;
    public static void NotifyUpdate(MatchSetup matchSettings) 
        => OnUpdated?.Invoke(matchSettings);

    [SerializeField]
    private List<Mutator> _mutators;
    public IEnumerable<Mutator> Mutators => _mutators;

    public UnitProductionBehaviour ProductionBehaviour;
    public VictoryChecker VictoryChecker;
    public DayNightCycle.DayNightBehaviour DayNightBehaviour;

    public void AddMutator(Mutator mutator) => _mutators.Add(mutator);
    public void RemoveMutator(Mutator mutator) => _mutators.Remove(mutator);
    public void ClearMutators() => _mutators = new List<Mutator>();

    public bool SupportsUnitType(UnitInfo.Type unitType)
    {
        if (unitType == UnitInfo.Type.Naval) return false;
        return true;
    }

    public static MatchSetup GetCurrent ()
    {
        if (_current == null)
            _current = Default();
        return _current;
    }

    public static MatchSetup Default ()
    {
        var settings = Instantiate(Resources.Load<MatchSetup>(PATH_TO_DEFAULT));
        foreach (var player in settings.Players)
        {
            player.Name = player.GenerateDefaultName();
        }
        return settings;
    }

    [SerializeField]
    private List<PlayerInfo> _players = new List<PlayerInfo>();
    public PlayerInfo[] Players => _players.ToArray();
    
    public void AddPlayer(PlayerInfo info)
    {
        info.Id = _playerIndex++;
        _players.Add(info);
    }

    public void RemovePlayer(PlayerInfo info) => _players.Remove(info);
    public void ClearPlayers() => _players.Clear();


}
