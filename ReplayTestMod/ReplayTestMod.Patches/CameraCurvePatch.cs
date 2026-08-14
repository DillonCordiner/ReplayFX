using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using ReplayTestMod.Utils;

namespace ReplayTestMod.Patches
{
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

    [HarmonyPatch(typeof(CameraCurve), "DeleteCurveKeys")]
    public static class CameraCurveDeleteCurvePatch
    {
        [HarmonyPostfix]
        static void Postfix(CameraCurve __instance, int i, bool refreshDirectly)
        {
            //CurveUtil.playbackSpeedCurve.DeleteCurveKey(i, refreshDirectly);
            CurveUtil.playbackSpeedCurve.DeleteCurveKey(i, refreshDirectly);
            CurveUtil.ClearCurveKeys();
            /*
            if (!Main.timelineManager.HasPlayBackKeys())
            {
                CurveUtil.playbackSpeedCurve.Clear();
            }
            else
            {
                //CurveUtil.playbackSpeedCurve.CalculateCurveControlPoints();
            }
            */
        }
    }
}