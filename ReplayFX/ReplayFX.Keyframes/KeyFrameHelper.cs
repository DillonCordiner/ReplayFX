using Cinemachine;
using ModIO.UI;
using ReplayEditor;
using ReplayFX.Utils;
using RootMotion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace ReplayFX.Keyframes
{
    public static class KeyFrameHelper
    {
        public static void AddImpluseKeyFrame()
        {
            CreateImpluseKeyFrame(Main.camController.impulseSource, ReplayEditorController.Instance.playbackController.CurrentTime, 
                Main.settings.impulse_force,
                Main.settings.impulse_source_amplitude, 
                Main.settings.impulse_source_frequency,
                Main.settings.impulse_source_decaytime);          
            //CurveUtil.Refresh();
        }

        public static void AddPlayBackKeyFrame()
        {
            CreatePlaybackKeyFrame(Main.settings.replay_playback_speed, ReplayEditorController.Instance.playbackController.CurrentTime);
           
            //CurveUtil.Refresh();
        }

        public static void RemoveAllImpulseKeys()
        {
            RemoveKeyFramesOfType(typeof(ImpulseKeyFrame));
            MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "All Impulse Keys Deleted", 1.5f);
            //CurveUtil.Refresh();
        }
        public static void RemoveAllPlaybackKeys()
        {
            RemoveKeyFramesOfType(typeof(PlaybackSpeedKeyFrame));
            MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "All Playback Keys Deleted", 1.5f);
            //CurveUtil.Refresh();
        }
        private static void RemoveKeyFramesOfType(Type keyframeType)
        {
            ReplayCameraController cameraController = ReplayEditorController.Instance.cameraController;
            //var keyframes = ReplayEditorController.Instance.cameraController.keyFrames;

            for (int i = cameraController.keyFrames.Count - 1; i >= 0; i--)
            {
                if (cameraController.keyFrames[i].GetType() == keyframeType)
                {
                    //ReplayEditorController.Instance.cameraController.keyFrames.Remove(keyframes[i]);
                    cameraController.keyFrames.RemoveAt(i);
                }
            }
            //CurveUtil.Refresh();
        }
        public static void RemoveAllCustomKeyFrames(ReplayCameraController cameraController)
        {
            if (cameraController?.keyFrames == null) return;

            for (int i = cameraController.keyFrames.Count - 1; i >= 0; i--)
            {
                if (cameraController.keyFrames[i] is PlaybackSpeedKeyFrame || cameraController.keyFrames[i] is ImpulseKeyFrame)
                {
                    cameraController.keyFrames.RemoveAt(i);
                }
            }
        }
        public static List<KeyFrame> GetAllKeyFramesOfType(Type keyframeType)
        {
            List<KeyFrame> keyFrameList = new List<KeyFrame>();

            ReplayCameraController cameraController = ReplayEditorController.Instance.cameraController;
            for (int i = cameraController.keyFrames.Count - 1; i >= 0; i--)
            {
                if (cameraController.keyFrames[i].GetType() == keyframeType)
                {
                    keyFrameList.Add(cameraController.keyFrames[i]);
                }
            }
            return keyFrameList;
        }
        public static void CreatePlaybackKeyFrame(float playbackspeed, float time)
        {
            int index = FindKeyFrameInsertIndex(time);
            KeyFrame keyFrame;

            keyFrame = new PlaybackSpeedKeyFrame(playbackspeed, time);
            keyFrame.AddKeyframes(ReplayEditorController.Instance.cameraController.cameraCurve);
            ReplayEditorController.Instance.cameraController.keyFrames.Insert(index, keyFrame);

            //Main.Logger.Log("PlayBack KeyFrame added at: " + time);
        }
        public static void CreateImpluseKeyFrame(CinemachineImpulseSource impulseSource, float time, float force, float amplitude, float frequency, float decay)
        {
            int index = FindKeyFrameInsertIndex(time);
            ImpulseKeyFrame keyFrame = new ImpulseKeyFrame(impulseSource, time, force, amplitude, frequency, decay);
            keyFrame.ApplyTo(ReplayEditorController.Instance.cameraController.VirtualCamera);
            ReplayEditorController.Instance.cameraController.keyFrames.Insert(index, keyFrame);

        }
        public static void CreateImpluseKeyFrame_old(CinemachineImpulseSource impulseSource, float time, float force, float amplitude, float frequency, float decay)
        {
            int index = FindKeyFrameInsertIndex(time);
            KeyFrame keyFrame;

            keyFrame = new ImpulseKeyFrame(impulseSource, time, force, amplitude, frequency, decay);
            keyFrame.ApplyTo(ReplayEditorController.Instance.cameraController.VirtualCamera);
            ReplayEditorController.Instance.cameraController.keyFrames.Insert(index, keyFrame);

            //Main.Logger.Log("Impulse KeyFrame added at: " + time);
        }
        private static int FindKeyFrameInsertIndex2(float time)
        {
            var keyFrames = ReplayEditorController.Instance.cameraController.keyFrames;
            int index = keyFrames.FindIndex(k => k.time > time);
            return index == -1 ? keyFrames.Count : index;
        }
        private static int FindKeyFrameInsertIndex(float time)
        {
            var keyFrames = ReplayEditorController.Instance.cameraController.keyFrames;
            if (keyFrames.Count == 0)
            {
                return 0;
            }
            if (time < keyFrames[0].time)
            {
                return 0;
            }
            if (keyFrames.Count == 1)
            {
                return 1;
            }
            for (int i = 0; i < keyFrames.Count - 1; i++)
            {
                if (time > keyFrames[i].time && time < keyFrames[i + 1].time)
                {
                    return i + 1;
                }
            }
            return keyFrames.Count;
        }
    }
}
