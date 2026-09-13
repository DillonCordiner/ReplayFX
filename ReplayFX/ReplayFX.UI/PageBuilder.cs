using UnityEngine;
using RapidGUI;
using System;
using System.Threading.Tasks;
using Rewired;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using ReplayFX.Keyframes;
using ReplayFX.Utils;

namespace ReplayFX.UI
{
    public static class PageBuilder
    {
        public static readonly string cameraSettings = "Camera Settings";
        public static readonly string keyframeSettings = "Keyframe Settings";
        public static readonly string ColorSettings = "Color Settings";
        public static readonly string OtherSettings = "Other Settings";

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
            ProceduralMenuPage proceduralMenuPage = await Main.replayfxMenu.CreateSettingsPage(cameraSettings, -1);

            await proceduralMenuPage.AddBoolSetting("enable_noise", "Camera Shake", () => GetEnableNoise(), (val) => SetEnableNoise(val), "Enabled", "Disabled", int.MaxValue);
            /*
            await cameraSettings.AddBoolSetting("test_bool", "test_bool", () => Main.settings.enableNoise, delegate (bool v)
            {         
                Main.camController.ToggleNoise();
                v = Main.settings.enableNoise;
            }, "Enabled", "Disabled", int.MaxValue);
            */

            await proceduralMenuPage.AddStringEnumSetting("camera_profile", "Camera Profile", () => GetCameraProfileItem(), (name) => SetCameraProfileItem(name), Main.camController.ProfileOptionsArray);

            /*
            await cameraSettings.AddStringEnumSetting("test_string", "test_string", () => Main.camController.targetProfile, delegate (string v)
            {
                Main.camController.targetProfile = v;
            }, Main.camController.ProfileOptionsArray);
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
            //ProceduralMenuPage proceduralMenuPage = await SettingsMenuController.Instance.CreateSettingsPage("Keyframe Settings", -1);
            ProceduralMenuPage proceduralMenuPage = await Main.replayfxMenu.CreateSettingsPage(keyframeSettings, -1);

            await proceduralMenuPage.AddIntSetting("playback_speed", "Playback Key Speed", () => Mathf.RoundToInt(Main.settings.replay_playback_speed * 100f), delegate (int v)
            {
                Main.settings.replay_playback_speed = v / 100f;
            }, 0, 200, "{0}%", int.MaxValue);

            await proceduralMenuPage.AddIntSetting("impulse_force", "Impulse Key Force", () => Mathf.RoundToInt(Main.settings.impulse_force * 10f), delegate (int v)
            {
                Main.settings.impulse_force = v / 10f;
            }, 0, 100, "{0}%", int.MaxValue);

            await proceduralMenuPage.AddIntSetting("impulse_amplitude", "Impulse Key Amplitude", () => Mathf.RoundToInt(Main.settings.impulse_source_amplitude * 10f), delegate (int v)
            {
                Main.settings.impulse_source_amplitude = v / 10f;
            }, 0, 100, "{0}%", int.MaxValue);

            await proceduralMenuPage.AddIntSetting("impulse_frequency", "Impulse Key Frequency", () => Mathf.RoundToInt(Main.settings.impulse_source_frequency * 10f), delegate (int v)
            {
                Main.settings.impulse_source_frequency = v / 10f;
            }, 0, 100, "{0}%", int.MaxValue);

            await proceduralMenuPage.AddIntSetting("impulse_decay", "Impulse Key Decay", () => Mathf.RoundToInt(Main.settings.impulse_source_decaytime * 100f), delegate (int v)
            {
                Main.settings.impulse_source_decaytime = v / 100f;
            }, 0, 200, "{0}%", int.MaxValue);

            await AddBoolButton(proceduralMenuPage, "impulse_test",  "Test Impulse", () => false, delegate (bool v) { TestImpulseButton(v); });

            //await proceduralMenuPage.AddButton("impulse_test", "Test Impulse", () => Main.camController.GenerateImpluse(), int.MaxValue);

            /*
            await keyframeSettings.AddFloatSetting("test_slider", "Test Slider", () => Main.settings.replay_playback_speed, delegate (float v)
            {
                Main.settings.replay_playback_speed = v;
            }, 0.0f, 2.0f, 2);
            */
            //await keyframeSettings.AddFloatSetting("test_slider", "Test Slider", () => GetTestSlider(), (val) => SetTestSlider(val), 0f, 1f, 2);

