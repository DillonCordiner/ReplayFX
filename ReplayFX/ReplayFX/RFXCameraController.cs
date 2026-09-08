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

    public class RFXCameraController : MonoBehaviour
    {
        private CinemachineVirtualCamera Vcam;
        private CinemachineBasicMultiChannelPerlin noise;
        private CinemachineImpulseListener impulseListener;
        public CinemachineImpulseSource impulseSource;
        //public CustomCameraCurve customCurve = new CustomCameraCurve();

        //NoiseSettings blankProfile = new NoiseSettings();
        private NoiseSettings blankProfile;

        public List<NoiseSettings> noiseSettings = new List<NoiseSettings>();
        private bool lastEnableNoise;
        private const string none = "None";
        public string targetProfile = none;
        public string currentProfile { get; private set; } = "";

        public readonly string[] ProfileOptionsArray = Enum.GetNames(typeof(ProfileOptions));
        public readonly string[] recordedFPSarray = new string[] { "15", "30", "60" };

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
            UpdateNoiseState();
            UpdateNoiseProfile();
            UpdateNoiseProfileValues();
            UpdatePivotOffset();
            UpdateImpulseValues();
            UpdateRecordedFPS();
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
            if (impulseListener != null)
            {
                impulseListener.m_Gain = 2.0f;
                impulseListener.m_ChannelMask = 1;
            }
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
            if(noise == null)
                return;

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

            if (AssetLoader.noiseSettingsAssets.Count <= 0)
                return;

            NoiseSettings profile = GetCurrentProfile(AssetLoader.noiseSettingsAssets);
            LoadNoiseProfile(profile);
            //currentProfile = targetProfile;
            currentProfile = profile.name;

        }    
        private void UpdateNoiseProfileValues()
        {
            if (noise == null || noise.m_NoiseProfile == null || noise.m_NoiseProfile.name == none || noise.m_NoiseProfile.name == "")
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
            if (noise == null || noise.m_NoiseProfile == null || noise.m_NoiseProfile.name == none || noise.m_NoiseProfile.name == "")
                return;

            if (noise.m_PivotOffset.x != Main.settings.noise_offset_x ||
                noise.m_PivotOffset.y != Main.settings.noise_offset_y ||
                noise.m_PivotOffset.z != Main.settings.noise_offset_z)
            {
                //noise.m_PivotOffset.Set(Main.settings.noise_offset_x, Main.settings.noise_offset_y, Main.settings.noise_offset_z);
                noise.m_PivotOffset = new Vector3(Main.settings.noise_offset_x, Main.settings.noise_offset_y, Main.settings.noise_offset_z);
            }
        }
        public void UpdateRecordedFPS()
        {
            if (ReplaySettings.Instance == null || recordedFPSarray == null || recordedFPSarray.Length <= 0)
                return;

            if (Main.settings.replay_recorded_fps != ReplaySettings.Instance.FPS)
            {
                ReplaySettings.Instance.FPS = Main.settings.replay_recorded_fps;
            }
        }
        public void GenerateNewSeed()
        {
            if (noise == null)
                return;

            noise.ReSeed();
        }  
        public void ToggleNoise()
        {
            Main.settings.enableNoise = !Main.settings.enableNoise;
        } 
        private void UpdateNoiseState()
        {
            if (Main.settings.enableNoise != lastEnableNoise)
            {
                lastEnableNoise = Main.settings.enableNoise;
                ApplyNoiseState(lastEnableNoise);
            }
        }
        private void ApplyNoiseState(bool isEnabled)
        {
            if (isEnabled)
            {
                if (targetProfile != Main.settings.savedProfile)
                {
                    targetProfile = Main.settings.savedProfile;
                }
            }
            else
            {
                Main.settings.savedProfile = targetProfile;
                targetProfile = none;
            }
            if (Main.replayfxMenu.cameraMenuPage != null)
            {
                Main.replayfxMenu.cameraMenuPage.SetVisible("camera_profile", isEnabled);
                Main.replayfxMenu.cameraMenuPage.UpdatePage();
            }
        }
        public void GenerateImpluse()
        {
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse(Main.settings.impulse_force);
            }
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