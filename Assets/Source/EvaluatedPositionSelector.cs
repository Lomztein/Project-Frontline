using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class EvaluatedPositionSelector : MonoBehaviour, IPositionSeletor
{
    public int TargetPoints = 100;
    public int MaxTries = 1000;

    private const string StructTag = "StructureUnit";
    public LayerMask TerrainLayer;

    [SerializeReference, SR]
    public IPositionEvaluator[] PositionEvaluators;

    public Vector3? SelectPosition(Commander commander, GameObject unit, Vector3 checkSize)
    {
        int tries = MaxTries;
        Vector3[] targetPoints = new Vector3[TargetPoints];
        var placed = Enumerable.Concat(commander.Fortress.ObjectToEnumerable(), commander.AlivePlaced.Select(x => x.transform)).ToArray();

        int points = 0;
        for (points = 0; points < TargetPoints; points++)
        {
            bool success = false;
            Vector3 point = Vector3.zero;
            while (!success)
            {
                Transform buildFrom = placed[Random.Range(0, placed.Length)].transform;
                point = buildFrom.position + commander.BuildRadius * Random.insideUnitSphere;
                point.y = 0;

                if (IsWithinRange(point, commander) && CanPlace(point, checkSize))
                {
                    success = true;
                    targetPoints[points] = point;
                    points++;
                }
                else
                {
                    tries--;
                }

                if (tries < 0)
                {
                    break;
                }
            }

            if (tries < 0)
            {
                break;
            }
        }

        if (points > 0)
        {
            Vector3 bestPoint = targetPoints[0];
            float bestScore = float.MinValue;
            for (int i = 0; i < points; i++)
            {
                Vector3 position = targetPoints[i];
                float score = PositionEvaluators.Sum(x => x.Evaluate(commander, unit, position));
                Debug.DrawRay(position, Vector3.up * score, Color.white, 1f);

                if (score > bestScore)
                {
                    bestPoint = position;
                    bestScore = score;
                }
            }

            return bestPoint;
        }
        else
        {
            return null;
        }
    }

    private bool IsWithinRange (Vector3 pos, Commander commander)
    {
        return commander.IsNearAnyPlaced(pos);
    }

    private bool CanPlace (Vector3 position, Vector3 checkSize)
    {
        Collider[] colliders = Physics.OverlapBox(position, checkSize / 2f, Quaternion.identity, ~TerrainLayer);
        return !colliders.Any(x => x.CompareTag(StructTag)) && MatchSettings.Current.MapInfo.Contains(position);
    }
}
