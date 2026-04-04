using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    public static void Build()
    {
        string[] args = Environment.GetCommandLineArgs();

        string buildType = GetArg(args, "-buildType", "Release");
        string outputName = GetArg(args, "-outputName", "Build");
        bool development = string.Equals(buildType, "Debug", StringComparison.OrdinalIgnoreCase);

        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            throw new Exception("No enabled scenes found in File > Build Settings.");
        }

        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        string locationPathName = GetLocationPath(target, outputName);

        Directory.CreateDirectory(Path.GetDirectoryName(locationPathName) ?? "build");

        var options = BuildOptions.None;
        if (development)
        {
            options |= BuildOptions.Development | BuildOptions.AllowDebugging;
        }

        EditorUserBuildSettings.development = development;
        EditorUserBuildSettings.allowDebugging = development;

        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = locationPathName,
            target = target,
            options = options
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Build failed for {target}: {report.summary.result}");
        }
    }

    private static string GetArg(string[] args, string name, string defaultValue)
    {
        int index = Array.IndexOf(args, name);
        if (index >= 0 && index + 1 < args.Length)
        {
            return args[index + 1];
        }
        return defaultValue;
    }

    private static string GetLocationPath(BuildTarget target, string outputName)
    {
        string buildRoot = Path.GetFullPath("build");

        return target switch
        {
            BuildTarget.Android => Path.Combine(buildRoot, $"{outputName}.apk"),
            BuildTarget.StandaloneWindows64 => Path.Combine(buildRoot, outputName, $"{outputName}.exe"),
            BuildTarget.StandaloneLinux64 => Path.Combine(buildRoot, outputName, $"{outputName}.x86_64"),
            BuildTarget.WebGL => Path.Combine(buildRoot, outputName),
            _ => Path.Combine(buildRoot, outputName)
        };
    }
}
