using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NameGenerator
{
    private const string FIRSTNAME_PATH = "Names/FirstNames";
    private const string LASTNAME_PATH = "Names/LastNames";

    private static string[] _firstNames;
    private static string[] _lastNames;
    private static bool Cached => _lastNames != null && _firstNames != null;

    private static void CacheNames()
    {
        _firstNames = Resources.Load<TextAsset>(FIRSTNAME_PATH).text.Split('\n');
        _lastNames = Resources.Load<TextAsset>(LASTNAME_PATH).text.Split('\n');
    }

    public static string GenerateName()
    {
        if (!Cached) CacheNames();
        string first = _firstNames[Random.Range(0, _firstNames.Length)];
        string last = _lastNames[Random.Range(0, _lastNames.Length)];
        return first + " " + last;
    }
}