            return proceduralMenuPage;
        }
        public static async Task<ProceduralMenuPage> BuildColorPageAsync()
        {
            ProceduralMenuPage proceduralMenuPage = await Main.replayfxMenu.CreateSettingsPage(ColorSettings, -1);

            await proceduralMenuPage.AddColorSetting("playback_color", "Playback Key Color", () => GetPlaybackColorItem(), (color) => SetPlaybackColorItem(color), FloatToColor.ConversionType.Hue, int.MaxValue);
            await proceduralMenuPage.AddBoolSetting("playback_greyscale", "Color Mode", () => GetPlaybackGreyScale(), (val) => SetPlaybackGreyScale(val), "Greyscale", "RGB", int.MaxValue);

            await proceduralMenuPage.AddColorSetting("impulse_color", "Impulse Key Color", () => GetImpulseColorItem(), (color) => SetImpulseColorItem(color), FloatToColor.ConversionType.Hue, int.MaxValue);
            await proceduralMenuPage.AddBoolSetting("impulse_greyscale", "Color Mode", () => GetImpulseGreyScale(), (val) => SetImpulseGreyScale(val), "Greyscale", "RGB", int.MaxValue);
            /*
            await proceduralMenuPage.AddBoolSetting("playback_greyscale", "Color Type", () => Main.settings.isPlaybackGreyscale, delegate (bool v)
            {
                Main.settings.isPlaybackGreyscale = v;
                proceduralMenuPage.UpdatePage();
            }, "Greyscale", "RGB", int.MaxValue);
            */
            /*
            await proceduralMenuPage.AddColorSetting("playback_color", "Playback Key Color", () => Main.settings.playback_key_color, delegate (Color v)
            {
                //Color color = ColorUtil.FloatToRGB(Main.settings.playback_color_value);
                Main.settings.playback_key_color = v;
            }, FloatToColor.ConversionType.Hue, int.MaxValue);
            */

            return proceduralMenuPage;
        }
        public static async Task<ProceduralMenuPage> BuildOtherPageAsync()
        {
            ProceduralMenuPage proceduralMenuPage = await Main.replayfxMenu.CreateSettingsPage(OtherSettings, -1);

            await proceduralMenuPage.AddStringEnumSetting("replay_fps", "Recorded FPS", () => GetFPSItem(), (name) => SetFPSItem(name), Main.camController.recordedFPSarray);

            await AddBoolButton(proceduralMenuPage, "delete_playback", "Delete All Playback Keys", () => false, delegate (bool v) { DeleteAllPlaybackKeysButton(v); });
            await AddBoolButton(proceduralMenuPage, "delete_impulse", "Delete All Impulse Keys", () => false, delegate (bool v) { DeleteAllImpulseKeysButton(v); });

            //await AddBoolButton(proceduralMenuPage, "reload_gear", "Reload Custom Gear", () => GetReloadGearButton(), (val) => SetReloadGearButton(val));
            await AddBoolButton(proceduralMenuPage, "reload_gear", "Reload Custom Gear", () => false, delegate (bool v) { ReloadGearButton(v); });
            return proceduralMenuPage;
        }
        private static async Task<ProceduralMenuPage> AddBoolButton(ProceduralMenuPage page, string id, string label, Func<bool> getter, Action<bool> setter)
        {
            // The "" arguments natively remove the "On"/"Off" text visuals
            var item = await page.AddBoolSetting(id, label, getter, setter, "", "", int.MaxValue);

            ToggleItem toggleItem = item as ToggleItem;
            if (toggleItem != null && toggleItem.selectable != null)
            {
                toggleItem.selectable.gameObject.AddComponent<ButtonMarker>();
            }
            return page;
        }

        private static bool GetEnableNoise() => Main.settings.enableNoise;
        private static void SetEnableNoise(bool val)
        {
            if (Main.camController != null)
            {
                Main.camController.ToggleNoise();
            }
        }
        private static void ReloadGearButton(bool val)
        {
            GearUtil.ReloadOnStateExit();
        }
        private static void TestImpulseButton(bool val)
        {
            Main.camController.GenerateImpluse();
        }
        private static void DeleteAllPlaybackKeysButton(bool val)
        {
            KeyFrameHelper.RemoveAllPlaybackKeys();
        }
        private static void DeleteAllImpulseKeysButton(bool val)
        {
            KeyFrameHelper.RemoveAllImpulseKeys();
        }
        private static string GetCameraProfileItem() => Main.camController.targetProfile;
        private static void SetCameraProfileItem(string name)
        {
            Main.camController.targetProfile = name;
            //Main.rfxSettings.cameraMenuPage.UpdateItem("camera_profile");
        }
        private static string GetFPSItem() => Main.settings.replay_recorded_fps.ToString();
        private static void SetFPSItem(string name)
        {
            int.TryParse(name, out Main.settings.replay_recorded_fps);
        }
        private static bool GetPlaybackGreyScale() => Main.settings.isPlaybackGreyscale;
        private static void SetPlaybackGreyScale(bool val)
        {
            Main.settings.isPlaybackGreyscale = val;

            if (Main.replayfxMenu.colorMenuPage != null)
            {
                Main.replayfxMenu.colorMenuPage.UpdatePage();
            }
        }
        private static bool GetImpulseGreyScale() => Main.settings.isImpulseGreyscale;
        private static void SetImpulseGreyScale(bool val)
        {
            Main.settings.isImpulseGreyscale = val;

            if (Main.replayfxMenu.colorMenuPage != null)
            {
                Main.replayfxMenu.colorMenuPage.UpdatePage();
            }
        }
        private static Color GetPlaybackColorItem()
        {
            return ColorUtil.GetColorFromValue(Main.settings.isPlaybackGreyscale, Main.settings.playback_color_value);
            //return Main.settings.isPlaybackGreyscale ? ColorUtil.FloatToGrayscale(Main.settings.playback_color_value) : ColorUtil.FloatToRGB(Main.settings.playback_color_value);
        }
        private static void SetPlaybackColorItem(Color color)
        {
            Main.settings.playback_color_value = ColorUtil.SetValueFromColor(Main.settings.isPlaybackGreyscale, color);
            //Main.settings.playback_color_value = Main.settings.isPlaybackGreyscale ? ColorUtil.GrayscaleToFloat(color) : ColorUtil.RGBToFloat(color);
        }
        private static Color GetImpulseColorItem()
        {
            return ColorUtil.GetColorFromValue(Main.settings.isImpulseGreyscale, Main.settings.impulse_color_value);
        }
        private static void SetImpulseColorItem(Color color)
        {
            Main.settings.impulse_color_value = ColorUtil.SetValueFromColor(Main.settings.isImpulseGreyscale, color);
        }
      
        private static float GetTestSlider() => 0.5f;
        private static void SetTestSlider(float val)
        {
            Main.Logger.Log("Set Slider");
        }
    }
}
