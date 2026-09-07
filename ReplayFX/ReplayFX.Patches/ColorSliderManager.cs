using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using ReplayFX.Utils;

namespace ReplayFX.Patches
{
    public static class ColorSliderManager
    {
        // 1 = Playback, 2 = Impulse
        public static Dictionary<int, int> SliderModes = new Dictionary<int, int>();
    }

    [HarmonyPatch(typeof(ColorSliderItem), nameof(ColorSliderItem.UpdateItem))]
    public static class ColorSliderItem_UpdateItem_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ColorSliderItem __instance)
        {
            var floatToColor = __instance.selectable.GetComponent<FloatToColor>();
            if (floatToColor == null) return;

            // Tag the slider ID based on its label when the UI page updates
            string label = __instance.selectable.label.text;
            if (label.Contains("Playback"))
                ColorSliderManager.SliderModes[floatToColor.GetInstanceID()] = 1;
            else if (label.Contains("Impulse"))
                ColorSliderManager.SliderModes[floatToColor.GetInstanceID()] = 2;

            floatToColor.UpdateValue(__instance.selectable.value);
        }
    }

    [HarmonyPatch(typeof(FloatToColor))]
    public static class FloatToColor_Color_Patches
    {
        [HarmonyPatch(nameof(FloatToColor.GetColor))]
        [HarmonyPrefix]
        public static bool Prefix(float value, ref Color __result, FloatToColor __instance)
        {
            if (ColorSliderManager.SliderModes.TryGetValue(__instance.GetInstanceID(), out int mode))
            {
                if (mode == 1) // Playback Slider
                {
                    __result = Main.settings.isPlaybackGreyscale ? ColorUtil.FloatToGrayscale(value) : ColorUtil.FloatToRGB(value);
                    return false;
                }
                if (mode == 2) // Impulse Slider
                {
                    __result = Main.settings.isImpulseGreyscale ? ColorUtil.FloatToGrayscale(value) : ColorUtil.FloatToRGB(value);
                    return false;
                }
            }
            return true;
        }

        [HarmonyPatch(nameof(FloatToColor.GetValue))]
        [HarmonyPrefix]
        public static bool Prefix(Color value, ref float __result, FloatToColor __instance)
        {
            if (ColorSliderManager.SliderModes.TryGetValue(__instance.GetInstanceID(), out int mode))
            {
                if (mode == 1)
                {
                    __result = Main.settings.isPlaybackGreyscale ? ColorUtil.GrayscaleToFloat(value) : ColorUtil.RGBToFloat(value);
                    return false;
                }
                if (mode == 2)
                {
                    __result = Main.settings.isImpulseGreyscale ? ColorUtil.GrayscaleToFloat(value) : ColorUtil.RGBToFloat(value);
                    return false;
                }
            }
            return true;
        }
    }
}