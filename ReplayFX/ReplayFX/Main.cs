using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using RapidGUI;
using System;
using Object = UnityEngine.Object;
using System.Runtime;
using ReplayFX.Utils;
using ReplayFX.UI;

namespace ReplayFX
{
    internal static class Main
    {
        public static bool enabled;
        public static Harmony harmonyInstance;
        public static string modId = "ReplayFX";
        public static UnityModManager.ModEntry modEntry;
        public static Settings settings;
        public static GameObject ScriptManager;
        public static NoiseController noiseController;
        public static InputListener inputListener;
        public static UIController uiController;
        public static TimelineManager timelineManager;
        public static ReplayFXMenuController replayfxMenu;

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
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("<b>Most settings can be changed in the replay pause menu</b>");
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("[Ctrl + " + settings.noiseHotkey.keyCode.ToString("") + "] for Keyboard/mouse UI");
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                if (RGUI.Button(inputListener.changeHotKey, "Change HotKey"))
                {
                    inputListener.changeHotKey = !inputListener.changeHotKey;
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                if (inputListener.changeHotKey)
                {
                    GUILayout.Label("<b>Press any Key to change Noise HotKey</b>");
                    GUILayout.Box("<b>Current HotKey: </b>" + settings.noiseHotkey.keyCode.ToString(""), GUILayout.Height(25f));
                    if (inputListener.GetCurrentKeyDown() != null)
                    {
                        settings.noiseHotkey = new KeyBinding { keyCode = (KeyCode)inputListener.GetCurrentKeyDown() };
                        Logger.Log("HotKey Changed to:" + settings.noiseHotkey.keyCode.ToString(""));
                    }
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
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

                    ScriptManager = new GameObject("ReplayFX");
                    noiseController = ScriptManager.AddComponent<NoiseController>();
                    inputListener = ScriptManager.AddComponent<InputListener>();
                    uiController = ScriptManager.AddComponent<UIController>();
                    timelineManager = ScriptManager.AddComponent<TimelineManager>();
                    replayfxMenu = ScriptManager.AddComponent<ReplayFXMenuController>();
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
