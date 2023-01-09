using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    private static float _matchStartTime;

    public static float MatchTime => Time.time - _matchStartTime;
    public static Commander PlayerCommander { get; private set; }

    private void Start()
    {
        _matchStartTime = Time.time;
    }

    public static void SetPlayerCommander(Commander cmd) => PlayerCommander = cmd;
}
