using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using UnityEngine;

namespace ReplayFX.Utils
{
    public static class FloatToColourUtil
    {
        public static FloatToColor customFloatToColor = new FloatToColor();
        public static float GetValueFromColor(Color color)
        {
            float value = customFloatToColor.GetValue(color);
            return value;
        }
        public static Color GetColorFromValue(float value)
        {
            Color color = customFloatToColor.GetColor(value);
            return color;
        }


    }
}
