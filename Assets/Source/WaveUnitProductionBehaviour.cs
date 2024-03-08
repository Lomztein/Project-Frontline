using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New WaveUnitProductionBehaviour", menuName = "Production Behaviours/Wave")]
public class WaveUnitProductionBehaviour : UnitProductionBehaviour
{
    public float WaveTime;
    public bool RoundRobin;
    private float _nextTime;

    private int _waveIndex;
    private event Action<int> OnWave;

    public override void OnMatchInitialized()
    {
        MatchController.GetInstance().StartCoroutine(Wave(WaveTime));
        var commanders = Team.AllTeams.SelectMany(x => x.GetCommanders());
        foreach (var  commander in commanders)
        {
            commander.AddGlobalUnitCostModifier(new MatchProductionTimeUnitCostModifier(WaveTime));
        }
    }

    private IEnumerator Wave(float waveTime)
    {
        while (true)
        {
            _nextTime = Time.time + WaveTime;
            yield return new WaitForSeconds(waveTime);
            OnWave?.Invoke(RoundRobin ? _waveIndex++ : -1);
        }
    }

    public override UnitProductionCallback CreateCallback()
    {
        WaveUnitProductionCallback cb = new WaveUnitProductionCallback(this);
        return cb;
    }

    public class WaveUnitProductionCallback : UnitProductionCallback
    {
        public int CommanderIndex;
        public int MaxCommanders;

        private WaveUnitProductionBehaviour _parent;
        private Action _callback;

        public WaveUnitProductionCallback(WaveUnitProductionBehaviour parent)
        {
            _parent = parent;
        }

        public override float NextProductionTime => _parent.RoundRobin
            ? _parent._nextTime * MaxCommanders - CommanderIndex * _parent.WaveTime
            : _parent._nextTime;
        public override float ProductionTime => _parent.RoundRobin ? _parent.WaveTime : _parent.WaveTime * MaxCommanders;

        public override void Initialize(Commander owner, float baseProductionTime, Action callback)
        {
            MaxCommanders = owner.TeamInfo.GetTeam().GetCommanders().Length;
            CommanderIndex = Array.IndexOf(owner.TeamInfo.GetTeam().GetCommanders(), owner);
            _callback = callback;

            _parent.OnWave += OnWave;
        }

        private void OnWave(int index)
        {
            if (index == -1 || index % MaxCommanders == CommanderIndex)
            {
                _callback();
            }
        }

        public override void Stop()
        {
            _parent.OnWave -= OnWave;
        }
    }
}
