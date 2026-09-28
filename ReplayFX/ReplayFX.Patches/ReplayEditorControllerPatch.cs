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
    internal static class PathToLoad
    {
        public static string path;
    }

    [HarmonyPatch(typeof(ReplayEditorController), nameof(ReplayEditorController.LoadFromFile))]
    public static class ReplayEditorController_LoadFromFile_Patch
    {
        [HarmonyPrefix]
        static void Prefix(ref ReplayEditorController __instance, string path)
        {
            PathToLoad.path = path;
        }
    }

    [HarmonyPatch(typeof(ReplayEditorController), nameof(ReplayEditorController.LoadFromDataAwaitable))]
    public static class PReplayEditorController_LoadFromDataAwaitable_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref ReplayEditorController __instance, ref Task __result)
        {
            __result = LoadCustomKeyFramesAfter(__instance, __result);
        }

        private static async Task LoadCustomKeyFramesAfter(ReplayEditorController __instance, Task originalTask)
        {
            await originalTask;

            string path = PathToLoad.path;
            PathToLoad.path = null;

            if (string.IsNullOrEmpty(path)) return;

            try
            {
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
            catch (Exception ex)
            {
                Main.Logger.Log($"[ReplayFX] Failed to apply custom keyframes for '{path}': {ex.Message}");
            }
        }
    }
}