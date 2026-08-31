using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using ReplayFX.Utils;
using ReplayFX.Keyframes;

namespace ReplayFX.Patches
{
    [HarmonyPatch(typeof(ReplayCameraController), "DeleteKeyFramesOutside")]
    public static class ReplayCameraController_DeleteKeyFramesOutside_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ReplayCameraController __instance, ref float start, ref float end)
        {
            for (int i = __instance.keyFrames.Count - 1; i >= 0; i--)
            {
                if (__instance.keyFrames[i].time < start - 0.001f || __instance.keyFrames[i].time > end + 0.001f)
                {
                    //Main.Logger.Log($"[DeleteKeyFramesOutside] Removing Key {__instance.keyFrames[i].GetType().Name} : {i}");

                    __instance.keyFrames.RemoveAt(i);

                    /*
                    if (__instance.keyFrames[i] is PlaybackSpeedKeyFrame || __instance.keyFrames[i] is ImpulseKeyFrame)
                    {
                        Main.Logger.Log($"[DeleteKeyFramesOutside] Removing Key {Keyname} : {i}");
                        __instance.keyFrames.RemoveAt(i);
                        __instance.cameraCurve.DeleteCurveKeys(i, false);
                    }
                    */
                }
            }
            //CurveUtil.Refresh();
            //Main.Logger.Log("[DeleteKeyFramesOutside] Patch Complete");
            return false;
        }
    }
    /*
    [HarmonyPatch(typeof(ReplayCameraController), "AddKeyFrame")]
    public static class ReplayCameraController_AddKeyFrame_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ReplayCameraController __instance, ref float time)
        {
            CurveUtil.Refresh();
            Main.Logger.Log("[AddKeyFrame] Patch Complete - Refreshed");
        }
    }
    */
    /*
    [HarmonyPatch(typeof(ReplayCameraController), "DeleteKeyFrame")]
    public static class ReplayCameraController_DeleteKeyFrame_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ReplayCameraController __instance, int i, ref bool refreshDirectly)
        {
            CurveUtil.Refresh();
            Main.Logger.Log("[DeleteKeyFrame] Patch Complete - Refreshed");
        }
    }
    */
}