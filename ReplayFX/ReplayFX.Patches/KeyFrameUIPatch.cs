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
using UnityEngine.UI;

namespace ReplayFX.Patches
{
    
    [HarmonyPatch(typeof(KeyframeUIController), nameof(KeyframeUIController.UpdateKeyframes))]
    public static class KeyframeUIPatch
    {
        /*
        [HarmonyPrefix]
        static bool Prefix(ref KeyframeUIController __instance)
        {
            return true;
        }
        */
    }
    
}