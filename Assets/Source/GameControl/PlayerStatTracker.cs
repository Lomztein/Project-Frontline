using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerStatTracker : MonoBehaviour
{
    public PlayerStats[] Stats { get; private set; }
    private Dictionary<uint, PlayerStats> _playerIdStatsMap = new Dictionary<uint, PlayerStats>();

    public void BeginTracking(Commander[] commanders)
    {
        Stats = new PlayerStats[commanders.Length];
        for (int i = 0; i < commanders.Length; i++)
        {
            if (commanders[i] != null)
            {
                Stats[i] = new PlayerStats();
                Stats[i].Player = commanders[i].PlayerInfo;

                _playerIdStatsMap.Add(commanders[i].PlayerInfo.Id, Stats[i]);
                BeginTracking(commanders[i]);
            }
        }
    }

    public void EndTracking(Commander[] commanders)
    {
        Assert.AreEqual(commanders.Length, commanders.Length);
        for (int i = 0; i < commanders.Length; i++)
        {
            if (commanders[i] != null)
            {
                EndTracking(commanders[i]);
            }
        }
    }

    public PlayerStats[] GetStatsFor(PlayerInfo[] infos)
        => infos.Select(x => GetStats(x)).ToArray();

    private void BeginTracking(Commander commander)
    {
        commander.OnUnitPlaced += Commander_OnUnitPlaced;
        commander.OnUnitSpawned += Commander_OnUnitSpawned;
        commander.OnPlacedUnitDeath += Commander_OnPlacedUnitDeath;
    }

    private void EndTracking(Commander commander)
    {
        commander.OnUnitPlaced -= Commander_OnUnitPlaced;
        commander.OnUnitSpawned -= Commander_OnUnitSpawned;
        commander.OnPlacedUnitDeath -= Commander_OnPlacedUnitDeath;
    }

    private void Commander_OnPlacedUnitDeath(Commander arg1, Unit arg2)
    {
        GetPlayerStats(arg1).MutateStat("Placed unit deaths", x => x.Value++);
    }

    private void Commander_OnUnitSpawned(Commander arg1, UnitFactory arg2, Unit arg3)
    {
        GetPlayerStats(arg1).MutateStat("Units produced", x => x.Value++);
    }

    private void Commander_OnUnitPlaced(Commander arg1, Unit arg2)
    {
        GetPlayerStats(arg1).MutateStat("Units placed", x => x.Value++);
    }
    private PlayerStats GetPlayerStats(Commander commander)
        => _playerIdStatsMap[commander.PlayerInfo.Id];

    private Commander GetCommander(PlayerInfo playerInfo)
        => Team.AllCommanders.FirstOrDefault(x => x.PlayerInfo.Id == playerInfo.Id);

    // This chain of mappings feels like a sin, somehow.
    public PlayerStats GetStats(PlayerInfo playerInfo)
        => GetPlayerStats(GetCommander(playerInfo));
}