using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    private static float _matchStartTime;
    public static float MatchTime => Time.time - _matchStartTime;

    private void Start()
    {
        _matchStartTime = Time.time;
    }
}
