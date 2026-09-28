using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using ReplayFX.Utils;
using ReplayFX.Keyframes;

namespace ReplayFX.Patches
{
    
    [HarmonyPatch(typeof(CameraCurve), "CalculateCurveControlPoints")]
    public static class CameraCurveControlPoint_Patch
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
    public static class CameraCurveClear_Patch
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
    [HarmonyPatch(typeof(CameraCurve), nameof(CameraCurve.DeleteCurveKeys))]
    public static class CameraCurveDeleteCurveKeys_Patch
    {
        static bool Prefix(CameraCurve __instance, int i, bool refreshDirectly)
        {
            var keyFrames = ReplayEditorController.Instance.cameraController.keyFrames;

            if (i < 0 || i >= keyFrames.Count) return false;

            var keyToDelete = keyFrames[i];
            bool isCustomKey = (keyToDelete is PlaybackSpeedKeyFrame || keyToDelete is ImpulseKeyFrame);

            if (isCustomKey)
            {
                return false;
            }

            // If deleting a standard camera key  translate the UI list index into the actual AnimationCurve index by counting only standard keys.
            int actualCurveIndex = 0;
            for (int listIndex = 0; listIndex < i; listIndex++)
            {
                var k = keyFrames[listIndex];
                if (!(k is PlaybackSpeedKeyFrame || k is ImpulseKeyFrame))
                {
                    actualCurveIndex++;
                }
            }

            __instance.orientationCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.focusYOffsetCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.positionCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.fovCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.radiusCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.freeCamCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.orbitCamCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.tripodCamCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);

            return false;
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