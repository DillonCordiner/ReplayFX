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
using ReplayEditor;

namespace ReplayFX.Utils
{ 
    public static class GearUtil
    {
        private static bool isWaiting;
        private static TaskCompletionSource<bool> pendingTask;
        public static async void ReloadOnStateExit()
        {
            if (isWaiting || pendingTask != null)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, "Gear reload already in progress", 2f);
                return;
            }

            if (!(GameStateMachine.Instance.CurrentState is PlayState))
            {
                Type currentStateType = GameStateMachine.Instance.CurrentState.GetType();
                MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"Gear will reload when you exit {currentStateType.Name}.", 3f);
                await WaitUntilExit(currentStateType);
            }
            ReloadGear();
        }
        private static async void ReloadGear()
        {
            PlayerController playerController = PlayerController.Instance;

            await Task.Yield();
            string lastPlayer = GetLastPlayer();
            try
            {
                CustomizedPlayerDataV2 data;
                data = await SaveManager.Instance.LoadCharacterCustomizations(lastPlayer);
                if (data == null)
                {
                    SkaterInfo skater = GearDatabase.Instance.skaters.FirstOrDefault();
                    data = await SaveManager.Instance.LoadCharacterCustomizations(skater.CustomizationFileName);
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "[ReloadGear] Failed to Load LastPlayer; using backup Data", 2f);
                }

                await Task.Yield();
                GearDatabase.Instance.FetchCustomGear();

                CustomizedPlayerDataV2 validatedData = await GearDatabase.Instance.ValidateCustomization(data, CustomizedPlayerDataV2.Default, GearValidationContext.LocalPlayer);
                if (data.ToString() != validatedData.ToString())
                {
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "[ReloadGear] Failed to Validate Data", 3f);
                }

                await playerController.characterCustomizer.RemoveAllGear();
                playerController.characterCustomizer.RemovePreviews();

                foreach (GearInfo gearinfo in validatedData.GetAllGearInfos()) { playerController.characterCustomizer.LoadGearAsync(gearinfo); }
                foreach (GearInfo gearinfo in validatedData.GetAllGearInfos()) { playerController.characterCustomizer.EquipGear(gearinfo); }

                await Task.Yield();
                playerController.characterCustomizer.LoadCustomizations(validatedData);

                string currentGearName = playerController.characterCustomizer.CurrentCustomizations.ToString();
                string validatedDataName = validatedData.ToString();
                string DefaultGearName = CustomizedPlayerDataV2.Default.ToString();

                if (currentGearName == DefaultGearName)
                {
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Error,"[ReloadGear] Failed to Load Custom Gear - Using Default", 3f);
                }
                else if (currentGearName == validatedDataName && validatedDataName != DefaultGearName)
                {
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Success,"[ReloadGear] Custom Gear Reloaded", 3f);
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Log($"[ReloadGear] Could not load Player Gear Error: {ex.Message}");
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"[ReloadGear] Could not load Player Gear Error: {ex.Message}", 3f);
            }
        }
        private static Task WaitUntilExit(Type exitState)
        {
            if (GameStateMachine.Instance.CurrentState?.GetType() != exitState)
            {
                isWaiting = false;
                return Task.CompletedTask;
            }

            var result = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            pendingTask = result;
            isWaiting = true;

            void OnStateChanged(Type prevState, Type newState)
            {
                if (newState == exitState) return;

                GameStateMachine.Instance.OnGameStateChanged -= OnStateChanged;
                isWaiting = false;
                pendingTask = null;
                result.TrySetResult(true);
            }

            GameStateMachine.Instance.OnGameStateChanged += OnStateChanged;
            return result.Task;
        }
        public static string GetLastPlayer()
        {
            if (!ConsolePlayerPrefs.HasKey("LastPlayer"))
            {
                return null;
            }
            else
            {
                string lastPlayer = ConsolePlayerPrefs.GetString("LastPlayer");
                return lastPlayer;
            }
        }
        public static async Task CustomLoadLastPlayer(CharacterCustomizer customizer)
        {
            if (!ConsolePlayerPrefs.HasKey("LastPlayer"))
            {
                customizer.LoadCustomizations(CustomizedPlayerDataV2.Default);
            }
            else
            {
                await Task.Yield();
                string lastPlayer = ConsolePlayerPrefs.GetString("LastPlayer");
                try
                {
                    CustomizedPlayerDataV2 customizedPlayerDataV = await SaveManager.Instance.LoadCharacterCustomizations(lastPlayer);
                    if (customizedPlayerDataV == null)
                    {
                        throw new Exception("Failed to load Customization");
                    }
                    customizer.LoadCustomizations(customizedPlayerDataV);
                }
                catch (Exception ex)
                {
                    Logging.gear.LogWarning(string.Concat(new object[] { "Could not load Player ", lastPlayer, " Error: ", ex }));
                    customizer.LoadCustomizations(CustomizedPlayerDataV2.Default);
                }
            }
        }

    }
}