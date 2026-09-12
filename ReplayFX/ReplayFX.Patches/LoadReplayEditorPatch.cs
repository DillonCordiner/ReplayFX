using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using GameManagement;
using ReplayEditor;
using SkaterXL.Data;
using System;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
using Photon.Pun;
using SmoothKeyframeCurves;

namespace ReplayFX.Patches
{
    [HarmonyPatch(typeof(ReplayEditorController), "LoadReplayEditor")]
    public static class LoadReplayEditorPatch
    {
        private static readonly AccessTools.FieldRef<ReplayEditorController, List<ReplayEditorController.OnlinePlayerReplayInfo>> 
            OnlinePlayersRef = AccessTools.FieldRefAccess<ReplayEditorController, List<ReplayEditorController.OnlinePlayerReplayInfo>>("onlinePlayers");

        private static readonly AccessTools.FieldRef<ReplayEditorController, bool> 
            StartAtLastRespawnRef = AccessTools.FieldRefAccess<ReplayEditorController, bool>("startAtLastRespawn");

        private static readonly AccessTools.FieldRef<ReplayEditorController, bool> 
            StartPlayingRef = AccessTools.FieldRefAccess<ReplayEditorController, bool>("startPlaying");

        private static readonly AccessTools.FieldRef<ReplayEditorController, bool> 
            IsPlayingRef = AccessTools.FieldRefAccess<ReplayEditorController, bool>("isPlaying");

        private static readonly Func<ReplayEditorController, Task> 
            SetupOnlinePlayersDelegate = AccessTools.MethodDelegate<Func<ReplayEditorController, Task>>( AccessTools.Method(typeof(ReplayEditorController), "SetupOnlinePlayers"));

        private static readonly Action<ReplayEditorController, bool> 
            SetOnlinePlayerReplayVisibleDelegate = AccessTools.MethodDelegate<Action<ReplayEditorController, bool>>( AccessTools.Method(typeof(ReplayEditorController), "SetOnlinePlayerReplayVisible", new[] { typeof(bool) }));

        private static readonly Action<ReplayEditorController> 
            ResetClipValuesDelegate = AccessTools.MethodDelegate<Action<ReplayEditorController>>(AccessTools.Method(typeof(ReplayEditorController), "ResetClipValues"));

        private static readonly Action<ReplayEditorController, float, bool> 
            SetPlaybackTimeDelegate = AccessTools.MethodDelegate<Action<ReplayEditorController, float, bool>>( AccessTools.Method(typeof(ReplayEditorController), "SetPlaybackTime", new[] { typeof(float), typeof(bool) }));

        [HarmonyPrefix]
        public static bool Prefix(ReplayEditorController __instance)
        {
            OptimizedLoadReplayEditor(__instance);
            //Main.Logger.Log("[LoadReplayEditor] Patch Run");
            return false; // Skips the original unoptimized private method
        }

