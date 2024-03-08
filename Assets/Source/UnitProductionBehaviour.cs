using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitProductionBehaviour : ScriptableObject
{
    public string Name;
    public string Description;

    public virtual void OnMatchInitialized() { }

    public abstract UnitProductionCallback CreateCallback();

    public abstract class UnitProductionCallback
    {
        public abstract float ProductionTime { get; }
        public abstract float NextProductionTime { get; }

        public abstract void Initialize(Commander owner, float baseProductionTime, Action callback);

        public abstract void Stop();
    }

    public static UnitProductionBehaviour[] LoadAll()
        => Resources.LoadAll<UnitProductionBehaviour>("UnitProductionBehaviours");
}