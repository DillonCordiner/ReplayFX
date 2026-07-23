using Cinemachine;
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
            ReplayEditorController.Instance.cameraController.keyframeUI.UpdateKeyframes(ReplayEditorController.Instance.cameraController.keyFrames);
        }

        public static void AddPlayBackKeyFrame()
        {
            CreatePlaybackKeyFrame(Main.settings.playBackSpeed, ReplayEditorController.Instance.playbackController.CurrentTime);
            ReplayEditorController.Instance.cameraController.keyframeUI.UpdateKeyframes(ReplayEditorController.Instance.cameraController.keyFrames);
        }

        public static void CreatePlaybackKeyFrame(float playbackspeed, float time)
        {
            //time = time + Main.settings.time_offset; // time offset if needed

            int index = FindKeyFrameInsertIndex(time);
            KeyFrame keyFrame;

            keyFrame = new PlaybackSpeedKeyFrame(playbackspeed, time);
            //keyFrame.AddKeyframes(ReplayEditorController.Instance.cameraController.cameraCurve);
            keyFrame.AddKeyframes(Main.camNoiseController.customCurve);
            ReplayEditorController.Instance.cameraController.keyFrames.Insert(index, keyFrame);

            Main.Logger.Log("PlayBack KeyFrame added at: " + time);
        }

        public static void CreateImpluseKeyFrame(CinemachineImpulseSource impulseSource, float time)
        {
            //time = time + Main.settings.time_offset; // time offset if needed

            int index = FindKeyFrameInsertIndex(time);
            KeyFrame keyFrame;

            //keyFrame = new FreeCameraKeyFrame(copy.transform, Main.settings.keyframe_fov, time);
            keyFrame = new ImpulseKeyFrame(impulseSource, time);
            keyFrame.AddKeyframes(ReplayEditorController.Instance.cameraController.cameraCurve);
            ReplayEditorController.Instance.cameraController.keyFrames.Insert(index, keyFrame);

            Main.Logger.Log("Impulse KeyFrame added at: " + time);
        }

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
    }
}
