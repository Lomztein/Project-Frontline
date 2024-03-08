using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New ContinuousUnitProductionBehaviour", menuName = "Production Behaviours/Continuous")]
public class ContinuousUnitProductionBehaviour : UnitProductionBehaviour
{
    public bool NormalizePerTeam;

    public override UnitProductionCallback CreateCallback()
    {
        ContinuousProductionCallback callback = new ContinuousProductionCallback();
        callback.NormalizePerTeam = NormalizePerTeam;
        return callback;
    }

    public class ContinuousProductionCallback : UnitProductionCallback
    {
        public bool NormalizePerTeam;
        private Action _callback;
        private Commander _owner;
        private Coroutine _coroutine;

        private float _productionTime;
        private float _nextProductionTime;

        public override float NextProductionTime => _nextProductionTime;
        public override float ProductionTime => _productionTime;

        public override void Initialize(Commander owner, float baseProductionTime, Action callback)
        {
            _callback = callback;
            _owner = owner;

            int teamMembers = owner.TeamInfo.GetTeam().GetCommanders().Length;
            float mult = NormalizePerTeam ? teamMembers : 1f;

            _productionTime = baseProductionTime * mult;
            _coroutine = _owner.StartCoroutine(InvokeCallback(_productionTime));
        }

        public IEnumerator InvokeCallback (float time)
        {
            while (true)
            {
                _nextProductionTime = Time.time + _productionTime;
                yield return new WaitForSeconds(time);
                _callback();
            }
        }

        public override void Stop()
        {
            if (_owner && _coroutine != null)
            {
                _owner.StopCoroutine(_coroutine);
            }
        }
    }
}
