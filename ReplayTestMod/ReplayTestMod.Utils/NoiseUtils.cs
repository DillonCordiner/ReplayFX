using Cinemachine;
using UnityEngine;

namespace ReplayTestMod.Utils
{
    public static class NoiseUtils
    {
        public static NoiseSettings Create6DShakeCustomProfile()
        {
            NoiseSettings profile = ScriptableObject.CreateInstance<NoiseSettings>();
            profile.name = "Custom 6D Shake";

            profile.PositionNoise = new NoiseSettings.TransformNoiseParams[]
            {
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 4.2f, Amplitude = 0.011f },
                Y = new NoiseSettings.NoiseParams { Frequency = 12.7f, Amplitude = 0.021f },
                Z = new NoiseSettings.NoiseParams { Frequency = 31.51f, Amplitude = 0.002f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 1.9f, Amplitude = 0.059f },
                Y = new NoiseSettings.NoiseParams { Frequency = 14.1f, Amplitude = 0.06f },
                Z = new NoiseSettings.NoiseParams { Frequency = 25.54f, Amplitude = 0.05f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 5.33f, Amplitude = 0.059f },
                Y = new NoiseSettings.NoiseParams { Frequency = 13.22f, Amplitude = 0.06f },
                Z = new NoiseSettings.NoiseParams { Frequency = 28.55f, Amplitude = 0.05f }
            }
            };

