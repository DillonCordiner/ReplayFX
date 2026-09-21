using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using Photon.Pun;
using SkaterXL.Data;
using ReplayFX.Utils;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;

namespace ReplayFX.Patches
{
    /*
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

            //CustomizedPlayerDataV2 data = customizations;
            //string currentGear = JsonUtility.ToJson(__instance.customizer.CurrentCustomizations);
            //string currentGear = __instance.customizer.CurrentCustomizations.ToJsonString();

            //string currentGear = __instance.customizer.CurrentCustomizations.ToString();
            //string currentGear = customizations.ToString();
            //string currentGear = data.ToString();
            //string defaultGear = CustomizedPlayerDataV2.Default.ToString();

            bool listsMatch = customizations.GetAllGearInfos().Select(g => g.name).SequenceEqual(CustomizedPlayerDataV2.Default.GetAllGearInfos().Select(g => g.name));

            if (listsMatch)//currentGear == defaultGear
            {
                customizations = GearUtil.RandomizeGear(CustomizedPlayerDataV2.Default);
                //customizations = GearUtil.RandomizeGear(customizations);
                Main.Logger.Log($"[Network Customization] Random Default Gear loaded for player: {info.Sender.NickName}");
            }
            else
            {
                Main.Logger.Log($"[Network Customization] Failed to Randomize Default Gear: **** {CustomizedPlayerDataV2.Default} **** current Gear: **** {customizations} **** for player: {info.Sender.NickName}");
            }
            return true;
        }
        
        
        [HarmonyPostfix]
        static void Postfix(ref NetworkPlayerCustomisation __instance, ref CustomizedPlayerDataV2 customizations, PhotonMessageInfo info)
        {
            RandomizeDefaultGear(__instance, customizations, info);
        }

        private static async void RandomizeDefaultGear(NetworkPlayerCustomisation __instance, CustomizedPlayerDataV2 customizations, PhotonMessageInfo info)
        {
            if (info.Sender != null)
            {
                Main.Logger.Log($"[Network Customization] Intercepted gear load RPC from player: {info.Sender.NickName}");
            }

            //customizations = await GearDatabase.Instance.ValidateCustomization(customizations, CustomizedPlayerDataV2.Default, GearValidationContext.OnlinePlayer);

            while (__instance.customizer != null && __instance.customizer.IsLoading)
            {
                await Task.Yield();
            }

            bool listsMatch = customizations.GetAllGearInfos().Select(g => g.name).SequenceEqual(CustomizedPlayerDataV2.Default.GetAllGearInfos().Select(g => g.name));

            if (listsMatch)//currentGear == defaultGear
            {
                customizations = GearUtil.RandomizeGear(CustomizedPlayerDataV2.Default);

                //await __instance.customizer.RemoveAllGear();
                //foreach (GearInfo gearinfo in customizations.GetAllGearInfos()) { __instance.customizer.LoadGearAsync(gearinfo); }
                //foreach (GearInfo gearinfo in customizations.GetAllGearInfos()) { __instance.customizer.EquipGear(gearinfo); }

                __instance.customizer.LoadCustomizations(customizations);

                Main.Logger.Log($"[Network Customization] Random Default Gear loaded for player: {info.Sender.NickName}");
            }
            else
            {
                Main.Logger.Log($"[Network Customization] Failed to Randomize Default Gear: **** {CustomizedPlayerDataV2.Default} **** current Gear: **** {customizations} **** for player: {info.Sender.NickName}");
            }
        }
        
    }
    */
    
}