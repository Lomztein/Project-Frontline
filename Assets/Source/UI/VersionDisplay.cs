using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VersionDisplay : MonoBehaviour
{
    public Text Text;
    public TMP_Text TMPText;

    private const string GAME_VERSION_REPLACE = "{GAMEVERSION}";
    private const string UNITY_VERSION_REPLACE = "{UNITYVERSION}";

    private void Start()
    {
        string original = Text == null ? TMPText.text : Text.text;
        string gameVersion = Application.version;
        string unityVersion = Application.unityVersion;

        if (TMPText)
        {
            TMPText.text = original.Replace(GAME_VERSION_REPLACE, gameVersion).Replace(UNITY_VERSION_REPLACE, unityVersion);
        }
        else
        {
            Text.text = original.Replace(GAME_VERSION_REPLACE, gameVersion).Replace(UNITY_VERSION_REPLACE, unityVersion);
        }
    }
}
