using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using UnityEngine;

namespace ReplayFX.Utils
{
    public static class ColorUtil
    {
        public static Color FloatToRGB(float value, float min = 0f, float max = 1f)
        {
            // normalize value to a 0.0 - 1.0 scale, clamping it if it goes out of bounds.
            float hue = Mathf.InverseLerp(min, max, value); // 1f for Saturation means full color, 1f for Value means full brightness.
            return Color.HSVToRGB(hue, 1f, 1f);
        }
        public static Color FloatToGrayscale(float value, float min = 0f, float max = 1f)
        {
            float t = Mathf.InverseLerp(min, max, value);
            return Color.Lerp(Color.white, Color.black, t);
        }
        public static Color FloatToRGBLooping(float value)
        {
            float hue = Mathf.Repeat(value, 1f);

            return Color.HSVToRGB(hue, 1f, 1f);
        }
    }
}
