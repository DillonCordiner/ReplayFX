using Cinemachine;
using ReplayEditor;
using ReplayFX.Utils;
using RootMotion;
using System;

namespace ReplayFX.Keyframes
{
    public static class KeyFrameHelper
    {
        public static void AddImpluseKeyFrame()
        {
            CreateImpluseKeyFrame(Main.camNoiseController.impulseSource, ReplayEditorController.Instance.playbackController.CurrentTime);
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
            //CurveUtil.Refresh();
        }
        public static void RemoveAllPlaybackKeys()
        {
            RemoveKeyFramesOfType(typeof(PlaybackSpeedKeyFrame));
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
        public static void CreatePlaybackKeyFrame(float playbackspeed, float time)
        {
            int index = FindKeyFrameInsertIndex(time);
            KeyFrame keyFrame;

            keyFrame = new PlaybackSpeedKeyFrame(playbackspeed, time);
            keyFrame.AddKeyframes(ReplayEditorController.Instance.cameraController.cameraCurve);
            //keyFrame.AddKeyframes(Main.camNoiseController.customCurve);
            ReplayEditorController.Instance.cameraController.keyFrames.Insert(index, keyFrame);

            //Main.Logger.Log("PlayBack KeyFrame added at: " + time);
        }

        public static void CreateImpluseKeyFrame(CinemachineImpulseSource impulseSource, float time)
        {
            int index = FindKeyFrameInsertIndex(time);
            KeyFrame keyFrame;

            keyFrame = new ImpulseKeyFrame(impulseSource, Main.settings.impulse_force, time);
            keyFrame.ApplyTo(ReplayEditorController.Instance.cameraController.VirtualCamera);
            ReplayEditorController.Instance.cameraController.keyFrames.Insert(index, keyFrame);

            //Main.Logger.Log("Impulse KeyFrame added at: " + time);
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
