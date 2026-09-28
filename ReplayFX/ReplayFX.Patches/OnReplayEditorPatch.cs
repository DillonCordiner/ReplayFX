using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ReplayEditor;
using ReplayFX.Keyframes;
using ReplayFX.Utils;
using RootMotion;
using SkaterXL.Data;
using UnityEngine;

namespace ReplayFX.Patches
{
    
    [HarmonyPatch(typeof(ReplayCameraController), nameof(ReplayCameraController.OnReplayEditorStart))]
    public static class OnReplayEditorStartPatch
    {
        /*
        [HarmonyPrefix]
        static bool Prefix(ref ReplayCameraController __instance)
        {
            CurveUtil.Refresh();
            return true;
        }
        */

        [HarmonyPostfix]
        static void Postfix(ref ReplayCameraController __instance)
        {
            CurveUtil.Refresh();
        }
    }
    
}