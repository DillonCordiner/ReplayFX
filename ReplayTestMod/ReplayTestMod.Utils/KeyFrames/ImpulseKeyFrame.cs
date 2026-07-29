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
        private float force;

        public ImpulseKeyFrame(CinemachineImpulseSource newimpulseSource, float impulseForce ,float currenttime)
        {
            impulseSource = newimpulseSource;
            time = currenttime;
            force = impulseForce;
        }
        public void TriggerImpulse()
        {
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse(force);
                Main.Logger.Log($"Impulse triggered at time: {time}");
            }
        }
        public override void ApplyTo(CinemachineVirtualCamera camera)
        {
            TriggerImpulse();
            Main.Logger.Log("ImpulseKeyFrame: ApplyTo Called");
        }

        public override void AddKeyframes(CameraCurve cameraCurve)
        {
            // Leave empty. An impulse shouldn't change the camera's X/Y/Z spline path.
            //cameraCurve.CalculateCurveControlPoints();
            Main.Logger.Log("ImpulseKeyFrame: Add Key Frame Called");
        }

        public override void Update(Transform cameraTransform, float t)
        {
            //impulseSource.GenerateImpulse(Vector3.down);
            Main.Logger.Log("ImpulseKeyFrame: Update Called");
        }
    }
}
