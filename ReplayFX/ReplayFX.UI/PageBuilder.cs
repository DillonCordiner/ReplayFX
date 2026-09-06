using UnityEngine;
using RapidGUI;
using System;
using System.Threading.Tasks;
using Rewired;

namespace ReplayFX.UI
{
    public static class PageBuilder
    {
        public static readonly string cameraSettings = "Camera Settings";
        public static readonly string keyframeSettings = "KeyFrame Settings";

        public static async Task<ProceduralMenuPage> BuildCameraPageAsync()
        {
            //ProceduralMenuPage proceduralMenuPage = await SettingsMenuController.Instance.CreateSettingsPage("Camera Settings", -1);
            ProceduralMenuPage proceduralMenuPage = await Main.rfxSettings.CreateSettingsPage(cameraSettings, -1);

            await proceduralMenuPage.AddBoolSetting("enable_noise", "Camera Shake", () => GetEnableNoise(), (val) => SetEnableNoise(val), "Enabled", "Disabled", int.MaxValue);
            /*
            await cameraSettings.AddBoolSetting("test_bool", "test_bool", () => Main.settings.enableNoise, delegate (bool v)
            {         
                Main.noiseController.ToggleNoise();
                v = Main.settings.enableNoise;
            }, "Enabled", "Disabled", int.MaxValue);
            */

            await proceduralMenuPage.AddStringEnumSetting("camera_profile", "Camera Profile", () => GetCameraProfileItem(), (name) => SetCameraProfileItem(name), Main.noiseController.ProfileOptionsArray);
            /*
            await cameraSettings.AddStringEnumSetting("test_string", "test_string", () => Main.noiseController.targetProfile, delegate (string v)
            {
                Main.noiseController.targetProfile = v;
            }, Main.noiseController.ProfileOptionsArray);
            */
            return proceduralMenuPage;
        }
        public static async Task<ProceduralMenuPage> BuildKeyframePageAsync()
        {
            //ProceduralMenuPage proceduralMenuPage = await SettingsMenuController.Instance.CreateSettingsPage("KeyFrame Settings", -1);
            ProceduralMenuPage proceduralMenuPage = await Main.rfxSettings.CreateSettingsPage(keyframeSettings, -1);

            await proceduralMenuPage.AddIntSetting("playback_speed", "Playback Speed", () => Mathf.RoundToInt(Main.settings.replay_playback_speed * 100f), delegate (int v)
            {
                Main.settings.replay_playback_speed = v / 100f;
            }, 0, 200, "{0}%", int.MaxValue);
            /*
            await keyframeSettings.AddFloatSetting("test_slider", "Test Slider", () => Main.settings.replay_playback_speed, delegate (float v)
            {
                Main.settings.replay_playback_speed = v;
            }, 0.0f, 2.0f, 2);
            */
            //await keyframeSettings.AddFloatSetting("test_slider", "Test Slider", () => GetTestSlider(), (val) => SetTestSlider(val), 0f, 1f, 2);

            return proceduralMenuPage;
        }
        private static bool GetEnableNoise() => Main.settings.enableNoise;
        private static void SetEnableNoise(bool val)
        {
            Main.noiseController.ToggleNoise();
            Main.Logger.Log("Enable Noise");
        }
        private static string GetCameraProfileItem() => Main.noiseController.targetProfile;
        private static void SetCameraProfileItem(string name)
        {
            Main.noiseController.targetProfile = name;
            Main.Logger.Log("Camera Profile");
        }
        private static float GetTestSlider() => 0.5f;
        private static void SetTestSlider(float val)
        {
            Main.Logger.Log("Set Slider");
        }

      
    }
}
