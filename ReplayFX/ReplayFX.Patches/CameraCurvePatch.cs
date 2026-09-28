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
    [HarmonyPatch(typeof(CameraCurve), nameof(CameraCurve.DeleteCurveKeys))]
    public static class Patch_CameraCurve_DeleteCurveKeys
    {
        static bool Prefix(CameraCurve __instance, int i, bool refreshDirectly)
        {
            var keyFrames = ReplayEditorController.Instance.cameraController.keyFrames;

            // 1. Failsafe: Prevent out of bounds
            if (i < 0 || i >= keyFrames.Count) return false;

            var keyToDelete = keyFrames[i];
            bool isCustomKey = (keyToDelete is PlaybackSpeedKeyFrame || keyToDelete is ImpulseKeyFrame);

            // 2. If deleting a custom key, skip curve deletion entirely.
            // The ReplayEditorController will still remove it from the keyFrames list natively.
            if (isCustomKey)
            {
                return false;
            }

            // 3. If deleting a standard camera key, we must translate the UI list index
            // into the actual AnimationCurve index by counting only standard keys.
            int actualCurveIndex = 0;
            for (int listIndex = 0; listIndex < i; listIndex++)
            {
                var k = keyFrames[listIndex];
                if (!(k is PlaybackSpeedKeyFrame || k is ImpulseKeyFrame))
                {
                    actualCurveIndex++;
                }
            }

            // 4. Perform the deletion on all curves using the translated index
            __instance.orientationCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.focusYOffsetCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.positionCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.fovCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.radiusCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.freeCamCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.orbitCamCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);
            __instance.tripodCamCurve.DeleteCurveKey(actualCurveIndex, refreshDirectly);

            // 5. Skip the original method since we handled it manually
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