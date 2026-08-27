using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using ReplayFX.Utils;

namespace ReplayFX.Patches
{
    
    [HarmonyPatch(typeof(CameraCurve), nameof(CameraCurve.CalculateCurveControlPoints))]
    public static class CameraCurveControlPointPatch
    {
        [HarmonyPostfix]
        static void Postfix(CameraCurve __instance)
        {
            CurveUtil.playbackSpeedCurve.CalculateCurveControlPoints();
        }
    }
    
    [HarmonyPatch(typeof(CameraCurve), nameof(CameraCurve.Clear))]
    public static class CameraCurveClearPatch
    {
        [HarmonyPostfix]
        static void Postfix(CameraCurve __instance)
        {
            CurveUtil.playbackSpeedCurve.Clear();
        }
    }

    [HarmonyPatch(typeof(CameraCurve), nameof(CameraCurve.DeleteCurveKeys))]
    public static class CameraCurveDeleteCurvePatch
    {
        [HarmonyPostfix]
        static void Postfix(CameraCurve __instance, int i, bool refreshDirectly)
        {
            CurveUtil.playbackSpeedCurve.DeleteCurveKey(i, refreshDirectly);
        }
    }

    /*
    [HarmonyPatch(typeof(CameraCurve), nameof(CameraCurve.Refresh))]
    public static class CameraCurveRefreshPatch
    {
        [HarmonyPostfix]
        static void Postfix()
        {
            //CurveUtil.playbackSpeedCurve.CalculateCurveControlPoints();

            
            if (CurveUtil.HasPlayBackKeys())
            {
                CurveUtil.playbackSpeedCurve.CalculateCurveControlPoints();
            }
            
        }
    }
    */


}