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
    public enum ProfileOptions
    {
        None,
        Shake,
        Normal_extreme,
        Normal_mild,
        Normal_strong,
        Tele_mild,
        Tele_strong,
        Wideangle_mild,
        Wideangle_strong
    }

    public class NoiseController : MonoBehaviour
    {
        private CinemachineVirtualCamera Vcam;
        private CinemachineBasicMultiChannelPerlin noise;
        private CinemachineImpulseListener impulseListener;
        public CinemachineImpulseSource impulseSource;
        //public CustomCameraCurve customCurve = new CustomCameraCurve();

        //NoiseSettings blankProfile = new NoiseSettings();
        private NoiseSettings blankProfile;

        public List<NoiseSettings> noiseSettings = new List<NoiseSettings>();

        private const string none = "None";
        public string targetProfile = none;
        public string currentProfile { get; private set; } = "";
        //private string storedProfile = empty;
        public string[] ProfileOptionsArray = Enum.GetNames(typeof(ProfileOptions));

        /*
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
        */

        private void Start()
        {
            Vcam = GetVirtualCamera();
            blankProfile = NoiseUtils.CreateBlankProfile();
            SetUpNoiseProfiles();
            AddNoiseToCamera();
            SetDefaultNoiseProfile();
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
            //NoiseSettings generatedShake = NoiseUtils.CreateShakeProfile();
            NoiseSettings generatedShake = NoiseUtils.CreateShakeCustomProfile();
            source.m_ImpulseDefinition.m_RawSignal = generatedShake;
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

            noiseSettings.Clear();

            noiseSettings.Add(NoiseUtils.CreateShakeProfile());
            noiseSettings.Add(NoiseUtils.Create_Normal_Extreme_Profile());
            noiseSettings.Add(NoiseUtils.Create_Normal_Mild_Profile());
            noiseSettings.Add(NoiseUtils.Create_Normal_Strong_Profile());
            noiseSettings.Add(NoiseUtils.Create_Tele_Mild_Profile());
            noiseSettings.Add(NoiseUtils.Create_Tele_Strong_Profile());
            noiseSettings.Add(NoiseUtils.Create_Wideangle_Mild_Profile());
            noiseSettings.Add(NoiseUtils.Create_Wideangle_Strong_Profile());
        }
        private void SetDefaultNoiseProfile()
        {
            if (Main.settings.enableNoise)
            {
                targetProfile = Main.settings.savedProfile;
            }
        }
        public void LoadNoiseProfile(NoiseSettings noiseProfile)
        {
            noise.m_NoiseProfile = noiseProfile;
        }
        private NoiseSettings GetCurrentProfile(List<NoiseSettings> noisesettings)
        {
            if (noise == null || noisesettings.Count <= 0)
                return null;

            if (targetProfile == none || targetProfile == "")
            {
                return blankProfile;
            }

            foreach (NoiseSettings profile in noisesettings)
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
            if (noise == null || currentProfile == targetProfile)
                return;

            /*
            NoiseSettings profile;
            if (Main.settings.useAssetBundleProfiles)
            {
                profile = GetCurrentProfile(AssetLoader.noiseSettingsAssets);
            }
            else
            {
                profile = GetCurrentProfile(noiseSettings);
            }
            */

            //NoiseSettings profile = GetCurrentProfile(noiseSettings);

            if (AssetLoader.noiseSettingsAssets.Count <= 0)
                return;

            NoiseSettings profile = GetCurrentProfile(AssetLoader.noiseSettingsAssets);
            LoadNoiseProfile(profile);
            //currentProfile = targetProfile;
            currentProfile = profile.name;

        }    
        private void UpdateNoiseProfileValues()
        {
            if (noise.m_NoiseProfile == null || noise.m_NoiseProfile.name == none || noise.m_NoiseProfile.name == "")
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
            if (noise.m_NoiseProfile == null || noise.m_NoiseProfile.name == none || noise.m_NoiseProfile.name == "")
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
                    if (targetProfile != Main.settings.savedProfile)
                    {
                        targetProfile = Main.settings.savedProfile;
                    }
                    Main.rfxSettings.cameraSettings.SetVisible("camera_profile", true);
                    //Main.rfxSettings.cameraSettings.UpdateItem("camera_profile");
                    Main.rfxSettings.cameraSettings.UpdatePage();
                    break;

                case false:
                    Main.settings.savedProfile = targetProfile;
                    targetProfile = none;
                    Main.rfxSettings.cameraSettings.SetVisible("camera_profile", false);
                    //Main.rfxSettings.cameraSettings.UpdateItem("camera_profile");
                    Main.rfxSettings.cameraSettings.UpdatePage();
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

            if (impulseSource.m_ImpulseDefinition.m_AmplitudeGain != Main.settings.impulse_source_amplitude)
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