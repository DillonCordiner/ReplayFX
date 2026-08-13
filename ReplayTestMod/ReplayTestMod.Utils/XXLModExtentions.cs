using System;
using System.Reflection;

namespace ReplayTestMod.Utils
{
    public static class XXLModExtentions
    {
        private static FieldInfo settingsField;
        private static FieldInfo speedField;
        private static object settingsInstance;

        private static bool isInitialized = false;
        private static float originalSpeed = 1.0f;
        private static bool hasSavedOriginal = false;

        public static void GetXXLModSettings()
        {
            try
            {
                // 1. Get the Main class type from XXLMod3
                Type xxlMainType = Type.GetType("XXLMod3.Main, XXLMod3");
                if (xxlMainType == null) return;

                // 2. Get the static 'settings' field on XXLMod3.Main
                settingsField = xxlMainType.GetField("settings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (settingsField == null) return;

                // 3. Get the actual settings instance object
                settingsInstance = settingsField.GetValue(null);
                if (settingsInstance == null) return;

                // 4. Get the replayPlaybackSpeed field from the Settings class
                // (Verify exact field name in decompiled XXLMod3, e.g. "replayPlaybackSpeed")
                speedField = settingsInstance.GetType().GetField("replayPlaybackSpeed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (speedField != null)
                {
                    isInitialized = true;
                    Main.Logger.Log("[XXLModHijacker] Successfully connected to XXLMod3 settings.");
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Log($"[XXLModHijacker] Failed to initialize: {ex.Message}");
            }
        }

        public static void SetXXLSpeed(float targetSpeed)
        {
            if (!isInitialized || settingsInstance == null || speedField == null) return;

            // Save the user's original XXL slider speed before we overwrite it
            if (!hasSavedOriginal)
            {
                originalSpeed = (float)speedField.GetValue(settingsInstance);
                hasSavedOriginal = true;
            }

            // Overwrite XXLMod3's setting value
            speedField.SetValue(settingsInstance, targetSpeed);
        }

        public static void RestoreOriginalSpeed()
        {
            if (!isInitialized || settingsInstance == null || speedField == null || !hasSavedOriginal) return;

            // Restore what the user originally had set on their XXL slider
            speedField.SetValue(settingsInstance, originalSpeed);
            hasSavedOriginal = false;
            Main.Logger.Log("[XXLModHijacker] Restored original XXLMod speed setting.");
        }
    }
}