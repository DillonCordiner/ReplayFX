using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using ReplayFX.Utils;
using ReplayFX.Keyframes;
using System.Xml.Linq;


namespace ReplayFX.Patches
{
    [HarmonyPatch(typeof(ReplayCameraController), "DeleteKeyFramesOutside")]
    public static class DeleteKeyFramesOutsidePatch
    {
        [HarmonyPrefix]
        static bool Prefix(ReplayCameraController __instance, ref float start, ref float end)
        {
            if (__instance.keyFrames == null || __instance.keyFrames.Count <= 0)
            {
                return false;
            }
            bool keysDeleted = false;

            for (int i = __instance.keyFrames.Count - 1; i >= 0; i--)
            {
                KeyFrame key = __instance.keyFrames[i];
                if (key != null && (key.time < start - 0.001f || key.time > end + 0.001f))
                {
                    try
                    {
                        __instance.keyFrames.RemoveAt(i);
                        keysDeleted = true;
                    }
                    catch (Exception ex)
                    {
                        Main.Logger.Log($"[DeleteKeyFramesOutside] Failed to delete keyframe at index {i}: {ex.Message}");
                    }
                }
            }
            if (keysDeleted)
            {
                CurveUtil.Refresh();
            }
            return false;
        }

        /*
         [HarmonyPrefix]
         static bool Prefix(ReplayCameraController __instance, ref float start, ref float end)
         {
             if (__instance.keyFrames == null || __instance.keyFrames.Count <= 0)
             {
                 return false;
             }

             int i = 0;
             while (i < __instance.keyFrames.Count)
             {
                 if (__instance.keyFrames[i].time < start - 0.001f || __instance.keyFrames[i].time > end + 0.001f)
                 {
                     try
                     {
                         __instance.keyFrames.RemoveAt(i);
                         CurveUtil.Refresh();
                         __instance.cameraCurve?.DeleteCurveKeys(i, false);
                     }
                     catch (Exception ex)
                     {
                         Main.Logger.Log($"[DeleteKeyFramesOutside] Failed to delete keyframe at index {i}: {ex.Message}");
                     }
                 }
                 else
                 {
                     i++;
                 }
             }
             return false;
         }
         */
    }
}
