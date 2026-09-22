using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;

namespace ReplayFX.Patches
{
    /*
    [HarmonyPatch(typeof(ReplayCameraController), "ForceCameraPosition")]
    public static class ForceCameraPositionPatch
    {
        [HarmonyPrefix]
        static bool Prefix(ReplayCameraController __instance, ref Vector3 pos, ref Quaternion rot)
        {
            __instance.VirtualCamera.UpdateCameraState(Vector3.up, ReplayEditorController.Instance.playbackController.CurrentTime);
            Main.Logger.Log("[ForceCameraPositionPatch] Updated Camera State");
            return true;
        }
    }
    */
}