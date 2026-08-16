using UnityEngine;
using Cinemachine;
using ReplayTestMod.Utils;
using System.Collections.Generic;
using ReplayEditor;
using System.Linq;
using System;
using UnityEngine.Profiling;

namespace ReplayTestMod
{
    public class CamNoiseController : MonoBehaviour
    {
        CinemachineVirtualCamera Vcam;
        CinemachineBasicMultiChannelPerlin noise;
        CinemachineImpulseListener impulseListener;
        public CinemachineImpulseSource impulseSource;
        //public CustomCameraCurve customCurve = new CustomCameraCurve();

        NoiseSettings blankProfile = new NoiseSettings();

        private const string empty = "None";
        public string targetProfile = empty;
        private string currentProfile = "";
        private string storedProfile = empty;

        public string[] ProfileOptions = new string[] {
            "None",
            "6D Shake",
            "Handheld_normal_extreme",
            "Handheld_normal_mild",
            "Handheld_normal_strong",
            "Handheld_tele_mild",
            "Handheld_tele_strong",
            "Handheld_wideangle_mild",
            "Handheld_wideangle_strong"
        };

        private void Start()
        {
            blankProfile.name = empty;
            Vcam = GetVirtualCamera();
            AddNoiseToCamera();
            AddCameraExtensions();
            AddImpulseSource();
        }

        private void Update()
        {
            if (noise == null)
                return;

            UpdateProfile();
            UpdateValues();
            UpdatePivotOffset();
        }
        private CinemachineVirtualCamera GetVirtualCamera()
        {
            if (ReplayEditorController.Instance == null)
            {
                Main.Logger.Log("ReplayEditorController.Instance is null.");
                return null;
            }

            CinemachineVirtualCamera vcam = ReplayEditorController.Instance.cameraController.VirtualCamera;

            if (vcam == null)
            {
                Main.Logger.Log("CinemachineVirtualCamera is missing.");
            }

            return vcam;
        }
        private void AddCameraExtensions()
        {
            if (Vcam == null)
            {
                Main.Logger.Log("CinemachineVirtualCamera is missing.");
                return;
            }
            Vcam.AddExtension(impulseListener);
            impulseListener = Vcam.gameObject.AddComponent<CinemachineImpulseListener>();
            impulseListener.m_Gain = 1f;
            impulseListener.m_ChannelMask = 1;
        }
        private void AddNoiseToCamera()
        {
            if (Vcam == null)
            {
                Main.Logger.Log("CinemachineVirtualCamera is missing.");
                return;
            }

            noise = Vcam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        private void AddImpulseSource()
        {
            if (Vcam != null)
            {
                impulseSource = Vcam.gameObject.AddComponent<CinemachineImpulseSource>();
                if (impulseSource != null)
                {
                    SetUpImpulseSourse(impulseSource);
                }
            }
        }
        private void SetUpImpulseSourse(CinemachineImpulseSource source)
        {
            NoiseSettings generated6DShake = NoiseUtils.Create6DShakeProfile();
            source.m_ImpulseDefinition.m_RawSignal = generated6DShake;
            source.m_ImpulseDefinition.m_AmplitudeGain = 2f;
            source.m_ImpulseDefinition.m_FrequencyGain = 3f;
            source.m_ImpulseDefinition.m_ImpactRadius = 100f;
            source.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime = 0.0f;
            source.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = 0.2f;
            source.m_ImpulseDefinition.m_ImpulseChannel = 1;
            source.m_ImpulseDefinition.m_PropagationSpeed = float.MaxValue;
        }
        public void LoadNoiseProfile(NoiseSettings noiseProfile)
        {
            noise.m_NoiseProfile = noiseProfile;
        }
        private NoiseSettings GetCurrentProfile()
        {
            if(noise == null)
                return null;

            if (targetProfile == empty)
            {
                return blankProfile;
            }
            foreach (NoiseSettings profile in AssetLoader.noiseSettings)
            {
                if (profile.name == targetProfile)
                {
                    return profile;
                }
            }
            return null;
        }
        private void UpdateProfile()
        {
            if (currentProfile == targetProfile)
                return;

            NoiseSettings profile = GetCurrentProfile();
            LoadNoiseProfile(profile);
            currentProfile = targetProfile;

        }    
        private void UpdateValues()
        {
            if (noise.m_NoiseProfile.name == empty)
                return;

            if (noise.m_AmplitudeGain != Main.settings.amplitude)
            {
                noise.m_AmplitudeGain = Main.settings.amplitude;
            }
            if (noise.m_FrequencyGain != Main.settings.frequency)
            {
                noise.m_FrequencyGain = Main.settings.frequency;
            }
        }
        public void UpdatePivotOffset()
        {
            if (noise.m_NoiseProfile.name == empty)
                return;

            if (noise.m_PivotOffset.x != Main.settings.offset_x ||
                noise.m_PivotOffset.y != Main.settings.offset_y ||
                noise.m_PivotOffset.z != Main.settings.offset_z)
            {
                noise.m_PivotOffset.Set(Main.settings.offset_x, Main.settings.offset_y, Main.settings.offset_z);
            }
        }
        public void GenerateNewSeed()
        {
            noise.ReSeed();
        }

        public void ToggleNoise()
        {
            //noise.enabled = Main.settings.enableNoise;
            Main.settings.enableNoise = !Main.settings.enableNoise;

            switch (Main.settings.enableNoise)
            {
                case true:
                    if (targetProfile != storedProfile)
                    {
                        targetProfile = storedProfile;
                    }
                    break;

                case false:
                    storedProfile = GetCurrentProfile().name;
                    targetProfile = empty;
                    break;

            }
        }
        public void GenerateImpluse()
        {
            impulseSource.GenerateImpulse(Main.settings.impulseForce);
        }
    }
}