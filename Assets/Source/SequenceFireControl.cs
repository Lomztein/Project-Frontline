using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceFireControl : MonoBehaviour, IFireControl
{
    public float SequenceDelay;

    public void Fire(int amount, Action<int> callback)
    {
        StartCoroutine(SequenceFire(amount, callback));
    }

    private IEnumerator SequenceFire (int amount, Action<int> callback)
    {
        for (int i = 0; i < amount; i++)
        {
            callback(i);
            yield return new WaitForSeconds(SequenceDelay);
        }
    }
}
