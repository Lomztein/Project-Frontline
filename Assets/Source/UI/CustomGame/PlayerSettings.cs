using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CustomGame
{
    public class PlayerSettings : MonoBehaviour
    {
        public InputField Name;
        public Dropdown Faction;
        public Dropdown Type;
        public Dropdown Spawn;
        public Button Units;
        public InputField Credits;
        public InputField Handicap;
        public Dropdown Team;

        public Dictionary<GameObject, bool> UnitAvailable = new Dictionary<GameObject, bool>();

        private void Awake()
        {
            MatchSettings.OnUpdated += MatchSettings_OnUpdated;
            MatchSettings_OnUpdated(MatchSettings.Current);

            Faction.options = GetFactions().Select(x => new Dropdown.OptionData(x.Name)).ToList();
            Type.options.Add(new Dropdown.OptionData("Meatbag (You)"));
            Type.options.Add(new Dropdown.OptionData("Any AI"));
            Type.options.AddRange(GetAIProfiles().Select(x => new Dropdown.OptionData(x.Name)));
            Team.options = GetTeams().Select(x => new Dropdown.OptionData(x.Name)).ToList();
        }

        private void OnDestroy()
        {
            MatchSettings.OnUpdated -= MatchSettings_OnUpdated;
        }

        private void MatchSettings_OnUpdated(MatchSettings obj)
        {
            Spawn.options = Enumerable.Range(0, obj.MapInfo.Shape.Spawns).Select(x => new Dropdown.OptionData(x.ToString())).ToList();
        }

        private IEnumerable<Faction> GetFactions ()
            => Resources.LoadAll<Faction>("Factions");
        private IEnumerable<AIPlayerProfile> GetAIProfiles ()
            => Resources.LoadAll<AIPlayerProfile>("AIProfiles");
        private IEnumerable<TeamInfo> GetTeams()
            => Resources.LoadAll<TeamInfo>("Teams");

        public void Remove()
        {
            Destroy(gameObject);
        }

        public Faction GetFaction() => GetFactions().ElementAt(Faction.value);

        public MatchSettings.PlayerInfo CreatePlayerInfo ()
        {
            var playerInfo = new MatchSettings.PlayerInfo ();
            playerInfo.Name = Name.text;
            playerInfo.Team = GetTeams().ElementAt(Team.value);
            if (Type.value == 1) playerInfo.AIProfile = GetRandomAIProfile();
            if (Type.value > 1) playerInfo.AIProfile = GetAIProfiles().ElementAt(Type.value - 2);
            playerInfo.SpawnIndex = Spawn.value;
            playerInfo.Faction = GetFaction();
            playerInfo.StartingCredits = int.Parse(Credits.text);
            playerInfo.Handicap = float.Parse(Handicap.text);
            playerInfo.UnitAvailable = UnitAvailable;
            foreach (var unit in playerInfo.Faction.LoadUnits())
            {
                if (!playerInfo.UnitAvailable.ContainsKey(unit))
                {
                    playerInfo.UnitAvailable.Add(unit, true);
                }
            }
            return playerInfo;
        }

        public void ApplyPlayerInfo(MatchSettings.PlayerInfo info)
        {
            Name.text = info.Name;
            Team.value = GetTeams().ToList().IndexOf(info.Team);
            Faction.value = GetFactions().ToList().IndexOf(info.Faction);
            Spawn.value = info.SpawnIndex;
            if (info.AIProfile != null)
            {
                int val = GetAIProfiles().ToList().FindIndex(x => x.Name == info.AIProfile.Name) + 2;
                Type.value = val;
            }
            Credits.text = info.StartingCredits.ToString();
            Handicap.text = info.Handicap.ToString();
        }

        private AIPlayerProfile GetRandomAIProfile()
        {
            var profiles = GetAIProfiles().ToArray();
            return profiles[UnityEngine.Random.Range(0, profiles.Length)];
        }
    }
}