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
        public static float RGBToFloat(Color color, float min = 0f, float max = 1f)
        {
            Color.RGBToHSV(color, out float hue, out float saturation, out float value);
            return Mathf.Lerp(min, max, hue);
        }
        public static Color FloatToGrayscale(float value, float min = 0f, float max = 1f)
        {
            float t = Mathf.InverseLerp(min, max, value);
            return Color.Lerp(Color.white, Color.black, t);
        }     
        public static float GrayscaleToFloat(Color color, float min = 0f, float max = 1f)
        {
            float t = 1f - color.grayscale;
            return Mathf.Lerp(min, max, t);
        }
        public static Color FloatToRGBLooping(float value)
        {
            float hue = Mathf.Repeat(value, 1f);

            return Color.HSVToRGB(hue, 1f, 1f);
        }
        public static Color GetColorFromValue(bool greyscale, float value)
        {
            return greyscale ? FloatToGrayscale(value) : FloatToRGB(value);
        }

        public static float SetValueFromColor(bool greyscale, Color color)
        {
            return greyscale ? GrayscaleToFloat(color) : RGBToFloat(color);
        }
    }
}