        private static async void OptimizedLoadReplayEditor( ReplayEditorController __instance)
        {
            GameStateMachine.Instance.StartLoading(false, null, "Loading");
            Main.Logger.Log("[LoadReplayEditor] Start Loading ...");

            try
            {
                ReplayPlaybackController playbackController = __instance.playbackController;
                ReplayCameraController cameraController = __instance.cameraController;

                Task localReplayTask = playbackController.LoadReplay(
                    ReplayRecorder.Instance.LocalPlayerFrames,
                    ReplayRecorder.Instance.gamePlayEvents,
                    PlayerController.Instance.characterCustomizer.CurrentCustomizations,
                    false
                );

                bool isOnline = PhotonNetwork.IsConnected && PhotonNetwork.InRoom;

                List<ReplayEditorController.OnlinePlayerReplayInfo> onlinePlayers = null;

                List<Task> onlinePlayerLoadTasks = null;
                if (isOnline)
                {
                    await SetupOnlinePlayersDelegate(__instance);

                    bool showOnline = ReplaySettings.Instance.showOnlinePlayers && !ReplaySettings.Instance.onlinePlayerLiveView;
                    onlinePlayers = OnlinePlayersRef(__instance);

                    if (showOnline && onlinePlayers != null && onlinePlayers.Count > 0)
                    {
                        onlinePlayerLoadTasks = new List<Task>(onlinePlayers.Count);
                        for (int i = 0; i < onlinePlayers.Count; i++)
                        {
                            onlinePlayerLoadTasks.Add(onlinePlayers[i].Load());
                        }
                    }

                    SetOnlinePlayerReplayVisibleDelegate(__instance, showOnline);
                }
                Main.Logger.Log("[LoadReplayEditor] awaiting load tasks...");
                if (onlinePlayerLoadTasks != null)
                {
                    onlinePlayerLoadTasks.Add(localReplayTask);
                    await Task.WhenAll(onlinePlayerLoadTasks);
                }
                else
                {
                    await localReplayTask;
                }

                cameraController.DeleteKeyFramesOutside(playbackController.ClipStartTime,playbackController.ClipEndTime);
                Main.Logger.Log("[LoadReplayEditor] DeleteKeyFramesOutside complete...");
                ResetClipValuesDelegate(__instance);
                cameraController.CamFollowKeyFrames = false;

                if (StartAtLastRespawnRef(__instance))
                {
                    float targetTime = 0f;

                    List<GPEvent> events =
                        playbackController.gameplayEvents;

                    if (events != null)
                    {
                        for (int i = events.Count - 1; i >= 0; i--)
                        {
                            if (events[i] is RespawnEvent)
                            {
                                targetTime = events[i].time;
                                break;
                            }
                        }
                    }

                    SetPlaybackTimeDelegate(__instance,targetTime, true);
                }

                CameraAnimationCurve cameraCurve = playbackController.gameplayCameraCurve;
                if (cameraCurve != null)
                {
                    try
                    {
                        CameraCurveResult transformData = cameraCurve.Evaluate( playbackController.CurrentTime );

                        cameraController.ApplyGameplayCameraTransform( transformData );
                        Main.Logger.Log("[LoadReplayEditor] ApplyGameplayCameraTransform complete...");
                    }
                    catch (Exception ex)
                    {
                        Main.Logger.Log( "[LoadReplayEditor] Camera evaluation failed: " + ex);
                    }
                }

                if (isOnline)
                {
                    while (AreCustomizersLoading( playbackController, onlinePlayers))
                    {
                        await Task.Yield();
                    }
                }

                if (StartPlayingRef(__instance))
                {
                    IsPlayingRef(__instance) = true;
                    Main.Logger.Log("[LoadReplayEditor] isPlaying" + IsPlayingRef(__instance));
                }
                cameraController.OnReplayEditorStart();
            }
            catch (Exception ex)
            {
                Main.Logger.Log("[LoadReplayEditor] ERROR: " + ex);
            }
            finally
            {
                GameStateMachine.Instance.StopLoading();
                Main.Logger.Log("[LoadReplayEditor] Loading complete ...");
            }
        }

        private static bool AreCustomizersLoading(ReplayPlaybackController playbackController,List<ReplayEditorController.OnlinePlayerReplayInfo> onlinePlayers)
        {
            if (playbackController.characterCustomizer != null && playbackController.characterCustomizer.IsLoading)
            {
                return true;
            }

            if (onlinePlayers == null)
                return false;

            for (int i = 0; i < onlinePlayers.Count; i++)
            {
                ReplayEditorController.OnlinePlayerReplayInfo player = onlinePlayers[i];

                if (player?.playbackController?.characterCustomizer?.IsLoading == true)
                {
                    return true;
                }
            }
            return false;
        }

