using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomGame
{
    public class GameSettings : MonoBehaviour
    {
        public MapSettings Map;
        public UnitSelector UnitSelector;

        public GameObject PlayerSettingsPrefab;
        public Transform PlayerSettingsParent;


        private void Start()
        {
            Apply(MatchSettings.Current);
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
        }

        public void AddNewPlayer ()
        {
            GameObject newObj = Instantiate(PlayerSettingsPrefab, PlayerSettingsParent);
            MatchSettings.PlayerInfo newPlayer = new MatchSettings.PlayerInfo();
            newPlayer.Name = NameGenerator.GenerateName();
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

        public void SetCurrentSettings ()
        {
            MatchSettings.Current.MapInfo = Map.CreateMapInfo();

            MatchSettings.Current.ClearPlayers();
            foreach (var info in GetPlayerSettings().Select(x => x.CreatePlayerInfo()))
            {
                MatchSettings.Current.AddPlayer(info);
            }
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