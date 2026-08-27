using UnityEngine;
using Cinemachine;
using ReplayFX.Utils;
using System.Collections.Generic;
using ReplayEditor;
using System.Linq;
using System;
using UnityEngine.Profiling;

namespace ReplayFX
{
    public class CamNoiseController : MonoBehaviour
    {
        CinemachineVirtualCamera Vcam;
        CinemachineBasicMultiChannelPerlin noise;
        CinemachineImpulseListener impulseListener;
        public CinemachineImpulseSource impulseSource;
        //public CustomCameraCurve customCurve = new CustomCameraCurve();

        NoiseSettings blankProfile = new NoiseSettings();

        List<NoiseSettings> noiseSettings = new List<NoiseSettings>();

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
            SetUpNoiseProfiles();
            Vcam = GetVirtualCamera();
            AddNoiseToCamera();
            AddCameraExtensions();
            AddImpulseSource();
        }

        private void Update()
        {
            if (noise == null)
                return;

            UpdateNoiseProfile();
            UpdateNoiseProfileValues();
            UpdatePivotOffset();
            UpdateImpulseValues();
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
            impulseListener.m_Gain = 2.0f;
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
            //NoiseSettings generated6DShake = NoiseUtils.Create6DShakeProfile();
            NoiseSettings generated6DShake = NoiseUtils.Create6DShakeCustomProfile();
            source.m_ImpulseDefinition.m_RawSignal = generated6DShake;
            source.m_ImpulseDefinition.m_AmplitudeGain = 2.0f;
            source.m_ImpulseDefinition.m_FrequencyGain = 1.0f;
            source.m_ImpulseDefinition.m_ImpactRadius = 100.0f;
            source.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime = 0.0f;
            source.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = 0.2f;
            source.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = 0.5f;
            source.m_ImpulseDefinition.m_ImpulseChannel = 1;
            source.m_ImpulseDefinition.m_PropagationSpeed = float.MaxValue;
        }
        private void SetUpNoiseProfiles()
        {
            if (noiseSettings == null)
                return;

            noiseSettings.Add(NoiseUtils.Create6DShakeProfile());
            noiseSettings.Add(NoiseUtils.Create_Handheld_Normal_Extreme_Profile());
            noiseSettings.Add(NoiseUtils.Create_Handheld_Normal_Mild_Profile());
            noiseSettings.Add(NoiseUtils.Create_Handheld_Normal_Strong_Profile());
            noiseSettings.Add(NoiseUtils.Create_Handheld_Tele_Mild_Profile());
            noiseSettings.Add(NoiseUtils.Create_Handheld_Tele_Strong_Profile());
            noiseSettings.Add(NoiseUtils.Create_Handheld_Wideangle_Mild_Profile());
            noiseSettings.Add(NoiseUtils.Create_Handheld_Wideangle_Strong_Profile());
        }
        public void LoadNoiseProfile(NoiseSettings noiseProfile)
        {
            noise.m_NoiseProfile = noiseProfile;
        }
        private NoiseSettings GetCurrentProfile()
        {
            if (noise == null)
                return null;

            if (targetProfile == empty)
            {
                return blankProfile;
            }

            foreach (NoiseSettings profile in noiseSettings)
            {
                if (profile.name == targetProfile)
                {
                    return profile;
                }
            }
            return null;
        }
        
        private NoiseSettings GetCurrentProfileFromAssets()
        {
            if(noise == null)
                return null;

            if (targetProfile == empty)
            {
                return blankProfile;
            }
            foreach (NoiseSettings profile in AssetLoader.noiseSettingsAssets)
            {
                if (profile.name == targetProfile)
                {
                    return profile;
                }
            }
            return null;
        }
        
        private void UpdateNoiseProfile()
        {
            if (currentProfile == targetProfile)
                return;

            /* //for testing camera shake asset vs generated values
            NoiseSettings profile;
            if (Main.settings.useAssetBundleProfiles)
            {
                profile = GetCurrentProfileFromAssets();
            }
            else
            {
                profile = GetCurrentProfile();
            }
            */

            NoiseSettings profile = GetCurrentProfileFromAssets();
            //NoiseSettings profile = GetCurrentProfile();
            LoadNoiseProfile(profile);
            currentProfile = targetProfile;

        }    
        private void UpdateNoiseProfileValues()
        {
            if (noise.m_NoiseProfile.name == empty)
                return;

            if (noise.m_AmplitudeGain != Main.settings.noise_amplitude)
            {
                noise.m_AmplitudeGain = Main.settings.noise_amplitude;
            }
            else if (noise.m_FrequencyGain != Main.settings.noise_frequency)
            {
                noise.m_FrequencyGain = Main.settings.noise_frequency;
            }
        }
        public void UpdatePivotOffset()
        {
            if (noise.m_NoiseProfile.name == empty)
                return;

            if (noise.m_PivotOffset.x != Main.settings.noise_offset_x ||
                noise.m_PivotOffset.y != Main.settings.noise_offset_y ||
                noise.m_PivotOffset.z != Main.settings.noise_offset_z)
            {
                noise.m_PivotOffset.Set(Main.settings.noise_offset_x, Main.settings.noise_offset_y, Main.settings.noise_offset_z);
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
                    //storedProfile = GetCurrentProfile().name;
                    storedProfile = GetCurrentProfileFromAssets().name;
                    targetProfile = empty;
                    break;

            }
        }
        public void GenerateImpluse()
        {
            impulseSource.GenerateImpulse(Main.settings.impulse_force);
        }

        public void UpdateImpulseValues()
        {
            if (impulseSource == null || impulseListener == null)
                return;

            if (impulseListener.m_Gain != Main.settings.impulse_listener_gain)
            {
                impulseListener.m_Gain = Main.settings.impulse_listener_gain;
            }
            else if (impulseSource.m_ImpulseDefinition.m_AmplitudeGain != Main.settings.impulse_source_amplitude)
            {
                impulseSource.m_ImpulseDefinition.m_AmplitudeGain = Main.settings.impulse_source_amplitude;
            }
            else if (impulseSource.m_ImpulseDefinition.m_FrequencyGain != Main.settings.impulse_source_frequency)
            {
                impulseSource.m_ImpulseDefinition.m_FrequencyGain = Main.settings.impulse_source_frequency;
            }
            else if (impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime != Main.settings.impulse_source_decaytime)
            {
                impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = Main.settings.impulse_source_decaytime;
            }
        }

    }
}