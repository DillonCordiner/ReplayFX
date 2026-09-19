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
            CustomLoadReplayEditor(__instance);
            //Main.Logger.Log("[LoadReplayEditor] Patch Run");
            return false; // Skip
        }
        

        private static async void CustomLoadReplayEditor( ReplayEditorController __instance)
        {
            GameStateMachine.Instance.StartLoading(false, null, "Loading");
            //Main.Logger.Log("[LoadReplayEditor] Start Loading ...");

            try
            {
                ReplayPlaybackController playbackController = __instance.playbackController;
                ReplayCameraController cameraController = __instance.cameraController;

                Task localReplayTask = playbackController.LoadReplay(ReplayRecorder.Instance.LocalPlayerFrames, ReplayRecorder.Instance.gamePlayEvents, PlayerController.Instance.characterCustomizer.CurrentCustomizations, false);

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
                //Main.Logger.Log("[LoadReplayEditor] awaiting load tasks...");
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
                //Main.Logger.Log("[LoadReplayEditor] DeleteKeyFramesOutside complete...");
                ResetClipValuesDelegate(__instance);
                cameraController.CamFollowKeyFrames = false;

                if (StartAtLastRespawnRef(__instance))
                {
                    float targetTime = 0f;

                    List<GPEvent> events = playbackController.gameplayEvents;

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
                        cameraController.VirtualCamera.UpdateCameraState(Vector3.up, playbackController.CurrentTime);
                        cameraController.ApplyGameplayCameraTransform( transformData );
                        //Main.Logger.Log("[LoadReplayEditor] ApplyGameplayCameraTransform complete...");
                    }
                    catch (Exception ex)
                    {
                        Main.Logger.Log( $"[LoadReplayEditor] Camera evaluation failed: {ex.Message}");
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
                    //Main.Logger.Log("[LoadReplayEditor] isPlaying" + IsPlayingRef(__instance));
                }
                cameraController.OnReplayEditorStart();
            }
            catch (Exception ex)
            {
                GameStateMachine.Instance.RequestPreviousState();
                Main.Logger.Log($"[LoadReplayEditor] Failed to load Replay Editor: { ex.Message}");           
            }
            finally
            {
                GameStateMachine.Instance.StopLoading();
                //Main.Logger.Log("[LoadReplayEditor] Loading complete ...");
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
    }
    
}