using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainHeightmapGeneratorStep : ISceneryGeneratorStep
{
    public float PlayAreaHeight = 10f;
    public float MaxOutsideHeight = 20f;
    public float BorderSize = 15f;
    public AnimationCurve BorderCurve;
    
    public float NoiseScale;
    public int NoiseOctaves;
    public float NoisePower = 4f;

    public void Execute(BattlefieldInfo info)
    {
        foreach (var terrain in Terrain.activeTerrains)
        {
            RenderTexture tex = terrain.terrainData.heightmapTexture;

            int width = tex.width;
            int height = tex.height;
            float[,] heightMap = new float[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    heightMap[x, y] = GenerateHeight(info, PixelToWorldCoord(terrain, width, height, x, y)) / terrain.terrainData.size.y;
                }
            }

            terrain.terrainData.SetHeightsDelayLOD(0, 0, heightMap);
            terrain.terrainData.SyncHeightmap();
            terrain.Flush();
        }
    }

    private float GenerateHeight(BattlefieldInfo info, Vector3 worldPos)
    {
        var shape = info.Shape.GetPerimeterPolygon(info);
        bool isInside = GeometryXZ.IsInsidePolygon(shape, worldPos);
        if (isInside) return PlayAreaHeight;

        float distance = GeometryXZ.DistanceFromPolygon(shape, worldPos);
        float noiseFactor = BorderCurve.Evaluate(Mathf.Clamp01(distance / BorderSize));
        float noise = Mathf.Pow(Noise(worldPos / NoiseScale, NoiseOctaves), NoisePower);

        return Mathf.Lerp(PlayAreaHeight, noise * MaxOutsideHeight, noiseFactor);
    }

    private float Noise(Vector3 pos, int octaves)
    {
        float total = 0f;
        float norm = 0f;
        for (int i = 1; i <= octaves; i++)
        {
            float scale = 1f / i;
            total += Mathf.PerlinNoise(pos.x / scale + 10000, pos.z / scale + 10000) * scale;
            norm += scale;
        }
        return total / norm;
    }

    private Vector3 PixelToWorldCoord(Terrain terrain, int width, int height, int x, int y)
    {
        float xr = (float)x / width;
        float yr = (float)y / height;

        float wx = terrain.GetPosition().x + xr * terrain.terrainData.size.x;
        float wz = terrain.GetPosition().z + yr * terrain.terrainData.size.z;

        return new Vector3(wz, 0f, wx);
    }
}
