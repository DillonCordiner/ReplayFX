using GameManagement;
using ModIO.UI;
using SkaterXL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ReplayFX.Utils
{ 
    public static class GearUtil
    {
        private static bool isWaiting;
        private static TaskCompletionSource<bool> pendingTask;
        private static bool isReloadingGear;
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
            if (isReloadingGear) return; // prevent overlapping reloads from racing on customizer state
            isReloadingGear = true;

            try
            {
                PlayerController playerController = PlayerController.Instance;

                await Task.Yield();
                string lastPlayer = GetLastPlayer();

                GearDatabase.Instance.FetchCustomGear(); // needs to run for validation to work for some reason

                CustomizedPlayerDataV2 data = await SaveManager.Instance.LoadCharacterCustomizations(lastPlayer);
                if (data == null)
                {
                    SkaterInfo skater = GearDatabase.Instance.skaters.Count > 0 ? GearDatabase.Instance.skaters[0] : null;
                    if (skater == null)
                        throw new InvalidOperationException("No skaters available in GearDatabase to use as backup data");

                    data = await SaveManager.Instance.LoadCharacterCustomizations(skater.CustomizationFileName);
                    if (data == null)
                        throw new InvalidOperationException("Backup skater data also failed to load");

                    MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "[ReloadGear] Failed to Load LastPlayer; using backup Data", 2f);
                }

                await Task.Yield();
                CustomizedPlayerDataV2 validatedData = await GearDatabase.Instance.ValidateCustomization(data, CustomizedPlayerDataV2.Default, GearValidationContext.LocalPlayer);
                string validatedDataName = validatedData.ToString();
                if (data.ToString() != validatedDataName)
                {
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "[ReloadGear] Failed to Validate Data", 3f);
                }

                await playerController.characterCustomizer.RemoveAllGear();
                playerController.characterCustomizer.RemovePreviews();
                await Task.Yield();

                List<GearInfo> gearInfos = validatedData.GetAllGearInfos().ToList();

                foreach (GearInfo gearInfo in gearInfos){playerController.characterCustomizer.LoadGearAsync(gearInfo); }
                while (playerController.characterCustomizer.IsLoading){await Task.Yield(); }
                foreach (GearInfo gearInfo in gearInfos) { playerController.characterCustomizer.EquipGear(gearInfo); }

                playerController.characterCustomizer.LoadCustomizations(validatedData);

                string currentGearName = playerController.characterCustomizer.CurrentCustomizations.ToString();
                string defaultGearName = CustomizedPlayerDataV2.Default.ToString();
                if (currentGearName == defaultGearName)
                {
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Error, "[ReloadGear] Failed to Load Custom Gear - Using Default", 3f);
                }
                else if (currentGearName == validatedDataName && validatedDataName != defaultGearName)
                {
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Success, "[ReloadGear] Custom Gear Reloaded", 3f);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"[ReloadGear] Could not load Player Gear Error: {ex.Message}";
                Main.Logger.Log(errorMessage);
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, errorMessage, 3f);
            }
            finally
            {
                isReloadingGear = false;
            }
        }
        private static async void ReloadGear2()
        {
            PlayerController playerController = PlayerController.Instance;

            await Task.Yield();
            string lastPlayer = GetLastPlayer();
           
            try
            {
                GearDatabase.Instance.FetchCustomGear(); // needs to run for validation to work for some reason

                CustomizedPlayerDataV2 data;
                data = await SaveManager.Instance.LoadCharacterCustomizations(lastPlayer);
                if (data == null)
                {
                    SkaterInfo skater = GearDatabase.Instance.skaters.FirstOrDefault();
                    data = await SaveManager.Instance.LoadCharacterCustomizations(skater.CustomizationFileName);
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "[ReloadGear] Failed to Load LastPlayer; using backup Data", 2f);
                }

                await Task.Yield();
                CustomizedPlayerDataV2 validatedData = await GearDatabase.Instance.ValidateCustomization(data, CustomizedPlayerDataV2.Default, GearValidationContext.LocalPlayer);
                if (data.ToString() != validatedData.ToString())
                {
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "[ReloadGear] Failed to Validate Data", 3f);
                }

                await playerController.characterCustomizer.RemoveAllGear();
                playerController.characterCustomizer.RemovePreviews();
                await Task.Yield();

                foreach (GearInfo gearinfo in validatedData.GetAllGearInfos()) { playerController.characterCustomizer.LoadGearAsync(gearinfo); }
                foreach (GearInfo gearinfo in validatedData.GetAllGearInfos()) { playerController.characterCustomizer.EquipGear(gearinfo); }

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
    }
}