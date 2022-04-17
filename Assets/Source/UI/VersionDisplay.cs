using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionDisplay : MonoBehaviour
{
    public Text Text;

    private const string GAME_VERSION_REPLACE = "{GAMEVERSION}";
    private const string UNITY_VERSION_REPLACE = "{UNITYVERSION}";

    private void Start()
    {
        string original = Text.text;
        string gameVersion = Application.version;
        string unityVersion = Application.unityVersion;
        Text.text = original.Replace(GAME_VERSION_REPLACE, gameVersion).Replace(UNITY_VERSION_REPLACE, unityVersion);
    }
}
