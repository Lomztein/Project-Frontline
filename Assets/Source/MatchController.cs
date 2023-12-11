using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    private static MatchController _instance;
    private static float _matchStartTime;

    public static float MatchStartTime => _matchStartTime;
    public static float MatchTime => Time.time - _matchStartTime;

    public static Commander PlayerCommander { get; private set; }

    public static MatchController GetInstance()
    {
        if (_instance == null)
            _instance = FindAnyObjectByType<MatchController>();
        return _instance;
    }

    private void Start()
    {
        _matchStartTime = Time.time;
    }

    public static void SetPlayerCommander(Commander cmd) => PlayerCommander = cmd;
}
