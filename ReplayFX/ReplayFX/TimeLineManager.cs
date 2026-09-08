using ReplayEditor;
using UnityEngine;
using ReplayFX.Utils;
using System.Reflection;
using HarmonyLib;
using GameManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using ReplayFX.Keyframes;

namespace ReplayFX
{
    [DefaultExecutionOrder(9999)]
    public class TimelineManager : MonoBehaviour
    {
        private int lastKeyframeCount = -1;
        private float lastPlaybackTime = -1f;
        private bool PlaybackOverwritten = false;
        private Color impulse_key_color = Color.cyan;
        private Color playback_key_color = Color.gray;
        //private Color playbackHandleColor = Color.cyan;
        //private Color impulseHandleColor = Color.gray;

        private void Start()
        {
            if (ModCheckUtil.CheckForMod(XXLModExtention.XXLmodID))
            {
                XXLModExtention.IsXXLModInstalled = true;
                XXLModExtention.GetXXLModSettings();
                ResetPlayBackSpeed();
            }
            else
            {
                XXLModExtention.IsXXLModInstalled = false;
            }
        }
        private void LateUpdate()
        {
            ReplayEditorController replayEditor = ReplayEditorController.Instance;

            if (replayEditor == null || replayEditor.cameraController == null)
                return;

            RefreshOnKeyChange(replayEditor);
            UpdateHandleColor(replayEditor);

            if (!replayEditor.cameraController.CamFollowKeyFrames)
            {
                lastPlaybackTime = replayEditor.PlaybackTime;
                return;
            }

            float currentTime = replayEditor.PlaybackTime;

            UpdatePlayBackSpeed(currentTime);
            UpdateImpulsekeys(replayEditor, currentTime);
        }

        private void RefreshOnKeyChange(ReplayEditorController replayEditor)
        {
            int currentCount = replayEditor.cameraController.keyFrames.Count;
            if (currentCount >= 0 && currentCount != lastKeyframeCount)
            {
                CurveUtil.Refresh();
                lastKeyframeCount = currentCount;
            }
        }

        private void UpdateImpulsekeys(ReplayEditorController replayEditor, float currentTime)
        {
            if (replayEditor.cameraController.keyFrames.Count <= 0)
                return;

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
            }
            lastPlaybackTime = currentTime;
        }

        private void UpdatePlayBackSpeed(float currentTime)
        {
            if (CurveUtil.HasPlayBackKeys())
            {
                SetPlayBackSpeed(currentTime);
                PlaybackOverwritten = true;
            }
            else
            {
                ResetPlayBackSpeed();
            }
        }
        private void ResetPlayBackSpeed()
        {
            if (PlaybackOverwritten)
            {
                if (XXLModExtention.IsXXLModInstalled)
                {
                    XXLModExtention.RestoreOriginalSpeed();
                }
                else
                {
                    //Traverse.Create(ReplayEditorController.Instance).Field("playbackSpeed").SetValue(1.0f);
                    PlayBackUtil.SetPlayBackSpeedValue(1.0f);
                }
                //KeyFrameHelper.RemoveAllPlaybackKeys();
                PlaybackOverwritten = false;
            }
        }

        public void SetPlayBackSpeed(float currentTime)
        {
            if (CurveUtil.playbackSpeedCurve != null)
            {
                //float interpolatedSpeed = CurveUtil.playbackSpeedCurve.Evaluate(currentTime);
                float interpolatedSpeed = CurveUtil.EvaluatePlaybackSpeed(currentTime);

                if (XXLModExtention.IsXXLModInstalled)
                {
                    XXLModExtention.SetXXLSpeed(interpolatedSpeed);
                }
                else
                {
                    //Traverse.Create(ReplayEditorController.Instance).Field("playbackSpeed").SetValue(interpolatedSpeed);
                    PlayBackUtil.SetPlayBackSpeedValue(interpolatedSpeed);
                }
            }
        }

        private void UpdateHandleColor(ReplayEditorController replayEditor)
        {
            List<KeyFrame> keyframes = replayEditor.cameraController.keyFrames;
            List<Slider> sliders = replayEditor.cameraController.keyframeUI.keyframeSliders;

            if (keyframes == null || sliders == null || keyframes.Count <= 0 || sliders.Count <= 0)
            {
                return;
            }

            int count = Mathf.Min(keyframes.Count, sliders.Count);

            for (int i = 0; i < count; i++)
            {
                if (keyframes[i] is PlaybackSpeedKeyFrame)
                {
                    //playbackHandleColor = FloatToColourUtil.GetColorFromValue(Main.settings.playbackHandleValue);

                    if (Main.settings.isPlaybackGreyscale)
                    {
                        playback_key_color = ColorUtil.FloatToGrayscale(Main.settings.playback_color_value);
                    }
                    else
                    {
                        playback_key_color = ColorUtil.FloatToRGB(Main.settings.playback_color_value);
                    }

                    SetHandleColor(i, sliders, playback_key_color);
                }
                else if (keyframes[i] is ImpulseKeyFrame)
                {
                    //impulseHandleColor = FloatToColourUtil.GetColorFromValue(Main.settings.impulseHandleValue);

                    if (Main.settings.isImpulseGreyscale)
                    {
                        impulse_key_color = ColorUtil.FloatToGrayscale(Main.settings.impulse_color_value);
                    }
                    else
                    {
                        impulse_key_color = ColorUtil.FloatToRGB(Main.settings.impulse_color_value);
                    }

                    SetHandleColor(i, sliders, impulse_key_color);
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