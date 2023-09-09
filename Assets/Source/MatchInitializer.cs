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

            Vector3 teamPosition = spawnLines[i].Center;
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

        foreach (var mutator in settings.Mutators)
        {
            mutator.Start();
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
            AICommander aiCom = commanderObj.GetComponent<AICommander>();
            aiCom.TargetAvarageAPM = info.AIProfile.ActionsPerMinute;
            var selector = aiCom.GetComponent<WeightedUnitSelector>();
            selector.WeightTable = info.AIProfile.UnitWeightTable;
            aiCom.SaveTimeMinMax = info.AIProfile.SaveTimeMinMax;
            aiCom.SaveTimeBias = info.AIProfile.SaveTimeBias;
        }
        Commander commander = commanderObj.GetComponent<Commander>();
        commander.Fortress = Instantiate(info.Faction.HeadquartersPrefab, commanderObj.transform).transform;
        commander.Credits = info.StartingCredits;
        commander.Name = info.Name;
        commander.UnitAvailable = info.UnitAvailable;
        commander.Faction = info.Faction;

        info.Team.ApplyTeam(commanderObj);
        commander.AssignCommander(commanderObj);

        if (info.IsPlayer)
        {
            AttachPlayerToPurchaseMenu(commander);
            MatchController.SetPlayerCommander(commander);
            Camera.main.GetComponent<TopDownCameraController>().MoveToHQ();
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
