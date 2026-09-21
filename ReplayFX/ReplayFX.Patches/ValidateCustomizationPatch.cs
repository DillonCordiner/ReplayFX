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
using ModIO.UI;

namespace ReplayFX.Patches
{
    [HarmonyPatch(typeof(GearDatabase), nameof(GearDatabase.ValidateCustomization),
    new[] { typeof(CustomizedPlayerDataV2), typeof(CustomizedPlayerDataV2), typeof(GearValidationContext) })]
    public static class ValidateCustomizationPatch
    {
        [HarmonyPostfix]
        static void Postfix(ref Task<CustomizedPlayerDataV2> __result, GearValidationContext context)
        {
            //if (context != GearValidationContext.OnlinePlayer)
            //    return;

            if (!Main.settings.useRandomDefaultDanny)
                return;

            __result = RandomizeDefault(__result);
        }

        private static async Task<CustomizedPlayerDataV2> RandomizeDefault(Task<CustomizedPlayerDataV2> originalTask)
        {
            CustomizedPlayerDataV2 validated = await originalTask;

            bool isDefault = validated.GetAllGearInfos().Select(g => g.name).SequenceEqual(CustomizedPlayerDataV2.Default.GetAllGearInfos().Select(g => g.name));
            if (!isDefault)
            {
                return validated;
            }
            Main.Logger.Log("[ValidateCustomization] Randomizing Default Gear.");
            return GearUtil.RandomizeGear(CustomizedPlayerDataV2.Default);
        }
    }
}