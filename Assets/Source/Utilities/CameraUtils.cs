using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class CameraUtils
{
    public static IEnumerable<Rect> ComputeViewportRects(int numCameras)
    {
        int remainingCameras = numCameras;
        int numRows = Mathf.RoundToInt(Mathf.Sqrt(numCameras));
        int maxCamerasPerRow = Mathf.CeilToInt(Mathf.Sqrt(numCameras));
        for (int r = 0; r < numRows; r++)
        {
            float viewportStartY = r / (float)numRows;
            float viewportHeight = 1 / (float)numRows;

            int camerasForRow = Mathf.Min(remainingCameras, maxCamerasPerRow);
            for (int c = 0; c < camerasForRow; c++)
            {
                float viewportStartX = c / (float)camerasForRow;
                float viewportWidth = 1 / (float)camerasForRow;

                yield return new Rect(viewportStartX, viewportStartY, viewportWidth, viewportHeight);
            }

            remainingCameras -= camerasForRow;
        }
    }

    public static Rect[] MatchPositions(Rect[] rects, Vector3[] positions)
    {
        if (rects.Length !=  positions.Length)
        {
            throw new InvalidOperationException("Lenghts of both input arrays must be equal.");
        }

        IList<IList<Rect>> allPermutations = EnumerableUtils.Permute(rects);

        float bestScore = float.MinValue;
        IList<Rect> bestPermutation = allPermutations.FirstOrDefault();

        foreach (IList<Rect> permutation in allPermutations)
        {
            float score = 0f;
            for (int i = 0; i < permutation.Count; i++)
            {
                score -= Vector2.Distance(permutation[i].center, positions[i].normalized);
            }
            if (score > bestScore)
            {
                bestScore = score;
                bestPermutation = permutation;
            }
        }

        return bestPermutation.ToArray();
    }
}