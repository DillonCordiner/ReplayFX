using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using ReplayTestMod.Utils;

namespace ReplayTestMod.Patches
{
    /*
    [HarmonyPatch(typeof(ReplayCameraController), "DeleteKeyFrame")]
    public static class ReplayCameraControllerPatch
    {
        [HarmonyPostfix]
        static void Postfix(ReplayCameraController __instance)
        {
            CurveUtil.Refresh();
        }
    }
    */
}