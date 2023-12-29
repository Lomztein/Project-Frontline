using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace CustomGame
{
    public class GameSettings : MonoBehaviour
    {
        public MapSettings Map;
        public UnitSelector UnitSelector;

        public GameObject PlayerSettingsPrefab;
        public Transform PlayerSettingsParent;
        public GameObject PlayerCountWarning;
        public int PlayerCountWarningThreshold = 4;

        public Dropdown ProductionModeDropdown;
        public Dropdown VictoryCheckerDropdown;
        public Dropdown DayNightCycleDropdown;

        private void Start()
        {
            Initialize();
            Apply(MatchSettings.Current);
        }

        private void Initialize()
        {
            ProductionModeDropdown.options = UnitProductionBehaviour.LoadAll().Select(x => new Dropdown.OptionData(x.Name)).ToList();
            VictoryCheckerDropdown.options = VictoryChecker.LoadAll().Select(x => new Dropdown.OptionData(x.Name)).ToList();
            DayNightCycleDropdown.onValueChanged.AddListener(SetDayNightCycle);
        }

        private void Update()
        {
            PlayerCountWarning.SetActive(PlayerSettingsParent.childCount > PlayerCountWarningThreshold);
        }

        public void Apply(MatchSettings settings)
        {
            ClearPlayerSettings();
            Map.ApplyMapInfo(settings.MapInfo);

            foreach (var player in settings.Players)
            {
                GameObject newObj = Instantiate(PlayerSettingsPrefab, PlayerSettingsParent);
                InitializePlayerSettings(newObj.GetComponent<PlayerSettings>(), player);
            }

            ProductionModeDropdown.value = Array.IndexOf(UnitProductionBehaviour.LoadAll(), settings.ProductionBehaviour);
            VictoryCheckerDropdown.value = Array.IndexOf(VictoryChecker.LoadAll(), settings.VictoryChecker);
            DayNightCycleDropdown.value = (int)settings.DayNightBehaviour;
        }

        public void AddNewPlayer ()
        {
            GameObject newObj = Instantiate(PlayerSettingsPrefab, PlayerSettingsParent);
            MatchSettings.PlayerInfo newPlayer = new MatchSettings.PlayerInfo();
            newPlayer.Name = newPlayer.GenerateDefaultName();
            newPlayer.AIProfile = Resources.Load<AIPlayerProfile>("AIProfiles/Balanced");
            newPlayer.Faction = Resources.Load<Faction>("Factions/ModernMilitary");
            newPlayer.StartingCredits = 500;

            var playerSettings = newObj.GetComponent<PlayerSettings>();
            InitializePlayerSettings(playerSettings, newPlayer);
        }

        private void InitializePlayerSettings(PlayerSettings settings, MatchSettings.PlayerInfo info)
        {
            settings.ApplyPlayerInfo(info);
            settings.Units.onClick.AddListener(() =>
            {
                SetEnabled(false);
                UnitSelector.SetEnabled(true);
                UnitSelector.Initialize(settings, settings.GetFaction().LoadUnits());
            });
        }

        private void ClearPlayerSettings ()
        {
            foreach (Transform trans in PlayerSettingsParent)
            {
                Destroy(trans.gameObject);
            }
        }

        private IEnumerable<PlayerSettings> GetPlayerSettings()
        {
            foreach (Transform trans in PlayerSettingsParent)
            {
                yield return trans.GetComponent<PlayerSettings>();
            }
        }

        public void SetDayNightCycle (int value)
        {
            MatchSettings.GetCurrent().DayNightBehaviour = (DayNightCycle.DayNightBehaviour)value;
        }

        public void SetCurrentSettings ()
        {
            MatchSettings.Current.MapInfo = Map.CreateMapInfo();

            MatchSettings.Current.ClearPlayers();
            foreach (var info in GetPlayerSettings().Select(x => x.CreatePlayerInfo()))
            {
                MatchSettings.Current.AddPlayer(info);
            }

            MatchSettings.Current.ProductionBehaviour = UnitProductionBehaviour.LoadAll()[ProductionModeDropdown.value];
            MatchSettings.Current.VictoryChecker = VictoryChecker.LoadAll()[VictoryCheckerDropdown.value];
        }

        public void StartGame ()
        {
            SetCurrentSettings();
            SceneManager.LoadScene(1);
        }

        public void SetEnabled (bool value)
        {
            gameObject.SetActive(value);
        }
    }
}