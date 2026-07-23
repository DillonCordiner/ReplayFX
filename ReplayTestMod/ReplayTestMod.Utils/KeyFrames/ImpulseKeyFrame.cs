using Cinemachine;
using ReplayEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReplayTestMod.Utils
{
    public class ImpulseKeyFrame : KeyFrame
    {
        private CinemachineImpulseSource impulseSource;

        public ImpulseKeyFrame(CinemachineImpulseSource newimpulseSource, float currenttime)
        {
            impulseSource = newimpulseSource;
            time = currenttime;
        }

        public override void ApplyTo(CinemachineVirtualCamera camera)
        {
            //camera.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
            impulseSource.GenerateImpulse();

            Main.Logger.Log("ImpulseKeyFrame: Apply To Called");
        }
       
        public override void AddKeyframes(CameraCurve cameraCurve)
        {
            //cameraCurve.freeCamCurve.InsertCurveKey(1.0f, this.time);
            cameraCurve.CalculateCurveControlPoints();

            Main.Logger.Log("ImpulseKeyFrame: Add Key Frame Called");
        }

        public override void Update(Transform cameraTransform, float t)
        {
            impulseSource.GenerateImpulse();

            Main.Logger.Log("ImpulseKeyFrame: Update Called");
        }
    }
}
