using GameManagement;
using ModIO.UI;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ReplayFX.Utils
{
   
    public static class XLGearModExtenstion
    {
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
    }
}