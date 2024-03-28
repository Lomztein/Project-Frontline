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
            Apply(MatchSetup.Current);
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

        public void Apply(MatchSetup settings)
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
            PlayerInfo newPlayer = new PlayerInfo();
            newPlayer.Name = newPlayer.GenerateDefaultName();
            newPlayer.AIProfile = Resources.Load<AIPlayerProfile>("AIProfiles/Balanced");
            newPlayer.Faction = Resources.Load<Faction>("Factions/ModernMilitary");
            newPlayer.StartingCredits = 500;

            var playerSettings = newObj.GetComponent<PlayerSettings>();
            InitializePlayerSettings(playerSettings, newPlayer);
        }

        private void InitializePlayerSettings(PlayerSettings settings, PlayerInfo info)
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
            MatchSetup.GetCurrent().DayNightBehaviour = (DayNightCycle.DayNightBehaviour)value;
        }

        public void SetCurrentSettings ()
        {
            MatchSetup.Current.MapInfo = Map.CreateMapInfo();

            MatchSetup.Current.ClearPlayers();
            foreach (var info in GetPlayerSettings().Select(x => x.CreatePlayerInfo()))
            {
                MatchSetup.Current.AddPlayer(info);
            }

            MatchSetup.Current.ProductionBehaviour = UnitProductionBehaviour.LoadAll()[ProductionModeDropdown.value];
            MatchSetup.Current.VictoryChecker = VictoryChecker.LoadAll()[VictoryCheckerDropdown.value];
        }

        public void StartGame ()
        {
            SetCurrentSettings();
            MatchRunner.GetInstance().RunMatch(MatchSetup.Current);
        }

        public void SetEnabled (bool value)
        {
            gameObject.SetActive(value);
        }
    }
}