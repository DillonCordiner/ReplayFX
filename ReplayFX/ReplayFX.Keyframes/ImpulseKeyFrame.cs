using Cinemachine;
using ReplayEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReplayFX.Keyframes
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
        public void TriggerKeyFrame()
        {
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse(force);
                //Main.Logger.Log($"Impulse triggered at time: {time}");
            }
        }
        public override void ApplyTo(CinemachineVirtualCamera camera)
        {
            TriggerKeyFrame();
            //Main.Logger.Log("ImpulseKeyFrame: ApplyTo Called");
        }

        public override void AddKeyframes(CameraCurve cameraCurve)
        {
            //Main.Logger.Log("ImpulseKeyFrame: Add Key Frame Called");
        }

        public override void Update(Transform cameraTransform, float t)
        {
            //Main.Logger.Log("ImpulseKeyFrame: Update Called");
        }
    }
}
