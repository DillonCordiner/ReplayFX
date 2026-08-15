using System;
using UnityEngine;
using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;
using ReplayTestMod.Utils;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ReplayTestMod.Patches
{
    /*

    [HarmonyPatch(typeof(KeyframeUIController), nameof(KeyframeUIController.UpdateKeyframes))]
    public static class KeyframeColorPatch
    {
        // Custom color for Playback Speed keyframes (e.g., Bright Cyan)
        private static readonly Color SpeedKeyFrameColor = new Color(0f, 0.8f, 1f, 1f);

        // Standard game color for normal keyframes (Default Red)
        private static readonly Color DefaultKeyFrameColor = new Color(1f, 0.2f, 0.2f, 1f);

        static void Postfix(KeyframeUIController __instance, IEnumerable<KeyFrame> keyframes)
        {
            if (keyframes == null || __instance.keyframeSliders == null) return;

            int index = 0;
            foreach (KeyFrame keyFrame in keyframes)
            {
                // Safety check bounds
                if (index >= __instance.keyframeSliders.Count) break;

                Slider slider = __instance.keyframeSliders[index];
                if (slider != null)
                {
                    // Find the handle graphic (typically slider.handleRect)
                    Image handleImage = slider.handleRect != null ? slider.handleRect.GetComponent<Image>() : null;

                    if (handleImage != null)
                    {
                        // Determine target color based on KeyFrame type
                        Color targetColor = (keyFrame is PlaybackSpeedKeyFrame) ? SpeedKeyFrameColor : DefaultKeyFrameColor;

                        // 1. Set immediate graphic color
                        handleImage.color = targetColor;

                        // 2. Modify Selectable ColorBlock so Unity UI doesn't overwrite it back to Red
                        ColorBlock cb = slider.colors;
                        cb.normalColor = targetColor;
                        cb.highlightedColor = targetColor;
                        cb.selectedColor = Color.yellow; // Keep yellow highlight on click/selection
                        slider.colors = cb;
                    }
                }

                index++;
            }
        }
    }
    */
}