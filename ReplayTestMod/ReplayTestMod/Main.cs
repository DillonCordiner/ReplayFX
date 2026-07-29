using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using RapidGUI;
using System;
using Object = UnityEngine.Object;
using System.Runtime;
using ReplayTestMod.Utils;

namespace ReplayTestMod
{
    internal static class Main
    {
        public static bool enabled;
        public static Harmony harmonyInstance;
        public static string modId = "ReplayTestMod";
        public static UnityModManager.ModEntry modEntry;
        public static Settings settings;
        public static GameObject ScriptManager;
        public static CamNoiseController camNoiseController;
        public static InputListener inputListener;
        public static UIController uiController;
        public static TimelineManager timelineManager;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
                modEntry.OnGUI = OnGUI;
                modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(OnSaveGUI);
                modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(OnToggle);
                modEntry.OnUnload = new Func<UnityModManager.ModEntry, bool>(Unload);
                Main.modEntry = modEntry;
                Logger.Log(nameof(Load));
            }
            catch (Exception ex)
            {
                Logger.Error($"Error Loading {modEntry}: {ex.Message}");
                return false;
            }

            return true;
        }
        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginVertical(GUILayout.Width(284));
            if (RGUI.Button(inputListener.changeHotKey, "Change HotKey"))
            {
                inputListener.changeHotKey = !inputListener.changeHotKey;
            }
            if (inputListener.changeHotKey)
            {
                GUILayout.Label("<b>Press any Key to change Noise HotKey</b>");
                GUILayout.Box("<b>Current Noise HotKey: </b>" + settings.noiseHotkey.keyCode.ToString(""), GUILayout.Height(25f));
                if (inputListener.GetCurrentKeyDown() != null)
                {
                    settings.noiseHotkey = new KeyBinding { keyCode = (KeyCode)inputListener.GetCurrentKeyDown() };
                    Logger.Log("Noise Hot Key Changed to:" + settings.noiseHotkey.keyCode.ToString(""));
                }
            }
            GUILayout.EndVertical();

            /*
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(GUILayout.MaxWidth(256));
                {
                    GUILayout.Label("Camera Profile");
                    camNoiseController.targetProfile = RGUI.SelectionPopup(camNoiseController.targetProfile, camNoiseController.ProfileOptions);
                    GUILayout.Space(6f);
                    settings.amplitude = RGUI.SliderFloat(settings.amplitude, 0.0f, 10.0f, 1.0f, 82, "Amplitude");
                    GUILayout.Space(4f);
                    settings.frequency = RGUI.SliderFloat(settings.frequency, 0.0f, 10.0f, 1.0f, 82, "Frequency");
                    GUILayout.Space(4f);
                    UIextensions.FlexableButton("Generate new seed", camNoiseController.GenerateNewSeed, Color.white);

                    GUILayout.Space(8f);

                    UIextensions.CenteredLabel("Pivot Offset");
                    GUILayout.Space(6f);
                    settings.offset_x = RGUI.SliderFloat(settings.offset_x, 0.0f, 10.0f, 0.0f, 72, "X Pivot");
                    GUILayout.Space(4f);
                    settings.offset_y = RGUI.SliderFloat(settings.offset_y, 0.0f, 10.0f, 0.0f, 72, "Y Pivot");
                    GUILayout.Space(4f);
                    settings.offset_z = RGUI.SliderFloat(settings.offset_z, 0.0f, 10.0f, 0.0f, 72, "Z Pivot");
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.MaxWidth(256));
                {
                    UIextensions.CenteredLabel("Impluse KeyFrames");
                    GUILayout.Space(6f);
                    UIextensions.FlexableButton("Create Impluse KeyFrame", KeyFrameHelper.AddImpluseKeyFrame, Color.white);

                    GUILayout.Space(8f);

                    UIextensions.CenteredLabel("PlayBack KeyFrames");
                    GUILayout.Space(6f);
                    UIextensions.FlexableButton("Create PlayBack KeyFrame", KeyFrameHelper.AddPlayBackKeyFrame, Color.white);
                    GUILayout.Space(4f);
                    settings.playBackSpeed = RGUI.SliderFloat(settings.playBackSpeed, 0.0f, 2.0f, 1.0f, 72, "PlayBack Speed");
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            */
        }
        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (enabled == value)
                return true;

            enabled = value;

            if (enabled)
            {
                try
                {
                    harmonyInstance = new Harmony(modEntry.Info.Id);
                    harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

                    ScriptManager = new GameObject("ReplayTestMod");
                    camNoiseController = ScriptManager.AddComponent<CamNoiseController>();
                    inputListener = ScriptManager.AddComponent<InputListener>();
                    uiController = ScriptManager.AddComponent<UIController>();
                    timelineManager = ScriptManager.AddComponent<TimelineManager>();
                    Object.DontDestroyOnLoad(ScriptManager);

                    AssetLoader.LoadBundles();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error during {modEntry} initialization: {ex.Message}");
                    enabled = false; // Rollback enabling if an error occurs
                    return false;
                }
            }
            else
            {
                Unload(modEntry);
            }

            return true;
        }

        public static bool Unload(UnityModManager.ModEntry modEntry)
        {
            try
            {
                harmonyInstance?.UnpatchAll(harmonyInstance.Id);

                if (ScriptManager != null)
                {
                    AssetLoader.UnloadAssetBundle();
                    Object.Destroy(ScriptManager);
                    ScriptManager = null;
                }

                Logger.Log(nameof(Unload));
            }
            catch (Exception ex)
            {
                Logger.Error($"Error during {modEntry} unload: {ex.Message}");
                return false;
            }

            return true;
        }

        public static UnityModManager.ModEntry.ModLogger Logger => modEntry.Logger;
    }
}
