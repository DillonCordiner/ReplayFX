using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ReplayEditor;
using ReplayFX.Keyframes;
using ReplayFX.Utils;
using RootMotion;
using SkaterXL.Data;
using UnityEngine;

namespace ReplayFX.Patches
{

    // 1. SANITIZE BASE SERIALIZATION: Remove custom keyframes from base .replay file array to avoid null crashes
    [HarmonyPatch(typeof(ReplayCameraDataExtension), nameof(ReplayCameraDataExtension.CreateFrom))]
    public static class Patch_ReplayCameraDataExtension_CreateFrom
    {
        static bool Prefix(IEnumerable<KeyFrame> keyFrames, ref ReplayCameraData __result)
        {
            __result = new ReplayCameraData
            {
                cameraKeyFrames = keyFrames
                    .Select(k => k.ToSerializatbleKeyframe())
                    .Where(k => k != null) // Filter out nulls produced by custom keyframes
                    .ToArray()
            };
            return false; // Skip original execution
        }
    }

    // 2. SAVE SIDECAR: Intercept SaveFile when a .replay file is written
    [HarmonyPatch(typeof(SaveManager), "SaveFile")]
    public static class Patch_SaveManager_SaveFile
    {
        static void Prefix(string path)
        {
            if (!string.IsNullOrEmpty(path) && path.EndsWith(".replay", StringComparison.OrdinalIgnoreCase))
            {
                var camController = ReplayEditorController.Instance?.cameraController;
                if (camController != null && camController.keyFrames != null)
                {
                    CustomKeyFrameManager.Save(path, camController.keyFrames);
                }
            }
        }
    }
    /*
    [HarmonyPatch(typeof(ReplayEditorController), nameof(ReplayEditorController.LoadFromFile))]
    public static class Patch_ReplayEditorController_LoadFromFile
    {
        static void Postfix(string path)
        {
            var camController = ReplayEditorController.Instance?.cameraController;
            if (camController == null) return;

            List<CustomKeyFrameData> savedKeys = CustomKeyFrameManager.LoadRawData(path);

            if (savedKeys != null && savedKeys.Count > 0)
            {
                foreach (CustomKeyFrameData d in savedKeys)
                {
                    if (d.type == "PlaybackSpeed")
                    {
                        KeyFrameHelper.CreatePlaybackKeyFrame(d.targetSpeed, d.time);
                    }
                    else if (d.type == "Impulse")
                    {
                        KeyFrameHelper.CreateImpluseKeyFrame(null, d.time, d.force, d.amplitude, d.frequency, d.decay);
                    }
                }

                Main.Logger.Log($"[ReplayFX] Successfully loaded {savedKeys.Count} custom keyframe(s).");
            }

            CurveUtil.Refresh();
        }
    }
    */
    [HarmonyPatch(typeof(ReplayEditorController), nameof(ReplayEditorController.LoadFromFile))]
    public static class Patch_ReplayEditorController_LoadFromFile
    {
        // Prefix only - capturing here is safe because it runs before anything
        // else. Do NOT insert keyframes from here; see the class-level comment
        // on why a Postfix on this specific method can't be trusted for timing.
        [HarmonyPrefix]
        static void Prefix(ref ReplayEditorController __instance, string path)
        {
            PathToLoad.path = path;
        }
    }

    internal static class PathToLoad
    {
        public static string path;
    }

    [HarmonyPatch(typeof(ReplayEditorController), nameof(ReplayEditorController.LoadFromDataAwaitable))]
    public static class Patch_ReplayEditorController_LoadFromDataAwaitable
    {
        [HarmonyPostfix]
        static void Postfix(ref ReplayEditorController __instance, ref Task __result)
        {
            __result = LoadCustomKeyFramesAfter(__instance, __result);
        }

        private static async Task LoadCustomKeyFramesAfter(ReplayEditorController __instance, Task originalTask)
        {
            await originalTask; // base game's full load, including LoadKeyFrames, is done now

            string path = PathToLoad.path;
            PathToLoad.path = null; // consume once, so a future caller of this method that isn't LoadFromFile can't accidentally reapply a stale path

            if (string.IsNullOrEmpty(path)) return;

            List<CustomKeyFrameData> savedKeys = CustomKeyFrameManager.LoadRawData(path);
            if (savedKeys == null || savedKeys.Count == 0) return;

            foreach (CustomKeyFrameData d in savedKeys)
            {
                if (d.type == "PlaybackSpeed")
                    KeyFrameHelper.CreatePlaybackKeyFrame(d.targetSpeed, d.time);
                else if (d.type == "Impulse")
                    KeyFrameHelper.CreateImpluseKeyFrame(null, d.time, d.force, d.amplitude, d.frequency, d.decay);
            }

            Main.Logger.Log($"[ReplayFX] Successfully loaded and refreshed {savedKeys.Count} custom keyframe(s).");
        }
    }
}