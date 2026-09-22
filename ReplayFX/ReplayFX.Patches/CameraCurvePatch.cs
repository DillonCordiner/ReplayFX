using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using ReplayFX.Utils;

namespace ReplayFX.Patches
{
    
    [HarmonyPatch(typeof(CameraCurve), "CalculateCurveControlPoints")]
    public static class CameraCurveControlPointPatch
    {
        [HarmonyPostfix]
        static void Postfix(CameraCurve __instance)
        {
            if (CurveUtil.HasPlayBackKeys())
            {
                CurveUtil.playbackSpeedCurve.CalculateCurveControlPoints();
            }
            //Main.Logger.Log("[CalculateCurveControlPoints] Patch Complete");
        }
    }
    
    [HarmonyPatch(typeof(CameraCurve), "Clear")]
    public static class CameraCurveClearPatch
    {
        [HarmonyPostfix]
        static void Postfix(CameraCurve __instance)
        {
            if (CurveUtil.HasPlayBackKeys())
            {
                CurveUtil.playbackSpeedCurve.Clear();
            }
            //Main.Logger.Log("[Clear] Patch Complete");
        }
    }

    /*
    [HarmonyPatch(typeof(CameraCurve), "DeleteCurveKeys")]
    public static class CameraCurveDeleteCurvePatch
    {
        [HarmonyPostfix]
        static void Postfix(CameraCurve __instance, ref int i, ref bool refreshDirectly)
        {
            if (CurveUtil.HasPlayBackKeys())
            {
                CurveUtil.playbackSpeedCurve.DeleteCurveKey(i, refreshDirectly);
            }
            //Main.Logger.Log("[DeleteCurveKeys] Patch Complete");
        }
    }
    */
}