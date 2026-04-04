// Assets/Editor/BuildScript.cs
// ─────────────────────────────────────────────────────────────────────────────
// Custom build method called by game-ci via:
//   buildMethod: BuildScript.BuildWithCompression
//
// What it does:
//   • Applies LZ4HC compression to every build (Release and Debug).
//   • On Debug builds it also enables the Development flag and script
//     debugging so you can attach the Unity profiler / debugger.
//   • Everything else (scripting backend, Android architecture, keystore,
//     target platform, output path) is already configured by game-ci before
//     this method is called, so we don't touch those.
// ─────────────────────────────────────────────────────────────────────────────

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    public static void BuildWithCompression()
    {
        // ── Resolve paths & settings from game-ci environment variables ──────
        string buildTarget   = Env("BUILD_TARGET",   "StandaloneWindows64");
        string buildPath     = Env("BUILD_PATH",     "build/" + buildTarget);
        string buildName     = Env("BUILD_NAME",     "SonicDashPlus");
        bool   isDebug       = string.Equals(Env("BUILD_TYPE", "Release"), "Debug",
                                   StringComparison.OrdinalIgnoreCase)
                            || string.Equals(Env("DEVELOPMENT_BUILD", "false"), "true",
                                   StringComparison.OrdinalIgnoreCase);

        // ── Collect enabled scenes from Build Settings ────────────────────────
        var scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenes.Add(scene.path);
        }

        if (scenes.Count == 0)
            Debug.LogWarning("[BuildScript] No scenes found in Build Settings — building with zero scenes.");

        // ── Resolve the Unity BuildTarget enum ───────────────────────────────
        BuildTarget target;
        if (!Enum.TryParse(buildTarget, out target))
        {
            throw new Exception("[BuildScript] Unknown BUILD_TARGET: " + buildTarget);
        }

        // ── Compose BuildOptions ─────────────────────────────────────────────
        // LZ4HC gives the best compression ratio at acceptable build-time cost.
        // On WebGL this flag is ignored — Unity uses its own Brotli/gzip pipeline.
        BuildOptions options = BuildOptions.CompressWithLz4HC;

        if (isDebug)
        {
            options |= BuildOptions.Development;       // enables Profiler connection
            options |= BuildOptions.AllowDebugging;    // enables script debugger attach
            Debug.Log("[BuildScript] Development build — profiler & script debugging enabled.");
        }

        // ── Run the build ─────────────────────────────────────────────────────
        var playerOptions = new BuildPlayerOptions
        {
            scenes             = scenes.ToArray(),
            locationPathName   = buildPath + "/" + buildName,
            target             = target,
            options            = options,
        };

        Debug.Log(string.Format(
            "[BuildScript] Building → target={0}  path={1}  debug={2}  options={3}",
            target, playerOptions.locationPathName, isDebug, options));

        BuildReport  report  = BuildPipeline.BuildPlayer(playerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log(string.Format(
                "[BuildScript] ✅ Build succeeded in {0:F1}s  ({1} bytes)",
                summary.totalTime.TotalSeconds, summary.totalSize));
        }
        else
        {
            throw new Exception(string.Format(
                "[BuildScript] ❌ Build failed — result={0}  errors={1}",
                summary.result, summary.totalErrors));
        }
    }

    // ── Helper ────────────────────────────────────────────────────────────────
    private static string Env(string key, string fallback = "")
    {
        string value = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrEmpty(value) ? fallback : value;
    }
}