            profile.OrientationNoise = new NoiseSettings.TransformNoiseParams[]
            {
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 7.83f, Amplitude = 0.09f },
                Y = new NoiseSettings.NoiseParams { Frequency = 16.17f, Amplitude = 0.22f },
                Z = new NoiseSettings.NoiseParams { Frequency = 43.17f, Amplitude = 0.15f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 2.8f, Amplitude = 0.059f },
                Y = new NoiseSettings.NoiseParams { Frequency = 14.35f, Amplitude = 0.082f },
                Z = new NoiseSettings.NoiseParams { Frequency = 34.17f, Amplitude = 0.048f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 3.38f, Amplitude = 0.017f },
                Y = new NoiseSettings.NoiseParams { Frequency = 11.52f, Amplitude = 0.018f },
                Z = new NoiseSettings.NoiseParams { Frequency = 33.76f, Amplitude = 0.016f }
            }
            };

            return profile;
        }

        public static NoiseSettings Create6DShakeProfile()
        {
            NoiseSettings profile = ScriptableObject.CreateInstance<NoiseSettings>();
            profile.name = "6D Shake";

            profile.PositionNoise = new NoiseSettings.TransformNoiseParams[]
            {
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 3.2f, Amplitude = 0.011f },
                Y = new NoiseSettings.NoiseParams { Frequency = 7.7f, Amplitude = 0.009f },
                Z = new NoiseSettings.NoiseParams { Frequency = 51.51f, Amplitude = 0.002f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 1.9f, Amplitude = 0.059f },
                Y = new NoiseSettings.NoiseParams { Frequency = 9.1f, Amplitude = 0.04f },
                Z = new NoiseSettings.NoiseParams { Frequency = 55.54f, Amplitude = 0.05f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 3.33f, Amplitude = 0.059f },
                Y = new NoiseSettings.NoiseParams { Frequency = 9.22f, Amplitude = 0.04f },
                Z = new NoiseSettings.NoiseParams { Frequency = 58.55f, Amplitude = 0.05f }
            }
            };

            profile.OrientationNoise = new NoiseSettings.TransformNoiseParams[]
            {
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 5.83f, Amplitude = 0.09f },
                Y = new NoiseSettings.NoiseParams { Frequency = 9.17f, Amplitude = 0.14f },
                Z = new NoiseSettings.NoiseParams { Frequency = 57.17f, Amplitude = 0.15f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 1.8f, Amplitude = 0.059f },
                Y = new NoiseSettings.NoiseParams { Frequency = 11.35f, Amplitude = 0.041f },
                Z = new NoiseSettings.NoiseParams { Frequency = 54.17f, Amplitude = 0.048f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 2.38f, Amplitude = 0.017f },
                Y = new NoiseSettings.NoiseParams { Frequency = 10.52f, Amplitude = 0.009f },
                Z = new NoiseSettings.NoiseParams { Frequency = 63.76f, Amplitude = 0.016f }
            }
            };

            return profile;
        }

        public static NoiseSettings Create_Handheld_Normal_Extreme_Profile()
        {
            NoiseSettings profile = ScriptableObject.CreateInstance<NoiseSettings>();
            profile.name = "Handheld_normal_extreme";

            profile.OrientationNoise = new NoiseSettings.TransformNoiseParams[]
            {
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.2f, Amplitude = 15.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.9f, Amplitude = 5.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 2.0f, Amplitude = 2.0f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.25f, Amplitude = 7.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 1.0f, Amplitude = 3.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 1.0f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f }
            }
            };

            return profile;
        }

        public static NoiseSettings Create_Handheld_Normal_Mild_Profile()
        {
            NoiseSettings profile = ScriptableObject.CreateInstance<NoiseSettings>();
            profile.name = "Handheld_normal_mild";

            profile.OrientationNoise = new NoiseSettings.TransformNoiseParams[]
            {
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.15f, Amplitude = 7.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.8f, Amplitude = 4.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 1.2f, Amplitude = 1.0f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.1f, Amplitude = 5.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.75f, Amplitude = 2.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 1.5f, Amplitude = 0.8f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f }
            }
            };

            return profile;
        }

        public static NoiseSettings Create_Handheld_Normal_Strong_Profile()
        {
            NoiseSettings profile = ScriptableObject.CreateInstance<NoiseSettings>();
            profile.name = "Handheld_normal_strong";

            profile.OrientationNoise = new NoiseSettings.TransformNoiseParams[]
            {
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.4f, Amplitude = 10.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 1.44f, Amplitude = 5.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 2.49f, Amplitude = 3.0f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.06f, Amplitude = 10.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.73f, Amplitude = 3.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 2.0f, Amplitude = 1.0f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f }
            }
            };

            return profile;
        }

        public static NoiseSettings Create_Handheld_Tele_Mild_Profile()
        {
            NoiseSettings profile = ScriptableObject.CreateInstance<NoiseSettings>();
            profile.name = "Handheld_tele_mild";

            profile.OrientationNoise = new NoiseSettings.TransformNoiseParams[]
            {
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.2f, Amplitude = 4.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.4f, Amplitude = 2.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 0.7f, Amplitude = 1.0f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.15f, Amplitude = 2.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.5f, Amplitude = 2.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 0.6f, Amplitude = 1.0f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f }
            }
            };

            return profile;
        }

        public static NoiseSettings Create_Handheld_Tele_Strong_Profile()
        {
            NoiseSettings profile = ScriptableObject.CreateInstance<NoiseSettings>();
            profile.name = "Handheld_tele_strong";

            profile.OrientationNoise = new NoiseSettings.TransformNoiseParams[]
            {
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.39f, Amplitude = 6.19f },
                Y = new NoiseSettings.NoiseParams { Frequency = 1.75f, Amplitude = 1.84f },
                Z = new NoiseSettings.NoiseParams { Frequency = 2.0f, Amplitude = 2.3f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.15f, Amplitude = 4.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.9f, Amplitude = 0.5f },
                Z = new NoiseSettings.NoiseParams { Frequency = 1.4f, Amplitude = 0.5f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.1f, Amplitude = 1.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f }
            }
            };

            return profile;
        }

        public static NoiseSettings Create_Handheld_Wideangle_Mild_Profile()
        {
            NoiseSettings profile = ScriptableObject.CreateInstance<NoiseSettings>();
            profile.name = "Handheld_wideangle_mild";

            profile.OrientationNoise = new NoiseSettings.TransformNoiseParams[]
            {
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.15f, Amplitude = 12.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.6f, Amplitude = 5.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 1.5f, Amplitude = 1.0f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.1f, Amplitude = 5.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.45f, Amplitude = 4.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 1.2f, Amplitude = 1.0f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f }
            }
            };

            return profile;
        }

        public static NoiseSettings Create_Handheld_Wideangle_Strong_Profile()
        {
            NoiseSettings profile = ScriptableObject.CreateInstance<NoiseSettings>();
            profile.name = "Handheld_wideangle_strong";

            profile.OrientationNoise = new NoiseSettings.TransformNoiseParams[]
            {
                new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.5f, Amplitude = 17.46f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.94f, Amplitude = 12.47f },
                Z = new NoiseSettings.NoiseParams { Frequency = 1.2f, Amplitude = 4.0f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.25f, Amplitude = 5.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.5f, Amplitude = 4.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 1.3f, Amplitude = 2.0f }
            },
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 0.1f, Amplitude = 0.0f },
                Y = new NoiseSettings.NoiseParams { Frequency = 0.4f, Amplitude = 1.0f },
                Z = new NoiseSettings.NoiseParams { Frequency = 0.0f, Amplitude = 0.0f }
            }
            };

            return profile;
        }
    }
}
