using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TargetIncomingDamageTracking
{
    private static readonly Dictionary<object, float> _incomingDamageTracker = new Dictionary<object, float>();

    public static float GetIncomingDamage(object forObject)
    {
        if (_incomingDamageTracker.ContainsKey(forObject))
        {
            return _incomingDamageTracker[forObject];
        }
        return 0;
    }

    public static void Clear(object forObject)
    {
        if (_incomingDamageTracker.ContainsKey(forObject))
            _incomingDamageTracker.Remove(forObject);
    }

    public static void AddIncomingDamage(object forObject, float amount)
    {
        if (!_incomingDamageTracker.ContainsKey(forObject))
        {
            _incomingDamageTracker.Add(forObject, amount);
        }
        else
        {
            _incomingDamageTracker[forObject] +=  amount;
        }
    }

    public static void SubtractIncomingDamage(object forObject, float amount)
        => AddIncomingDamage(forObject, -amount);
}
