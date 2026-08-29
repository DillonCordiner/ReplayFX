using Cinemachine;
using ReplayEditor;
using UnityEngine;
using HarmonyLib;
using ReplayFX.Utils;

namespace ReplayFX.Keyframes
{
    public class PlaybackSpeedKeyFrame : KeyFrame
    {
        /// <summary>
        /// NOTE: whenever a PlaybackSpeedKeyFrame is deleted or moved make sure CurveUtil.playbackSpeedCurve.Clear() is called to repopulate curve. ** only when using games built in curve **
        /// </summary>

        public float targetSpeed;

        public PlaybackSpeedKeyFrame(float speed, float currentTime)
        {
            targetSpeed = speed;
            time = currentTime;
        }

        public override void ApplyTo(CinemachineVirtualCamera camera)
        {
            //Main.Logger.Log("PlayBackSpeedKeyFrame: Apply To");
        }

        public override void AddKeyframes(CameraCurve cameraCurve)
        {
            CurveUtil.playbackSpeedCurve.InsertCurveKey(targetSpeed, time);
            CurveUtil.playbackSpeedCurve.CalculateCurveControlPoints();
            //Main.Logger.Log("PlayBackSpeedKeyFrame: AddKeyFrame");
        }

        public override void Update(Transform cameraTransform, float t)
        {
            //Main.Logger.Log("PlayBackSpeedKeyFrame: Update");
        }
    }
}
