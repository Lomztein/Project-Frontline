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

        public GameObject PlayerSettingsPrefab;
        public Transform PlayerSettingsParent;


        private void Start()
        {
            Apply(MatchSettings.Default());
        }

        public void Apply(MatchSettings settings)
        {
            ClearPlayerSettings();
            Map.ApplyMapInfo(settings.MapInfo);
            foreach (var player in settings.Players)
            {
                GameObject newObj = Instantiate(PlayerSettingsPrefab, PlayerSettingsParent);
                newObj.GetComponent<PlayerSettings>().ApplyPlayerInfo(player);
            }
        }

        public void AddNewPlayer ()
        {
            GameObject newObj = Instantiate(PlayerSettingsPrefab, PlayerSettingsParent);
            MatchSettings.PlayerInfo newPlayer = new MatchSettings.PlayerInfo();
            newPlayer.Name = NameGenerator.GenerateName();
            newPlayer.AIProfile = Resources.Load<AIPlayerProfile>("AIProfiles/Balanced");
            newPlayer.StartingCredits = 500;
            newObj.GetComponent<PlayerSettings>().ApplyPlayerInfo(newPlayer);
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
            MatchSettings.Current = ScriptableObject.CreateInstance<MatchSettings>();
            MatchSettings.Current.MapInfo = Map.CreateMapInfo();
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
    }
}