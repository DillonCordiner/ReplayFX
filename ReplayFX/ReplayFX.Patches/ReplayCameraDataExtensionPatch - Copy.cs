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
    // Remove custom keyframes from base .replay file array to avoid null crashes
    [HarmonyPatch(typeof(ReplayCameraDataExtension), nameof(ReplayCameraDataExtension.CreateFrom))]
    public static class ReplayCameraDataExtension_CreateFrom_Patch
    {
        static bool Prefix(IEnumerable<KeyFrame> keyFrames, ref ReplayCameraData __result)
        {
            __result = new ReplayCameraData
            {
                cameraKeyFrames = keyFrames.Select(k => k.ToSerializatbleKeyframe()).Where(k => k != null).ToArray()
            };
            return false; // Skip
        }
    }
}