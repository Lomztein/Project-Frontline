using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Builder : MonoBehaviour
{
    private static readonly string buildPrefix = "ProjectFrontline";

    private static readonly Dictionary<BuildTarget, string> buildSuffixes = new Dictionary<BuildTarget, string>()
        {
            { BuildTarget.StandaloneWindows, "Win32" },
            { BuildTarget.StandaloneWindows64, "Win64" },
            { BuildTarget.StandaloneLinux64, "Linux" },
            { BuildTarget.StandaloneOSX, "OSX" },
        };

    private static readonly Dictionary<BuildTarget, string> buildExtensions = new Dictionary<BuildTarget, string>()
        {
            { BuildTarget.StandaloneWindows, ".exe" },
            { BuildTarget.StandaloneWindows64, ".exe" },
            { BuildTarget.StandaloneLinux64, ".x86" },
            { BuildTarget.StandaloneOSX, ".app" },
        };

    private static readonly Dictionary<BuildTarget, string> buildChannels = new Dictionary<BuildTarget, string>()
        {
            { BuildTarget.StandaloneWindows, "windows" },
            { BuildTarget.StandaloneWindows64, "windows" },
            { BuildTarget.StandaloneLinux64, "linux" },
            { BuildTarget.StandaloneOSX, "osx" },
        };

    private static readonly string[] buildScenes =
        {
            "Assets/Scenes/SampleScene.unity"
        };

    [MenuItem("Project Frontline/Build")]
    public static void BuildGame()
    {
        BuildGame(Directory.GetParent(Application.dataPath) + "\\Build\\", "StandaloneWindows64");
    }

    public static void CDBuildGame()
    {
        BuildGame("./build/", Environment.GetEnvironmentVariable("BUILD_TARGET"));
    }

    public static void BuildGame(params string[] args)
    {
        BuildTarget[] targets = args.Skip(1).Select(x => (BuildTarget)Enum.Parse(typeof(BuildTarget), x)).ToArray();

        string buildDir = args[0];
        DateTime lastMinorRelease = new DateTime(2020, 10, 8);

        string patch = (DateTime.Now - lastMinorRelease).Days.ToString();
        string build = (DateTime.Now.Second + DateTime.Now.Minute * 60 + DateTime.Now.Hour * 3600).ToString();

        PlayerSettings.bundleVersion = $"0.0.{patch}.{build}";

        if (Directory.Exists(buildDir))
        {
            Directory.Delete(buildDir, true);
        }

        Directory.CreateDirectory(buildDir);

        foreach (var target in targets)
        {
            string dir = Path.Combine(buildDir, target.ToString());
            Directory.CreateDirectory(dir);

            BuildPlayerOptions options = new BuildPlayerOptions()
            {
                targetGroup = BuildTargetGroup.Standalone,
                target = target,
                scenes = buildScenes,
                options = BuildOptions.None,
                locationPathName = Path.Combine(dir, $"{buildPrefix}-{buildSuffixes[target]}{buildExtensions[target]}")
            };

            BuildPipeline.BuildPlayer(options);
            File.WriteAllText(Path.Combine(dir, "version.txt"), PlayerSettings.bundleVersion);
            File.WriteAllText(Path.Combine(dir, "channel.txt"), buildChannels[target]);
        }
    }
}