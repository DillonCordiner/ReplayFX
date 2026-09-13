using GameManagement;
using ModIO.UI;
using SkaterXL.Data;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using ReplayFX.Patches;

namespace ReplayFX.Utils
{
   
    public static class XLGearModExtenstion
    {
        //private static readonly string assetPath;
        private static bool isWaiting;
        private static TaskCompletionSource<bool> pendingTask;

        public static async void ReloadCustomGear()
        {
            if (isWaiting || pendingTask != null)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, "Gear reload already in progress", 2f);
                return;
            }

            Type helperType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("XLGearModifier.AssetBundleHelper")).FirstOrDefault(t => t != null);

            if (helperType == null)
            {
                Main.Logger.Log("[XLGearModExtenstion] Could not find XLGearModifier.AssetBundleHelper");
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, "[XLGearModExtenstion] XLGearMod Not Found", 3f);
                return;
            }

            PropertyInfo assetBundleHelperInstance = helperType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            MethodInfo loadBundlesMethod = helperType.GetMethod("LoadBundles", BindingFlags.Public | BindingFlags.Instance);

            await LoadXLGearModifierBundles(assetBundleHelperInstance, loadBundlesMethod);
        }

        private static async Task LoadXLGearModifierBundles(PropertyInfo assetBundleHelperInstance, MethodInfo loadBundlesMethod)
        {
            object helper = assetBundleHelperInstance.GetValue(null, null);
            if (helper == null) return;

            if (GameStateMachine.Instance.CurrentState is ReplayState)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Success, "Gear will reload when you exit Replay Editor.", 3f);
                await WaitUntilExit<ReplayState>();
            }

            /*
            SkaterInfo skater = GearDatabase.Instance.skaters.FirstOrDefault();
            CustomizedPlayerDataV2 customization = await SaveManager.Instance.LoadCharacterCustomizations(skater.CustomizationFileName);
            if (PlayerController.Instance.characterCustomizer.CurrentCustomizations != customization)
            {
                PlayerController.Instance.characterCustomizer.LoadCustomizations(CustomizedPlayerDataV2.Default);
                await PlayerController.Instance.characterCustomizer.LoadLastPlayer();
            }
            */

            PlayerController.Instance.characterCustomizer.LoadCustomizations(CustomizedPlayerDataV2.Default);
            await PlayerController.Instance.characterCustomizer.LoadLastPlayer();
            //UnloadXLGMAssetPacks();
            Task task = (Task)loadBundlesMethod.Invoke(helper, null);
            await task;
        }

        private static Task WaitUntilExit<TState>() where TState : GameState
        {
            if (GameStateMachine.Instance.CurrentState?.GetType() != typeof(TState))
            {
                isWaiting = false;
                return Task.CompletedTask;
            }

            var result = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            pendingTask = result;
            isWaiting = true;

            void OnStateChanged(Type prevState, Type newState)
            {
                if (newState == typeof(TState)) return;

                GameStateMachine.Instance.OnGameStateChanged -= OnStateChanged;
                isWaiting = false;
                pendingTask = null;
                result.TrySetResult(true);
            }

            GameStateMachine.Instance.OnGameStateChanged += OnStateChanged;
            return result.Task;
        }
        private static void UnloadXLGMAssetPacks()
        {
            IEnumerable<AssetBundle> allLoadedBundles = AssetBundle.GetAllLoadedAssetBundles();
            if (allLoadedBundles == null || allLoadedBundles.Count() <= 0) return;

            string assetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SkaterXL", "XLGearModifier", "Asset Packs");
            if (!Directory.Exists(assetPath)) return;

            var xlgmFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            //var xlgmFileNames = new HashSet<string>();
            foreach (string file in Directory.EnumerateFiles(assetPath, "*", SearchOption.AllDirectories))
            {
                if (!Path.HasExtension(file))
                {
                    xlgmFileNames.Add(Path.GetFileName(file));
                }
            }

            foreach (AssetBundle bundle in allLoadedBundles)
            {
                //string filename;
                //xlgmFileNames.TryGetValue(bundle.name, out string filename);
                if (xlgmFileNames.TryGetValue(bundle.name.ToLower(), out string filename))
                {
                    Main.Logger.Log($"[UnloadXLGMAssetPacks] Unloading XLGM bundle: {filename}");
                    bundle.Unload(false);
                }
            }
        }
    }
}