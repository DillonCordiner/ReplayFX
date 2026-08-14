using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using ReplayTestMod.Utils;

namespace ReplayTestMod.Patches
{
    /*
    [HarmonyPatch(typeof(CameraCurve), "CalculateCurveControlPoints")]
    public static class CameraCurveControlPointPatch
    {
        [HarmonyPostfix]
        static void Postfix(CameraCurve __instance)
        {
            //CurveUtil.playbackSpeedCurve.CalculateCurveControlPoints();
        }
    }

    [HarmonyPatch(typeof(CameraCurve), "Clear")]
    public static class CameraCurveClearPatch
    {
        [HarmonyPostfix]
        static void Postfix(CameraCurve __instance)
        {
            //CurveUtil.playbackSpeedCurve.Clear();
        }
    }
    */

    [HarmonyPatch(typeof(CameraCurve), "Refresh")]
    public static class CameraCurveRefreshPatch
    {
        static void Prefix()
        {
            // Wipe the stale data so we have a clean slate for the surviving keyframes
            if (CurveUtil.playbackSpeedCurve != null)
            {
                CurveUtil.playbackSpeedCurve.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(CameraCurve), "DeleteCurveKeys")]
    public static class CameraCurveDeleteCurvePatch
    {
        [HarmonyPostfix]
        static void Postfix(CameraCurve __instance, int i, bool refreshDirectly)
        {
            CurveUtil.playbackSpeedCurve.DeleteCurveKey(i, refreshDirectly);
            CurveUtil.Refresh();

        }
    }
}