using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using Photon.Pun;
using SkaterXL.Data;
using ReplayFX.Utils;
using System.Linq;

namespace ReplayFX.Patches
{
    
    [HarmonyPatch(typeof(NetworkPlayerCustomisation), nameof(NetworkPlayerCustomisation.rpcSetCustomization))]
    public static class rpcSetCustomizationPatch
    {
        [HarmonyPrefix]
        static bool Prefix(NetworkPlayerCustomisation __instance, ref CustomizedPlayerDataV2 customizations, PhotonMessageInfo info)
        {
            if (info.Sender != null)
            {
                Main.Logger.Log($"[Network Customization] Intercepted gear load RPC from player: {info.Sender.NickName}");
            }

            string currentGear = JsonUtility.ToJson(__instance.customizer.CurrentCustomizations);
            string defaultGear = JsonUtility.ToJson(CustomizedPlayerDataV2.Default);

            if (currentGear == defaultGear)
            {
                customizations = GearUtil.RandomizeGear(CustomizedPlayerDataV2.Default);
                Main.Logger.Log($"[Network Customization] Random Default Gear loaded for player: {info.Sender.NickName}");
            }
            else
            {
                Main.Logger.Log($"[Network Customization] Failed to Randomize {defaultGear} current Gear: {currentGear} for player: {info.Sender.NickName}");
            }
            return true;
        }
    }
    
}