using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

public class MatchInitializer : MonoBehaviour
{
    public bool InitializeCurrentSettingsOnAwake;

    public GameObject TeamPrefab;
    public GameObject AIPrefab;
    public GameObject PlayerPrefab;

    private void Awake()
    {
        if (InitializeCurrentSettingsOnAwake)
        {
            InitializeMatch(MatchSettings.Current);
        }
    }

    public void InitializeMatch (MatchSettings settings)
    {
        settings.MapInfo.SceneryGenerator.Generate(settings.MapInfo);
        settings.MapInfo.Shape.GenerateWaypoints(settings.MapInfo);

        var teams = settings.Players.GroupBy(x => x.Team).ToArray();
        var spawnLines = settings.MapInfo.Shape.GenerateSpawnLines(settings.MapInfo).ToArray();

        for (int i = 0; i < teams.Length; i++)
        {
            var players = teams[i].ToArray();

            Vector3 teamPosition = Vector3.Lerp(spawnLines[i].Start, spawnLines[i].End, 0.5f);
            Quaternion rotation = Quaternion.Euler(0f, Waypoint.GetNearest(teamPosition).OutgoingAngle, 0f);
            Team team = SpawnTeam(teams[i].Key, teamPosition, rotation);

            for (int j = 0; j < players.Length; j++)
            {
                Vector3 playerPosition = spawnLines[i].GetSpawnPoint(j, players.Length);
                Commander com = SpawnCommander(players[j], playerPosition, rotation);
                com.transform.SetParent(team.transform, true);
            }

            team.TeamInfo.ApplyTeam(team.gameObject); // shrug
        }
    }

    private Team SpawnTeam (TeamInfo teamInfo, Vector3 position, Quaternion rotation)
    {
        GameObject teamObj = Instantiate(TeamPrefab, position, rotation);
        Team team = teamObj.GetComponent<Team>();
        team.TeamInfo = teamInfo;
        return team;
    }

    private Commander SpawnCommander(MatchSettings.PlayerInfo info, Vector3 position, Quaternion rotation)
    {
        GameObject commanderObj;
        if (info.IsPlayer)
        {
            commanderObj = Instantiate(PlayerPrefab, position, rotation);
        }
        else
        {
            commanderObj = Instantiate(AIPrefab, position, rotation);
        }
        Commander commander = commanderObj.GetComponent<Commander>();
        commander.Fortress = Instantiate(info.Faction.HeadquartersPrefab, commanderObj.transform).transform;
        commander.Credits = info.StartingCredits;
        commander.AssignCommander(commanderObj);

        if (info.IsPlayer)
        {
            AttachPlayerToPurchaseMenu(commander);
            Camera.main.transform.position = new Vector3(position.x, Camera.main.transform.position.y, position.z);
        }
        return commander;
    }

    private void AttachPlayerToPurchaseMenu (Commander player)
    {
        UnitPurchaseMenu menu = GameObject.Find("PurchaseMenu").GetComponent<UnitPurchaseMenu>();
        menu.Commander = player;
        menu.UpdateActive();
        Object.FindObjectOfType<CreditsDisplay>().Commander = player;
    }
}
