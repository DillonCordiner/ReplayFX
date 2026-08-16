using Cinemachine;
using GameManagement;
using RapidGUI;
using ReplayEditor;
using ReplayTestMod;
using ReplayTestMod.Utils;
using SmoothKeyframeCurves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReplayTestMod.Utils
{
    public static class KeyFrameHelper
    {
        public static void AddImpluseKeyFrame()
        {
            CreateImpluseKeyFrame(Main.camNoiseController.impulseSource, ReplayEditorController.Instance.playbackController.CurrentTime);
            //CurveUtil.Refresh();
            //ReplayEditorController.Instance.cameraController.keyframeUI.UpdateKeyframes(ReplayEditorController.Instance.cameraController.keyFrames);
        }

        public static void AddPlayBackKeyFrame()
        {
            CreatePlaybackKeyFrame(Main.settings.playBackSpeed, ReplayEditorController.Instance.playbackController.CurrentTime);
            //CurveUtil.Refresh();
            //ReplayEditorController.Instance.cameraController.keyframeUI.UpdateKeyframes(ReplayEditorController.Instance.cameraController.keyFrames);
        }
        public static void RemoveAllImpulseKeys()
        {
            RemoveKeyFramesOfType(typeof(ImpulseKeyFrame));
            //CurveUtil.Refresh();
        }
        public static void RemoveAllPlaybackKeys()
        {
            //CurveUtil.playbackSpeedCurve.Clear();
            RemoveKeyFramesOfType(typeof(PlaybackSpeedKeyFrame));
            //CurveUtil.Refresh();
        }
        private static void RemoveKeyFramesOfType(Type keyframeType)
        {
            var keyframes = ReplayEditorController.Instance.cameraController.keyFrames;

            // Loop backwards so removing an item doesn't mess up the index of the next item
            for (int i = keyframes.Count - 1; i >= 0; i--)
            {
                if (keyframes[i].GetType() == keyframeType)
                {
                    ReplayEditorController.Instance.cameraController.keyFrames.Remove(keyframes[i]);
                }
            }
        }
        public static void CreatePlaybackKeyFrame(float playbackspeed, float time)
        {
            //time = time + Main.settings.time_offset; // time offset if needed

            int index = FindKeyFrameInsertIndex(time);
            KeyFrame keyFrame;

            keyFrame = new PlaybackSpeedKeyFrame(playbackspeed, time);
            keyFrame.AddKeyframes(ReplayEditorController.Instance.cameraController.cameraCurve);
            //keyFrame.AddKeyframes(Main.camNoiseController.customCurve);
            ReplayEditorController.Instance.cameraController.keyFrames.Insert(index, keyFrame);

            Main.Logger.Log("PlayBack KeyFrame added at: " + time);
        }

        public static void CreateImpluseKeyFrame(CinemachineImpulseSource impulseSource, float time)
        {
            //time = time + Main.settings.time_offset; // time offset if needed

            int index = FindKeyFrameInsertIndex(time);
            KeyFrame keyFrame;

            //keyFrame = new FreeCameraKeyFrame(copy.transform, Main.settings.keyframe_fov, time);
            keyFrame = new ImpulseKeyFrame(impulseSource, Main.settings.impulseForce, time);
            //keyFrame.AddKeyframes(ReplayEditorController.Instance.cameraController.cameraCurve);
            keyFrame.ApplyTo(ReplayEditorController.Instance.cameraController.VirtualCamera);
            //keyFrame.Update(ReplayEditorController.Instance.cameraController.VirtualCamera.transform, time);
            ReplayEditorController.Instance.cameraController.keyFrames.Insert(index, keyFrame);

            Main.Logger.Log("Impulse KeyFrame added at: " + time);
        }
        private static int FindKeyFrameInsertIndex(float time)
        {
            var keyFrames = ReplayEditorController.Instance.cameraController.keyFrames;
            int index = keyFrames.FindIndex(k => k.time > time);
            return index == -1 ? keyFrames.Count : index;
        }

        /*
        private static int FindKeyFrameInsertIndex(float time)
        {
            if (ReplayEditorController.Instance.cameraController.keyFrames.Count == 0)
            {
                return 0;
            }
            if (time < ReplayEditorController.Instance.cameraController.keyFrames[0].time)
            {
                return 0;
            }
            if (ReplayEditorController.Instance.cameraController.keyFrames.Count == 1)
            {
                return 1;
            }
            for (int i = 0; i < ReplayEditorController.Instance.cameraController.keyFrames.Count - 1; i++)
            {
                if (time > ReplayEditorController.Instance.cameraController.keyFrames[i].time && time < ReplayEditorController.Instance.cameraController.keyFrames[i + 1].time)
                {
                    return i + 1;
                }
            }
            return ReplayEditorController.Instance.cameraController.keyFrames.Count;
        }
        */
    }
}
