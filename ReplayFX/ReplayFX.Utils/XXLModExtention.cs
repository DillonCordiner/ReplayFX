using System;
using System.Linq;
using System.Reflection;

namespace ReplayFX.Utils
{
    public static class XXLModExtention
    {
        public static readonly string XXLmodID = "XXLMod3";
        public static bool IsXXLModInstalled = false;

        private static object otherSettingsInstance;
        private static FieldInfo replaySpeedField;

        private static bool isInitialized = false;
        private static float originalSpeed = 1.0f;
        private static bool hasSavedOriginal = false;

        public static void GetXXLModSettings()
        {
            try
            {
                Type xxlMain = Type.GetType("XXLMod3.Main, XXLMod3");
                if (xxlMain == null)
                {
                    Main.Logger.Log("[XXLModExtention] Could not find XXLMod3.Main type.");
                    return;
                }

                object settingsInstance = null;
                FieldInfo settingsField = xxlMain.GetField("settings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static) ?? xxlMain.GetField("Settings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                if (settingsField != null)
                {
                    settingsInstance = settingsField.GetValue(null);
                    //Main.Logger.Log("[XXLModExtention] Settings is a Field");
                }

                if (settingsInstance == null)
                {
                    Main.Logger.Log("[XXLModExtention] Could not find settings from XXLMod3.");
                    return;
                }

                Type settingsType = settingsInstance.GetType();
                FieldInfo otherSettingsField = settingsType.GetField("OtherSettings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (otherSettingsField != null)
                {
                    otherSettingsInstance = otherSettingsField.GetValue(settingsInstance);
                    //Main.Logger.Log("[XXLModExtention] OtherSettings is a Field");
                }

                if (otherSettingsInstance == null)
                {
                    Main.Logger.Log("[XXLModExtention] Could not find OtherSettings");
                    return;
                }

                replaySpeedField = otherSettingsInstance.GetType().GetField("ReplayPlaybackSpeed",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (replaySpeedField != null)
                {
                    isInitialized = true;
                    Main.Logger.Log("[XXLModExtention] Successfully found ReplayPlaybackSpeed");
                    replaySpeedField.SetValue(otherSettingsInstance, 1.0f);
                }
                else
                {
                    Main.Logger.Log("[XXLModExtention] Could not find ReplayPlaybackSpeed");
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Log($"[XXLModExtention] exception: {ex.Message}");
            }
        }

        public static void SetXXLSpeed(float targetSpeed)
        {
            if (!isInitialized || otherSettingsInstance == null || replaySpeedField == null) return;

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

            replaySpeedField.SetValue(otherSettingsInstance, originalSpeed);
            hasSavedOriginal = false;
            //Main.Logger.Log("[XXLModExtention] Restored original ReplayPlaybackSpeed setting in XXLMod3.");
        }
    }
}