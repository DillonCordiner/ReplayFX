using System;
using UnityModManagerNet;

namespace ReplayTestMod
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings
    {
        public float amplitude = 1.0f;
        public float frequency = 1.0f;

        public float offset_x = 0.0f;
        public float offset_y = 0.0f;
        public float offset_z = 0.0f;

        public float playBackSpeed = 1.0f;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}