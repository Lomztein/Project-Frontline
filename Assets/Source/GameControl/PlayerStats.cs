using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    public PlayerInfo Player;
    private Dictionary<string, Stat> _stats = new Dictionary<string, Stat>();

    public void MutateStat(string name, Action<Stat> action)
    {
        if (!_stats.ContainsKey(name))
        {
            _stats.Add(name, new Stat(name));
        }
        action(_stats[name]);
    }

    public Stat GetStat(string name) =>
        _stats.GetValueOrDefault(name);

    public class Stat
    {
        public string Name;
        public float Value;

        public Stat(string name)
        {
            this.Name = name;
        }

        public int ToInt()
            => (int)Value;
    }
}
