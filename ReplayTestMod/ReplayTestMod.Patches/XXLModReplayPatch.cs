using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using ReplayTestMod.Utils;

namespace ReplayTestMod.Patches
{
    /// <summary>
    /// Set up with XXL mod references and only run post fix if XXL mod is installed
    /// </summary>

    [HarmonyPatch(typeof(CameraCurve), "")]
    public static class XXLModReplayPatch
    {
        [HarmonyPostfix]
        static void Postfix(CameraCurve __instance)
        {
        }
    }
}