using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;

namespace ReplayFX.Patches
{
    [HarmonyPatch(typeof(ReplayMenuController), "CloseAll")]
    public static class ReplayMenuController_CloseAll_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ReplayMenuController __instance)
        {
            Main.replayfxMenu.clonedMenu.gameObject.SetActive(false);
            return true;
        }
    }
    [HarmonyPatch(typeof(ReplayMenuController), "OpenMainMenu")]
    public static class ReplayMenuController_OpenMainMenu_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ReplayMenuController __instance)
        {
            Main.replayfxMenu.clonedMenu.gameObject.SetActive(false);
            return true;
        }
    }
    [HarmonyPatch(typeof(ReplayMenuController), "OpenSaveMenu")]
    public static class ReplayMenuController_OpenSaveMenu_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ReplayMenuController __instance)
        {
            Main.replayfxMenu.clonedMenu.gameObject.SetActive(false);
            return true;
        }
    }
    [HarmonyPatch(typeof(ReplayMenuController), "OpenSettingsMenu")]
    public static class ReplayMenuController_OpenSettingsMenu_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ReplayMenuController __instance)
        {
            Main.replayfxMenu.clonedMenu.gameObject.SetActive(false);
            return true;
        }
    }
    
}