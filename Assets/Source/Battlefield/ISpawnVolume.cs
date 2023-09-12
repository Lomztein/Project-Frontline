using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ISpawnVolume
{
    public int Max { get; }
    public Vector3 Position { get; }

    public Vector3 GetSpawnPoint(int index, int total);
}