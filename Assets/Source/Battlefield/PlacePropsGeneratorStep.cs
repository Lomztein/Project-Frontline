using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlacePropsGeneratorStep : ISceneryGeneratorStep
{
    public PropInfo[] Props;
    public LayerMask TargetLayer;

    public void Execute(BattlefieldInfo info)
    {
        for (int i = 0; i < Props.Length; i++)
        {
            HandlePropInfo(info, Props[i]);
        }
    }

    private void HandlePropInfo(BattlefieldInfo info, PropInfo prop)
    {
        int amount = Random.Range(prop.Amount.x, prop.Amount.y);
        for (int i = 0; i < amount; i++)
        {
            SpawnProp(info, prop);
        }
    }

    private void SpawnProp(BattlefieldInfo info, PropInfo prop)
    {
        bool success = false;
        while (!success)
        {
            float x = Random.Range(-500, 500);
            float z = Random.Range(-500, 500);
            Vector3 pos = new Vector3(x, 100, z);

            bool fit =
                (prop.InsideBattlefield && GeometryXZ.IsInsidePolygon(info.Shape.GetPerimeterPolygon(info), new Vector3(pos.x, 0f, pos.y))) ||
                (!prop.InsideBattlefield && !GeometryXZ.IsInsidePolygon(info.Shape.GetPerimeterPolygon(info), new Vector3(pos.x, 0f, pos.y)));

            if (fit)
            {
                if (Physics.Raycast(new Ray(pos, Vector3.down), out RaycastHit hit, 200, TargetLayer))
                {
                    float y = hit.point.y;
                    if (y >= prop.MinMaxHeight.x && y <= prop.MinMaxHeight.y)
                    {
                        GameObject newProp = Object.Instantiate(prop.Prefab, hit.point, Quaternion.identity);
                        Quaternion rot = Quaternion.Euler(
                            Random.Range(-prop.RandomRotation.x, prop.RandomRotation.x),
                            Random.Range(-prop.RandomRotation.y, prop.RandomRotation.y),
                            Random.Range(-prop.RandomRotation.z, prop.RandomRotation.z)
                            );
                        newProp.transform.rotation = rot;
                        newProp.transform.localScale *= Random.Range(prop.RandomScale.x, prop.RandomScale.y);

                        success = true;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class PropInfo
    {
        public GameObject Prefab;
        public Vector2Int Amount;
        public Vector3 RandomRotation;
        public Vector2 RandomScale;
        public Vector2 MinMaxHeight;
        public bool InsideBattlefield;
    }
}
