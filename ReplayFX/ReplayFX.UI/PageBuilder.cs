using UnityEngine;
using RapidGUI;
using System;
using System.Threading.Tasks;
using Rewired;
using UnityEngine.Events;
using TMPro;
using ReplayFX.Keyframes;

namespace ReplayFX.UI
{
    public static class PageBuilder
    {
        public static readonly string cameraSettings = "Camera Settings";
        public static readonly string keyframeSettings = "KeyFrame Settings";

        public static MenuButton CreateButton(MenuButton originalButton, string label, UnityAction buttonAction)
        {
            GameObject newButtonObj = UnityEngine.Object.Instantiate(originalButton.gameObject, originalButton.gameObject.transform.parent);
            //newButtonObj.transform.SetSiblingIndex(originalButton.transform.GetSiblingIndex() + 1);
            newButtonObj.transform.SetAsFirstSibling();
            newButtonObj.name = label;

            MenuButton newButton;
            newButton = newButtonObj.GetComponent<MenuButton>();
            newButton.GreyedOut = false;
            newButton.GreyedOutInfoText = label;
            //newButton.Label.SetText(label);
            newButton.SetText(label);

            newButton.onClick.RemoveAllListeners();  // Remove existing listeners
            newButton.onClick.SetPersistentListenerState(0, UnityEventCallState.Off); // removes persistant listeners that are set in unity editor.
            newButton.onClick.AddListener(buttonAction);  // Add new listener

            return newButton;
        }
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
            await proceduralMenuPage.AddIntSetting("noise_amplitude", "Amplitude", () => Mathf.RoundToInt(Main.settings.noise_amplitude * 10f), delegate (int v)
            {
                Main.settings.noise_amplitude = v / 10f;
            }, 0, 100, "{0}%", int.MaxValue);

            await proceduralMenuPage.AddIntSetting("noise_frequency", "Frequency", () => Mathf.RoundToInt(Main.settings.noise_frequency * 10f), delegate (int v)
            {
                Main.settings.noise_frequency = v / 10f;
            }, 0, 100, "{0}%", int.MaxValue);

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

            await proceduralMenuPage.AddIntSetting("impulse_force", "Impulse Force", () => Mathf.RoundToInt(Main.settings.impulse_force * 10f), delegate (int v)
            {
                Main.settings.impulse_force = v / 10f;
            }, 0, 100, "{0}%", int.MaxValue);

            await proceduralMenuPage.AddIntSetting("impulse_amplitude", "Impulse Amplitude", () => Mathf.RoundToInt(Main.settings.impulse_source_amplitude * 10f), delegate (int v)
            {
                Main.settings.impulse_source_amplitude = v / 10f;
            }, 0, 100, "{0}%", int.MaxValue);

            await proceduralMenuPage.AddIntSetting("impulse_frequency", "Impulse Frequency", () => Mathf.RoundToInt(Main.settings.impulse_source_frequency * 10f), delegate (int v)
            {
                Main.settings.impulse_source_frequency = v / 10f;
            }, 0, 100, "{0}%", int.MaxValue);

            await proceduralMenuPage.AddIntSetting("impulse_decay", "Impulse Decay", () => Mathf.RoundToInt(Main.settings.impulse_source_decaytime * 100f), delegate (int v)
            {
                Main.settings.impulse_source_decaytime = v / 100f;
            }, 0, 200, "{0}%", int.MaxValue);

            //await proceduralMenuPage.AddButton("impulse_test", "Test Impulse", () => Main.noiseController.GenerateImpluse(), int.MaxValue);

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
