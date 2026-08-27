using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using ReplayFX.Utils;

namespace ReplayFX.Patches
{
    /*
    [HarmonyPatch(typeof(ReplayCameraController), "AddKeyFrame")]
    public static class ReplayCameraController_AddKeyFrame_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ReplayCameraController __instance, float time)
        {
            CurveUtil.Refresh();

            
            if (CurveUtil.HasPlayBackKeys())
            {
                CurveUtil.playbackSpeedCurve.CalculateCurveControlPoints();
            }
            
        }
    }
    */
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