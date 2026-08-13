using ReplayEditor;
using UnityEngine;
using ReplayTestMod.Utils;
using System.Reflection;
using HarmonyLib;
using GameManagement;

namespace ReplayTestMod
{
    [DefaultExecutionOrder(9999)]
    public class TimelineManager : MonoBehaviour
    {
        private float lastPlaybackTime = -1f;
        private bool PlaybackOverwritten = false;

        private void Start()
        {
            ModCheckUtil.CheckForXXLMod();
        }
        /*
        void Update()
        {
            ReplayEditorController replayEditor = ReplayEditorController.Instance;

            if (replayEditor == null || replayEditor.cameraController == null) 
                return;

            if (!replayEditor.cameraController.CamFollowKeyFrames)
            {
                lastPlaybackTime = replayEditor.PlaybackTime;
                return;
            }

            float currentTime = replayEditor.PlaybackTime;

            Updatekeys(replayEditor, currentTime);
            //UpdatePlayBackSpeedKeys(replayEditor, currentTime);

        }
        */
        void LateUpdate()
        {
            ReplayEditorController replayEditor = ReplayEditorController.Instance;

            if (replayEditor == null || replayEditor.cameraController == null)
                return;

            if (!replayEditor.cameraController.CamFollowKeyFrames)
            {
                lastPlaybackTime = replayEditor.PlaybackTime;
                return;
            }

            float currentTime = replayEditor.PlaybackTime;

            Updatekeys(replayEditor, currentTime);
            UpdatePlayBackKeys(currentTime); // curently not working
            //SetPlayBackSpeed(currentTime);
        }


        private void Updatekeys(ReplayEditorController replayEditor, float currentTime)
        {
            if (Mathf.Abs(currentTime - lastPlaybackTime) > 0.5f)
            {
                lastPlaybackTime = currentTime;
                return;
            }

            foreach (var keyframe in replayEditor.cameraController.keyFrames)
            {
                if (keyframe is ImpulseKeyFrame impulseKey)
                {
                    if (lastPlaybackTime <= impulseKey.time && currentTime > impulseKey.time)
                    {
                        impulseKey.TriggerKeyFrame();
                    }
                }
                /*
                else if (keyframe is PlaybackSpeedKeyFrame playbackspeedKey)
                {
                    if (lastPlaybackTime <= playbackspeedKey.time && currentTime > playbackspeedKey.time)
                    {
                        playbackspeedKey.TriggerKeyFrame();
                    }
                }
                */
            }
            lastPlaybackTime = currentTime;
        }
        
        private bool HasPlayBackKeys()
        {
            bool hasPlaybackKeys = false;
            if (CurveUtil.playbackSpeedCurve.Keys.Count > 0)
            {
                hasPlaybackKeys = true;
                return hasPlaybackKeys;
            }
            return hasPlaybackKeys;
            
        }
        private void UpdatePlayBackKeys(float currentTime)
        {
            if (HasPlayBackKeys())
            {
                SetPlayBackSpeed(currentTime);
                PlaybackOverwritten = true;
            }
            else
            {
                if (PlaybackOverwritten)
                {
                    //Traverse.Create(ReplayEditorController.Instance).Field("playbackSpeed").SetValue(1.0f);
                    //ReplayEditorController.Instance.cameraController.cameraCurve.Refresh(ReplayEditorController.Instance.cameraController.keyFrames); // Refresh original curves
                    //CurveUtil.playbackSpeedCurve.Clear();
                    PlaybackOverwritten = false;
                }
            }
        }

        private void SetPlayBackSpeed(float currentTime)
        {
            if (CurveUtil.playbackSpeedCurve != null)
            {
                float interpolatedSpeed = CurveUtil.playbackSpeedCurve.Evaluate(currentTime);
                Traverse.Create(ReplayEditorController.Instance).Field("playbackSpeed").SetValue(interpolatedSpeed);
            }
        }
    }
}