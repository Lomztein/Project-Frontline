using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Util;

public class EllipsoidBattlefieldShape : IBattlefieldShape
{
    public string DisplayName => "Ellipsoid";
    public int MaxTeams => 4;
    private float _spawnDepth = 50;

    public IEnumerable<ISpawnVolume> GenerateSpawnVolumes(MapInfo info)
    {
        float width = info.Width / 2f;
        float height = info.Height / 2f;
        int sides = MatchSettings.Current.Players.Select(x => x.Team).Distinct().Count();
        for (int i = 0; i < sides; i++)
        {
            float rads = (i / (float)sides) * Mathf.PI * 2f;
            float z = Mathf.Cos(rads) * width;  
            float x = Mathf.Sin(rads) * height;

            var dir = new Vector3(x, 0f, z).normalized * -1f;
            Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;

            yield return new SpawnLine(new Vector3(x, 0f, z) + dir * _spawnDepth, left, 30);
        }
    }

    public IEnumerable<IWaypoint> GenerateWaypoints(MapInfo info)
    {
        float width = info.Width / 2f;
        float height = info.Height / 2f;

        var teams = MatchSettings.Current.Players.Select(x => x.Team).Distinct().ToArray();
        int sides = teams.Length;

        TeamDependentWaypoint[] teamWaypoints = new TeamDependentWaypoint[sides];
        CascadeWaypoint[] teamCenters = new CascadeWaypoint[sides];

        for (int i = 0; i < sides; i++)
        {
            float rads = (i / (float)sides) * Mathf.PI * 2f;
            float z = Mathf.Cos(rads) * width;
            float x = Mathf.Sin(rads) * height;

            var dir = new Vector3(x, 0f, z).normalized * -1f;
            var pos = new Vector3(x, 0f, z);

            Waypoint spawnWaypoint = Waypoint.Create(pos + dir * _spawnDepth, null, null);
            teamWaypoints[i] = TeamDependentWaypoint.Create(pos + dir * _spawnDepth, null, null);
            teamCenters[i] = CascadeWaypoint.Create(Vector3.zero, spawnWaypoint, null);

            teamWaypoints[i].Prev = teamCenters[i];
            spawnWaypoint.Next = teamCenters[i];

            yield return spawnWaypoint;
        }

        for (int i = 0; i < sides; i++)
        {
            Team currentTeam = teams[i].GetTeam();
            IEnumerable<Team> otherTeams = teams.Where(x => x.GetTeam() != currentTeam).Select(x => x.GetTeam());

            var teamWaypoint = teamWaypoints[i];
            var enemyTeamWaypoints = teamWaypoints.Where(x => x != teamWaypoint);

            teamCenters[i].Options = UnityUtils.Shift(enemyTeamWaypoints.ToArray(), i);
        }

        for (int i =  sides; i < sides; i++)
        {
            yield return teamWaypoints[i];
            yield return teamCenters[i];
        }
    }

    public IEnumerable<Vector3> GetPerimeterPolygon(MapInfo info)
    {
        float width = info.Width / 2f;
        float height = info.Height / 2f;
        int sides = MatchSettings.Current.Players.Select(x => x.Team).Distinct().Count() * 2;
        for (int i = 0; i < sides; i++)
        {
            float rads = -((i / (float)sides) * Mathf.PI * 2f);
            float offset = Mathf.PI / (sides);
            float z = Mathf.Cos(rads + offset) * width;
            float x = Mathf.Sin(rads + offset) * height;
            yield return new Vector3(x, 0f, z);
        }
    }
}