        private static async void OptimizedLoadReplayEditor3(ReplayEditorController __instance)
        {
            GameStateMachine.Instance.StartLoading(false, null, "Loading");

            Main.Logger.Log("[LoadReplayEditor] Start Loading...");

            ReplayPlaybackController playbackController = ReplayEditorController.Instance.playbackController;
            ReplayCameraController cameraController = ReplayEditorController.Instance.cameraController;

            List<Task> loadTasks = new List<Task>(8)
            {
                playbackController.LoadReplay(
                    ReplayRecorder.Instance.LocalPlayerFrames,
                    ReplayRecorder.Instance.gamePlayEvents,
                    PlayerController.Instance.characterCustomizer.CurrentCustomizations,
                    false
                )
            };

            bool isOnline = PhotonNetwork.IsConnected && PhotonNetwork.InRoom;
            List<ReplayEditorController.OnlinePlayerReplayInfo> onlinePlayers = null;

            if (isOnline)
            {
                await SetupOnlinePlayersDelegate(__instance);

                bool showOnline = ReplaySettings.Instance.showOnlinePlayers && !ReplaySettings.Instance.onlinePlayerLiveView;
                onlinePlayers = OnlinePlayersRef(__instance);

                if (showOnline && onlinePlayers != null && onlinePlayers.Count > 0)
                {
                    for (int i = 0; i < onlinePlayers.Count; i++)
                    {
                        loadTasks.Add(onlinePlayers[i].Load());
                    }
                }
                SetOnlinePlayerReplayVisibleDelegate(__instance, showOnline);
            }

            Main.Logger.Log("[LoadReplayEditor] loadTasks Task.WhenAll Started...");
            await Task.WhenAll(loadTasks);

            cameraController.DeleteKeyFramesOutside(playbackController.ClipStartTime, playbackController.ClipEndTime);
            Main.Logger.Log("[LoadReplayEditor] DeleteKeyFramesOutside complete...");
            ResetClipValuesDelegate(__instance);
            cameraController.CamFollowKeyFrames = false;
            Main.Logger.Log("[LoadReplayEditor] ResetClipValues complete...");

            if (StartAtLastRespawnRef(__instance))
            {
                float targetTime = 0f;
                var events = playbackController.gameplayEvents;
                if (events != null)
                {
                    for (int i = events.Count - 1; i >= 0; i--)
                    {
                        if (events[i] is RespawnEvent)
                        {
                            targetTime = events[i].time;
                            break;
                        }
                    }
                }
                SetPlaybackTimeDelegate(__instance, targetTime, true);
                Main.Logger.Log("[LoadReplayEditor] startAtLastRespawn complete...");
            }

            // initial camera transform evaluation
            var cameraCurve = playbackController.gameplayCameraCurve;
            if (cameraCurve != null)
            {
                try
                {
                    CameraCurveResult transformData = cameraCurve.Evaluate(playbackController.CurrentTime);
                    cameraController.ApplyGameplayCameraTransform(transformData);
                    Main.Logger.Log("[LoadReplayEditor] ApplyGameplayCameraTransform complete...");
                }
                catch (Exception ex)
                {
                    Main.Logger.Log($"[LoadReplayEditor] Skipped camera evaluation: {ex.Message}");
                }
            }

            // Wait for Customizer Asset Streaming
            if (isOnline)
            {
                bool IsAnyCustomizerLoading()
                {
                    if (playbackController.characterCustomizer != null && playbackController.characterCustomizer.IsLoading)
                        return true;

                    if (onlinePlayers != null)
                    {
                        for (int i = 0; i < onlinePlayers.Count; i++)
                        {
                            if (onlinePlayers[i]?.playbackController?.characterCustomizer?.IsLoading == true)
                                return true;
                        }
                    }
                    return false;
                }

                while (IsAnyCustomizerLoading())
                {
                    Main.Logger.Log("[LoadReplayEditor] LoadOnlineCustomizer Task.Yeild...");
                    await Task.Yield();
                }
            }

            if (StartPlayingRef(__instance))
            {
                IsPlayingRef(__instance) = true;
                Main.Logger.Log("[LoadReplayEditor] isPlaying" + IsPlayingRef(__instance));
            }

            cameraController.OnReplayEditorStart();
            GameStateMachine.Instance.StopLoading();
            Main.Logger.Log("[LoadReplayEditor] Loading Complete");
        }
        private static async void OptimizedLoadReplayEditor2(ReplayEditorController __instance)
        {
            Traverse trv = Traverse.Create(__instance);

            GameStateMachine.Instance.StartLoading(false, null, "Loading");
            Main.Logger.Log("[LoadReplayEditor] after start Loading...");
            await Task.Yield();

            ReplayPlaybackController playbackController = ReplayEditorController.Instance.playbackController;
            ReplayCameraController cameraController = ReplayEditorController.Instance.cameraController;
            List<ReplayEditorController.OnlinePlayerReplayInfo> onlinePlayers = trv.Field("onlinePlayers").GetValue<List<ReplayEditorController.OnlinePlayerReplayInfo>>();

            //CustomizedPlayerDataV2 testCustomiztions = CustomizedPlayerDataV2.Default.;
            //playerController.characterCustomizer.LoadCustomizations(CustomizedPlayerDataV2.Default);
            //playerController.characterCustomizer.LoadCustomizations(playerController.characterCustomizer.CurrentCustomizations);

            List<Task> loadTasks = new List<Task>(8)
            {
                playbackController.LoadReplay(
                    ReplayRecorder.Instance.LocalPlayerFrames,
                    ReplayRecorder.Instance.gamePlayEvents,
                    PlayerController.Instance.characterCustomizer.CurrentCustomizations,
                    false
                )
            };

            Main.Logger.Log("[LoadReplayEditor] loadTasks Task.WhenAll Started...");
            await Task.WhenAll(loadTasks);

            cameraController.DeleteKeyFramesOutside(playbackController.ClipStartTime, playbackController.ClipEndTime);
            Main.Logger.Log("[LoadReplayEditor] DeleteKeyFramesOutside complete...");

            trv.Method("ResetClipValues").GetValue();
            cameraController.CamFollowKeyFrames = false;
            Main.Logger.Log("[LoadReplayEditor] ResetClipValues complete...");

            bool startAtLastRespawn = trv.Field("startAtLastRespawn").GetValue<bool>();
            if (startAtLastRespawn)
            {
                GPEvent targetRespawnEvent = null;
                var events = playbackController.gameplayEvents;
                for (int i = events.Count - 1; i >= 0; i--)
                {
                    if (events[i] is RespawnEvent)
                    {
                        targetRespawnEvent = events[i];
                        break;
                    }
                }
                trv.Method("SetPlaybackTime", targetRespawnEvent != null ? targetRespawnEvent.time : 0f, true).GetValue();
                Main.Logger.Log("[LoadReplayEditor] startAtLastRespawn - SetPlaybackTime complete...");
            }

            //cameraController.ApplyGameplayCameraTransform(playbackController.gameplayCameraCurve.Evaluate(playbackController.CurrentTime));
            //Main.Logger.Log("[LoadReplayEditor] ApplyGameplayCameraTransform complete...");

            if (playbackController.gameplayCameraCurve != null)
            {
                try
                {
                    //cameraController.ApplyGameplayCameraTransform(playbackController.gameplayCameraCurve.Evaluate(playbackController.CurrentTime));
                    CameraCurveResult transformData = playbackController.gameplayCameraCurve.Evaluate(playbackController.CurrentTime);
                    cameraController.ApplyGameplayCameraTransform(transformData);
                    Main.Logger.Log("[LoadReplayEditor] ApplyGameplayCameraTransform complete...");
                }
                catch (Exception ex)
                {
                    Main.Logger.Log($"Skipped ApplyGameplayCameraTransform due to curve error: {ex.Message}");
                }
            }
            else
            {
                Main.Logger.Log("Gameplay camera curve is null or empty. Skipping.");
            }

            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                await trv.Method("SetupOnlinePlayers").GetValue<Task>();

                bool showOnline = ReplaySettings.Instance.showOnlinePlayers && !ReplaySettings.Instance.onlinePlayerLiveView;
                if (showOnline && onlinePlayers != null && onlinePlayers.Count > 0)
                {
                    foreach (var player in onlinePlayers)
                    {
                        loadTasks.Add(player.Load());
                    }
                }
                trv.Method("SetOnlinePlayerReplayVisible", showOnline).GetValue();

                bool LoadOnlineCustomizer()
                {
                    if (!playbackController.characterCustomizer.IsLoading) return false;

                    if (onlinePlayers != null && onlinePlayers.Count > 0)
                    {
                        foreach (ReplayEditorController.OnlinePlayerReplayInfo pc in onlinePlayers)
                        {
                            if (pc.playbackController.characterCustomizer.IsLoading) return false;
                        }
                    }
                    return true;
                }
                while (LoadOnlineCustomizer())
                {
                    Main.Logger.Log("[LoadReplayEditor] LoadOnlineCustomizer Task.Yeild...");
                    await Task.Yield();
                }
            }

            bool startPlaying = trv.Field("startPlaying").GetValue<bool>();
            if (startPlaying)
            {
                trv.Field("isPlaying").SetValue(true);
                Main.Logger.Log("[LoadReplayEditor] isPlaying" + startPlaying);
            }

            cameraController.OnReplayEditorStart();
            Main.Logger.Log("[LoadReplayEditor] Before Stop Loading");
            GameStateMachine.Instance.StopLoading();
            Main.Logger.Log("[LoadReplayEditor] Loading Complete");
        }
    }
}