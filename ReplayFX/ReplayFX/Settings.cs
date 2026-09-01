using System;
using UnityModManagerNet;
using UnityEngine;

namespace ReplayFX
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings
    {
        public bool enableNoise = true;

        public KeyBinding noiseHotkey = new KeyBinding { keyCode = KeyCode.R };

        public float noise_amplitude = 1.0f;
        public float noise_frequency = 1.0f;

        public float noise_offset_x = 0.0f;
        public float noise_offset_y = 0.0f;
        public float noise_offset_z = 0.0f;

        public float impulse_force = 1.0f;
        public float impulse_source_amplitude = 2.0f;
        public float impulse_source_frequency = 1.0f;
        public float impulse_source_decaytime = 0.5f;

        public float replay_playback_speed = 1.0f;

        public float playback_color_value = 0.50f;
        public float impulse_color_value = 0.40f;

        public bool isPlaybackGreyscale = false;
        public bool isImpulseGreyscale = true;

        //public bool useAssetBundleProfiles = true;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}