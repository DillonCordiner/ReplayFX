using ReplayEditor;
using UnityEngine;
using ReplayTestMod.Utils;
using System.Reflection;
using HarmonyLib;
using GameManagement;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ReplayTestMod
{
    [DefaultExecutionOrder(99999)]
    public class TimelineManager : MonoBehaviour
    {
        private float lastPlaybackTime = -1f;
        private bool PlaybackOverwritten = false;
        private Color PlaybackHandleColor = Color.cyan;
        private Color ImpulseHandleColor = Color.magenta;

        //public CustomCameraCurve customCurve = new CustomCameraCurve();

        private void Start()
        {
            
            ModCheckUtil.CheckForXXLMod();
            if (ModCheckUtil.IsXXLModInstalled)
            {
                XXLModExtention.GetXXLModSettings();
            }
            
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

            UpdateHandleColor(replayEditor);

            if (!replayEditor.cameraController.CamFollowKeyFrames)
            {
                lastPlaybackTime = replayEditor.PlaybackTime;
                return;
            }

            float currentTime = replayEditor.PlaybackTime;

            Updatekeys(replayEditor, currentTime);
            UpdatePlayBackKeys(currentTime);
        }


        private void Updatekeys(ReplayEditorController replayEditor, float currentTime)
        {
            if (Mathf.Abs(currentTime - lastPlaybackTime) > 0.5f)
            {
                lastPlaybackTime = currentTime;
                return;
            }

            foreach (KeyFrame keyframe in replayEditor.cameraController.keyFrames)
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
        
        public bool HasPlayBackKeys()
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
                    if (ModCheckUtil.IsXXLModInstalled)
                    {
                        XXLModExtention.RestoreOriginalSpeed();
                    }
                    else
                    {
                        Traverse.Create(ReplayEditorController.Instance).Field("playbackSpeed").SetValue(1.0f);
                    }
                    CurveUtil.ClearCurveKeys();
                    //ReplayEditorController.Instance.cameraController.cameraCurve.Refresh(ReplayEditorController.Instance.cameraController.keyFrames); // Refresh original curves
                    //CurveUtil.playbackSpeedCurve.Clear();
                    PlaybackOverwritten = false;
                }
            }
        }

        public void SetPlayBackSpeed(float currentTime)
        {
            if (CurveUtil.playbackSpeedCurve != null)
            {
                //float interpolatedSpeed = CurveUtil.playbackSpeedCurve.Evaluate(currentTime);
                float interpolatedSpeed = CurveUtil.EvaluatePlaybackSpeed(currentTime);

                if (ModCheckUtil.IsXXLModInstalled)
                {
                    XXLModExtention.SetXXLSpeed(interpolatedSpeed);
                }
                else
                {
                    Traverse.Create(ReplayEditorController.Instance).Field("playbackSpeed").SetValue(interpolatedSpeed);
                }
            }
        }

        private void UpdateHandleColor(ReplayEditorController replayEditor)
        {
            var keyframes = replayEditor.cameraController.keyFrames;
            var sliders = replayEditor.cameraController.keyframeUI.keyframeSliders;

            if (keyframes.Count <= 0 || sliders.Count <= 0)
            {
                return;
            }

            int count = Mathf.Min(keyframes.Count, sliders.Count);

            for (int i = 0; i < count; i++)
            {
                if (keyframes[i] is PlaybackSpeedKeyFrame)
                {
                    SetHandleColor(i, sliders, PlaybackHandleColor);
                }
                else if (keyframes[i] is ImpulseKeyFrame)
                {
                    SetHandleColor(i, sliders, ImpulseHandleColor);
                }

            }
        }
        private void SetHandleColor(int i, List<Slider> sliders, Color handleColor)
        {
            Slider slider = sliders[i];
            if (slider != null)
            {
                Image handleImage = null;
                if (slider.handleRect != null)
                {
                    handleImage = slider.handleRect.GetComponent<Image>();
                }
                if (handleImage != null)
                {
                    handleImage.color = handleColor;
                }
            }
        }
    }
}