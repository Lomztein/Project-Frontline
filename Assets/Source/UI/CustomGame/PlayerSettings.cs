using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Util;

namespace CustomGame
{
    public class PlayerSettings : MonoBehaviour
    {
        public InputField Name;
        public Dropdown Faction;
        public Dropdown Player;
        public Dropdown Type;
        public Dropdown Spawn;
        public Button Units;
        public InputField Credits;
        public Dropdown Difficulty;
        public Dropdown Team;

        public Dictionary<GameObject, bool> UnitAvailable = new Dictionary<GameObject, bool>();

        private void Awake()
        {
            MatchSettings.OnUpdated += MatchSettings_OnUpdated;
            MatchSettings_OnUpdated(MatchSettings.Current);

            Faction.options.Add(new Dropdown.OptionData("Random Faction"));
            Faction.options.AddRange(GetFactions().Select(x => new Dropdown.OptionData(x.Name)).ToList());
            Faction.options.Add(new Dropdown.OptionData("Observer"));

            Player.options.Add(new Dropdown.OptionData("AI Player"));
            Player.options.Add(new Dropdown.OptionData("Mouse + KB"));
            Player.options.AddRange(UnityUtils.GetGamepads().Select(x => new Dropdown.OptionData($"Gamepad {GetPlayerDropdownValue(x.deviceId) - 1}")));

            Type.options.Add(new Dropdown.OptionData("No AI"));
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

        public Faction GetFaction()
        {
            var factions = GetFactions().ToArray();
            if (Faction.value == factions.Length + 1)
            {
                return null;
            }
            return factions[Faction.value - 1];
        }

        public int GetInputDeviceId()
        {
            if (Player.value == 1) return 0;
            if (Player.value < 2) return -1;
            int value = Player.value - 2;
            return UnityUtils.GetGamepads()[value].deviceId;
        }

        public int GetPlayerDropdownValue(int deviceId)
        {
            var gamepads = UnityUtils.GetGamepads();

            if (deviceId == -1) return 0;
            if (deviceId == 0) return 1;
            return Array.IndexOf(gamepads, gamepads.First(x => x.deviceId == deviceId)) + 2;
        }

        public PlayerHandler.InputType GetInputType()
        {
            var value = (PlayerHandler.InputType)(Player.value - 1);
            return value;
        }

        public MatchSettings.PlayerInfo CreatePlayerInfo ()
        {
            var playerInfo = new MatchSettings.PlayerInfo ();
            playerInfo.Name = Name.text;
            playerInfo.Team = GetTeams().ElementAt(Team.value);
            if (Type.value == 0) playerInfo.AIProfile = null;
            if (Type.value == 1) playerInfo.AIProfile = GetRandomAIProfile();
            if (Type.value > 1) playerInfo.AIProfile = GetAIProfiles().ElementAt(Type.value - 2);
            playerInfo.SpawnIndex = Spawn.value;
            playerInfo.Faction = GetFaction();
            playerInfo.PlayerInputType = GetInputType();
            playerInfo.PlayerInputDeviceId = GetInputDeviceId();
            playerInfo.StartingCredits = int.Parse(Credits.text);
            playerInfo.UnitAvailable = UnitAvailable;
            if (playerInfo.Faction)
            {
                foreach (var unit in playerInfo.Faction.LoadUnits())
                {
                    if (!playerInfo.UnitAvailable.ContainsKey(unit))
                    {
                        playerInfo.UnitAvailable.Add(unit, true);
                    }
                }
            }
            return playerInfo;
        }

        public void ApplyPlayerInfo(MatchSettings.PlayerInfo info)
        {
            Name.text = info.Name;
            Team.value = GetTeams().ToList().IndexOf(info.Team);
            Faction.value = GetFactions().ToList().IndexOf(info.Faction) + 1;
            Player.value = GetPlayerDropdownValue(info.PlayerInputDeviceId);
            Spawn.value = info.SpawnIndex;
            if (info.AIProfile != null)
            {
                int val = GetAIProfiles().ToList().FindIndex(x => x.Name == info.AIProfile.Name) + 2;
                Type.value = val;
            }
            Credits.text = info.StartingCredits.ToString();
        }

        private AIPlayerProfile GetRandomAIProfile()
        {
            var profiles = GetAIProfiles().ToArray();
            return profiles[UnityEngine.Random.Range(0, profiles.Length)];
        }
    }
}