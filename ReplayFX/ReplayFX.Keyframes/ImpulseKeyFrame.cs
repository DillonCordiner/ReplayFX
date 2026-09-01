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
        private float amplitude;
        private float frequency;
        private float decay;

        public ImpulseKeyFrame(CinemachineImpulseSource newimpulseSource, float currenttime, float impulseForce, float impulseAmplitude, float impulseFrequency, float impulseDecay)
        {
            impulseSource = newimpulseSource;
            time = currenttime;
            force = impulseForce;
            amplitude = impulseAmplitude;
            frequency = impulseFrequency;
            decay = impulseDecay;
        }
        public void TriggerKeyFrame()
        {
            if (impulseSource != null)
            {
                impulseSource.m_ImpulseDefinition.m_AmplitudeGain = amplitude;
                impulseSource.m_ImpulseDefinition.m_FrequencyGain = frequency;
                impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = decay;
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
