using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using ReplayFX.Utils;
using ReplayFX.Keyframes;

namespace ReplayFX.Patches
{
    [HarmonyPatch(typeof(ReplayCameraController), nameof(ReplayCameraController.DeleteKeyFramesOutside))]
    public static class ReplayCameraController_DeleteKeyFramesOutside_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ReplayCameraController __instance, float start, float end)
        {
            for (int i = __instance.keyFrames.Count - 1; i >= 0; i--)
            {
                if (__instance.keyFrames[i].time < start - 0.001f || __instance.keyFrames[i].time > end + 0.001f)
                {
                    if (__instance.keyFrames[i].GetType() == typeof(PlaybackSpeedKeyFrame) || __instance.keyFrames[i].GetType() == typeof(ImpulseKeyFrame))
                    {
                        __instance.keyFrames.RemoveAt(i);
                        Main.Logger.Log($"[DeleteKeyFramesOutside] Removed Key {__instance.keyFrames[i]} : {i}");
                    }
                }
            }
            /*
            int i = 0;
            while (i < __instance.keyFrames.Count)
            {
                if (__instance.keyFrames[i].time < start - 0.001f || __instance.keyFrames[i].time > end + 0.001f)
                {
                    __instance.keyFrames.RemoveAt(i);
                    __instance.cameraCurve.DeleteCurveKeys(i, false);
                }
                else
                {
                    i++;
                }
            }
            */
            CurveUtil.Refresh();
            Main.Logger.Log("[DeleteKeyFramesOutside] Patch Complete");
        }
    }
    
    [HarmonyPatch(typeof(ReplayCameraController), "AddKeyFrame")]
    public static class ReplayCameraController_AddKeyFrame_Patch
    {
        [HarmonyPrefix]
        static void Prefix(ReplayCameraController __instance, float time)
        {
            if (Main.inputListener.isBumperPressed)
            {
                Main.Logger.Log("[AddKeyFrame] Patch Complete - skipped AddKeyFrames");
                return;
            }
        }
    }
    
    /*
    [HarmonyPatch(typeof(ReplayCameraController), "DeleteKeyFrame")]
    public static class ReplayCameraController_DeleteKeyFrame_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ReplayCameraController __instance, int i, bool refreshDirectly)
        {
            CurveUtil.Refresh();

            if (CurveUtil.HasPlayBackKeys())
            {
                CurveUtil.playbackSpeedCurve.CalculateCurveControlPoints();
            }
        }
    }
    */
}