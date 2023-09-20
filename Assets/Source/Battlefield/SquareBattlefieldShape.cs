using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "New SquareBattlefieldShape", menuName = "Battlefield Shapes/Square")]
public class SquareBattlefieldShape : BattlefieldShape
{
    public int Lanes;
    public float Width;
    public float Height;
    public override int Spawns => Lanes * 2;

    public RangeProperty WidthProperty = new("Width", "Width of the battlefield.", 300, 1000, true);
    public RangeProperty HeightProperty = new("Height", "Height of the battlefield.", 300, 1000, true);
    public RangeProperty LanesProperty = new("Lanes", "Lanes of commanders.", 1, 4, true);

    public override IEnumerable<ISpawnVolume> GenerateSpawnVolumes(MapInfo info)
    {
        float halfWidth = Width / 2f;

        for (int i = 0; i < Lanes; i++)
        {
            yield return new SpawnLine(
                new Vector3(0f, 0f, halfWidth - 50 - (i * 50)), Quaternion.LookRotation(Vector3.back),
                new Vector3(1f, 0f, 0f), 30f);
        }

        for (int i = 0;i < Lanes; i++) {
            yield return new SpawnLine(
                new Vector3(0f, 0f, -halfWidth + 50 + (i * 50)), Quaternion.LookRotation(Vector3.forward),
                new Vector3(1f, 0f, 0f), 30f);
        }

    }

    public override IEnumerable<NavigationNode> GenerateNodes(MapInfo info)
    {
        float halfWidth = Width / 2f;
        var list = new List<NavigationNode>
        {
            // Left side
            NavigationNode.Create(new Vector3(0f, 0f, halfWidth), null),
            // Right side
            NavigationNode.Create(new Vector3(0f, 0f, -halfWidth), null)
        };

        list[0].Neighbours = new NavigationNode[] { list[1] };
        list[1].Neighbours = new NavigationNode[] { list[0] };
        return list;
    }

    public override IEnumerable<Vector3> GetPerimeterPolygon(MapInfo info)
    {
        float halfWidth = Width / 2f;
        float halfHeight = Height / 2f;

        yield return new Vector3(halfHeight, 0f, halfWidth);
        yield return new Vector3(halfHeight, 0f, -halfWidth);
        yield return new Vector3(-halfHeight, 0f, -halfWidth);
        yield return new Vector3(-halfHeight, 0f, halfWidth);
    }

    public override IEnumerable<IProperty> GetProperties()
    {
        yield return WidthProperty;
        yield return HeightProperty;
        yield return LanesProperty;
    }

    public override bool SetProperty(IProperty property, object value)
    {
        if (property == WidthProperty) Width = (float)value;
        if (property == HeightProperty) Height = (float)value;
        if (property == LanesProperty) Lanes = Convert.ToInt32(value);
        return true;
    }

    public override object GetProperty(IProperty property)
    {
        if (property == WidthProperty) return Width;
        if (property == HeightProperty) return Height;
        if (property == LanesProperty) return Lanes;
        return null;
    }
}
