using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Current;

    private struct RecentPlay
    {
        public float StartTime;
        public float Duration;
        public Vector3 Position;

        public RecentPlay(float time, float duration, Vector3 position)
        {
            StartTime = time;
            Duration = duration;
            Position = position;
        }

        public bool IsActive(float time)
            => StartTime + Duration > time;
    }

    [System.Serializable]
    public struct Rule
    {
        public Object Key;
        public int MaxTotal;
        public int MaxProximity;
        public float ProximityRange;
        public float DurationOverride;
    }

    public Rule DefaultRule;
    public Rule[] Rules;
    private static Dictionary<Object, Rule> _rules = new Dictionary<Object, Rule>();
    private static Dictionary<Object, RecentPlay[]> _recentPlays = new Dictionary<Object, RecentPlay[]>();

    private void Awake()
    {
        foreach (Rule rule in Rules)
        {
            RegisterRule(rule.Key, rule);
        }
        Current = this;
    }

    public static void RegisterRule(Object key, Rule rule)
    {
        if (!_rules.ContainsKey(key))
        {
            _rules.Add(key, rule);
            _recentPlays.Add(key, new RecentPlay[rule.MaxTotal]);
        }
    }

    public static bool RequestPlay(AudioClip clip, Object key, Vector3 position)
    {
        if (!_rules.ContainsKey(key))
        {
            RegisterRule(key, Current.DefaultRule);
        }

        float time = Time.time;

        Rule rule = _rules[key];
        RecentPlay[] arr = _recentPlays[key];

        float duration = rule.DurationOverride > 0.01 ? rule.DurationOverride : clip.length;

        // Ensure that there is any space at all.
        if (arr.All(x => x.IsActive(time)))
        {
            return false; // There is no more space for any new clips with this key.
        }

        // Check that no more than allowed are playing within proximity.
        int countWithinProx = 0;
        for (int i = 0; i <  arr.Length; i++)
        {
            if (arr[i].IsActive(time))
            {
                float sqr = Vector3.SqrMagnitude(arr[i].Position - position);
                if (sqr < rule.ProximityRange * rule.MaxProximity)
                {
                    countWithinProx++;
                }
            }
        }
        if (countWithinProx >= rule.MaxProximity)
        {
            return false;
        }

        RecentPlay play = new RecentPlay(time, duration, position);
        arr[GetInactiveID(arr, time)] = play;
        return true;
    }

    private static int GetInactiveID(RecentPlay[] arr, float time)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (!arr[i].IsActive(time))
            {
                return i;
            }
        }
        return -1;
    }
}
