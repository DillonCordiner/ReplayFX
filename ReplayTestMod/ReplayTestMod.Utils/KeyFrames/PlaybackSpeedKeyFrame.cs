using Cinemachine;
using ReplayEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;

namespace ReplayTestMod.Utils
{
    public class PlaybackSpeedKeyFrame : KeyFrame
    {
        public float playbackSpeed;

        public PlaybackSpeedKeyFrame(float playbackSpeed, float time)
        {
            this.playbackSpeed = playbackSpeed;
            this.time = time;
        }

        public override void ApplyTo(CinemachineVirtualCamera camera)
        {
            Main.Logger.Log("PlayBackSpeedKeyFrame: Apply To");
        }

        public override void AddKeyframes(CameraCurve cameraCurve)
        {
            if (cameraCurve is CustomCameraCurve customCurve)
            {
                customCurve.playbackSpeedCurve.InsertCurveKey(this.playbackSpeed, this.time);
                customCurve.CalculateCurveControlPoints();
            }

            //cameraCurve.freeCamCurve.InsertCurveKey(this.playbackSpeed, this.time);
            //cameraCurve.CalculateCurveControlPoints();

            Main.Logger.Log("PlayBackSpeedKeyFrame: AddKeyFrame");
        }

        public override void Update(Transform cameraTransform, float t)
        {
            Traverse.Create(ReplayEditorController.Instance).Field("playbackSpeed").SetValue(this.playbackSpeed);

            Main.Logger.Log("PlayBackSpeedKeyFrame: Update");
        }
    }
}
