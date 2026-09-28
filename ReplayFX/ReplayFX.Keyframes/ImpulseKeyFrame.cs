using Cinemachine;
using ReplayEditor;
using UnityEngine;
using ReplayFX.Utils;

namespace ReplayFX.Keyframes
{
    public class ImpulseKeyFrame : KeyFrame
    {
        public CinemachineImpulseSource impulseSource;
        public float force;
        public float amplitude;
        public float frequency;
        public float decay;

        public ImpulseKeyFrame(CinemachineImpulseSource newImpulseSource, float currentTime, float impulseForce, float impulseAmplitude, float impulseFrequency, float impulseDecay)
        {
            impulseSource = newImpulseSource;
            time = currentTime;
            force = impulseForce;
            amplitude = impulseAmplitude;
            frequency = impulseFrequency;
            decay = impulseDecay;
        }

        public void TriggerKeyFrame()
        {
            if (impulseSource == null)
            {
                impulseSource = Main.camController?.impulseSource;
            }

            if (impulseSource != null)
            {
                impulseSource.m_ImpulseDefinition.m_AmplitudeGain = amplitude;
                impulseSource.m_ImpulseDefinition.m_FrequencyGain = frequency;
                impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = decay;
                impulseSource.GenerateImpulse(force);
            }
        }

        public override void ApplyTo(CinemachineVirtualCamera camera)
        {
            TriggerKeyFrame();
        }

        public override void AddKeyframes(CameraCurve cameraCurve)
        {
        }

        public override void Update(Transform cameraTransform, float t)
        {
        }
    }
}