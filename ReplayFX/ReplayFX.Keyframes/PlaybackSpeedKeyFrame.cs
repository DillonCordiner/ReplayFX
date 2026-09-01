using Cinemachine;
using ReplayEditor;
using UnityEngine;
using HarmonyLib;
using ReplayFX.Utils;

namespace ReplayFX.Keyframes
{
    public class PlaybackSpeedKeyFrame : KeyFrame
    {
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
            //CurveUtil.playbackSpeedCurve.CalculateCurveControlPoints();
            //Main.Logger.Log("PlayBackSpeedKeyFrame: AddKeyFrame");
        }

        public override void Update(Transform cameraTransform, float t)
        {
            //Main.Logger.Log("PlayBackSpeedKeyFrame: Update");
        }
    }
}
