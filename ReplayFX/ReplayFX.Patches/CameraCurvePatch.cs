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
            CurveUtil.playbackSpeedCurve.CalculateCurveControlPoints();
            //Main.Logger.Log("[CalculateCurveControlPoints] Patch Complete");
        }
    }
    
    [HarmonyPatch(typeof(CameraCurve), "Clear")]
    public static class CameraCurveClearPatch
    {
        [HarmonyPostfix]
        static void Postfix(CameraCurve __instance)
        {
            CurveUtil.playbackSpeedCurve.Clear();
            //Main.Logger.Log("[Clear] Patch Complete");
        }
    }

    [HarmonyPatch(typeof(CameraCurve), "DeleteCurveKeys")]
    public static class CameraCurveDeleteCurvePatch
    {
        [HarmonyPostfix]
        static void Postfix(CameraCurve __instance, ref int i, ref bool refreshDirectly)
        {
            CurveUtil.playbackSpeedCurve.DeleteCurveKey(i, refreshDirectly);
            //Main.Logger.Log("[DeleteCurveKeys] Patch Complete");
        }
    }
}