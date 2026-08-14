using System;
using System.Reflection;

namespace ReplayTestMod.Utils
{
    public static class XXLModExtention
    {
        private static object otherSettingsInstance;
        private static FieldInfo replaySpeedField;

        private static bool isInitialized = false;
        private static float originalSpeed = 1.0f;
        private static bool hasSavedOriginal = false;

        public static void GetXXLModSettings()
        {
            try
            {
                // 1. Find XXLMod3's Main class
                Type xxlMainType = Type.GetType("XXLMod3.Main, XXLMod3");
                if (xxlMainType == null)
                {
                    Main.Logger.Log("[XXLModExtention] Could not find XXLMod3.Main type.");
                    return;
                }

                // 2. Get the static 'settings' or 'Settings' field/property on XXLMod3.Main
                object mainSettingsInstance = null;
                FieldInfo mainSettingsField = xxlMainType.GetField("settings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                           ?? xxlMainType.GetField("Settings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                if (mainSettingsField != null)
                {
                    mainSettingsInstance = mainSettingsField.GetValue(null);
                }
                else
                {
                    PropertyInfo mainSettingsProp = xxlMainType.GetProperty("settings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                                 ?? xxlMainType.GetProperty("Settings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if (mainSettingsProp != null)
                    {
                        mainSettingsInstance = mainSettingsProp.GetValue(null);
                    }
                }

                if (mainSettingsInstance == null)
                {
                    Main.Logger.Log("[XXLModExtention] Could not retrieve main settings instance from XXLMod3.");
                    return;
                }

                Type mainSettingsType = mainSettingsInstance.GetType();
                FieldInfo otherSettingsField = mainSettingsType.GetField("OtherSettings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (otherSettingsField != null)
                {
                    otherSettingsInstance = otherSettingsField.GetValue(mainSettingsInstance);
                }
                else
                {
                    PropertyInfo otherSettingsProp = mainSettingsType.GetProperty("OtherSettings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (otherSettingsProp != null)
                    {
                        otherSettingsInstance = otherSettingsProp.GetValue(mainSettingsInstance);
                    }
                }

                if (otherSettingsInstance == null)
                {
                    Main.Logger.Log("[XXLModExtention] Could not retrieve OtherSettings object instance.");
                    return;
                }

                // 4. Get the 'ReplayPlaybackSpeed' field on the OtherSettings instance
                replaySpeedField = otherSettingsInstance.GetType().GetField("ReplayPlaybackSpeed",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (replaySpeedField != null)
                {
                    isInitialized = true;
                    Main.Logger.Log("[XXLModExtention] Successfully targeted XXLMod3.ProfileSettings.OtherSettings.ReplayPlaybackSpeed!");
                }
                else
                {
                    Main.Logger.Log("[XXLModExtention] Could not find field 'ReplayPlaybackSpeed' on OtherSettings.");
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Log($"[XXLModExtention] Initialization exception: {ex.Message}");
            }
        }

        public static void SetXXLSpeed(float targetSpeed)
        {
            if (!isInitialized || otherSettingsInstance == null || replaySpeedField == null) return;

            // Save the original speed setting before overwriting
            if (!hasSavedOriginal)
            {
                originalSpeed = (float)replaySpeedField.GetValue(otherSettingsInstance);
                hasSavedOriginal = true;
            }
            replaySpeedField.SetValue(otherSettingsInstance, targetSpeed);
        }

        public static void RestoreOriginalSpeed()
        {
            if (!isInitialized || otherSettingsInstance == null || replaySpeedField == null || !hasSavedOriginal) return;

            // Restore the player's original setting
            replaySpeedField.SetValue(otherSettingsInstance, originalSpeed);
            hasSavedOriginal = false;
            Main.Logger.Log("[XXLModExtention] Restored original ReplayPlaybackSpeed setting in XXLMod3.");
        }
    }
}