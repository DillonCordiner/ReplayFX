using Cinemachine;
using UnityEngine;

namespace ReplayTestMod.Utils
{
    public static class NoiseUtils
    {
        public static NoiseSettings Create6DShakeProfile()
        {
            NoiseSettings profile = ScriptableObject.CreateInstance<NoiseSettings>();
            profile.name = "Procedural 6D Shake";

            profile.PositionNoise = new NoiseSettings.TransformNoiseParams[]
            {
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 1.0f, Amplitude = 0.3f },
                Y = new NoiseSettings.NoiseParams { Frequency = 1.2f, Amplitude = 0.3f },
                Z = new NoiseSettings.NoiseParams { Frequency = 0.8f, Amplitude = 0.3f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 8.0f, Amplitude = 0.1f },
                Y = new NoiseSettings.NoiseParams { Frequency = 7.5f, Amplitude = 0.1f },
                Z = new NoiseSettings.NoiseParams { Frequency = 9.0f, Amplitude = 0.1f }
            }
            };
            profile.OrientationNoise = new NoiseSettings.TransformNoiseParams[]
            {
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 1.5f, Amplitude = 1.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 1.3f, Amplitude = 1.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 1.1f, Amplitude = 1.0f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 12.0f, Amplitude = 0.3f },
                Y = new NoiseSettings.NoiseParams { Frequency = 10.5f, Amplitude = 0.3f },
                Z = new NoiseSettings.NoiseParams { Frequency = 11.0f, Amplitude = 0.3f }
            }
            };

            return profile;
        }
    }
}
