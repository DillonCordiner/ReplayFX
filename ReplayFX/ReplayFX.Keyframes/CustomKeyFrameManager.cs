using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ModIO;
using Newtonsoft.Json;
using ReplayEditor;
using ReplayFX.Utils;
using UnityEngine;

namespace ReplayFX.Keyframes
{
    
    [Serializable]
    public class CustomKeyFrameData
    {
        public string type;   // "PlaybackSpeed" or "Impulse"
        public float time;

        // PlaybackSpeed
        public float targetSpeed;

        // Impulse
        public float force;
        public float amplitude;
        public float frequency;
        public float decay;
    }

    [Serializable]
    public class CustomKeyFrameFile
    {
        public int version = 1;
        public List<CustomKeyFrameData> keyFrames = new List<CustomKeyFrameData>();
    }

    public static class CustomKeyFrameManager
    {
        public static string GetSidecarPath(string replayFilePath)
        {
            return Path.ChangeExtension(replayFilePath, ".replayfx.json");
        }

        public static void Save(string replayFilePath, IEnumerable<KeyFrame> customKeyFrames)
        {
            if (string.IsNullOrEmpty(replayFilePath) || customKeyFrames == null) return;

            var file = new CustomKeyFrameFile();

            foreach (KeyFrame k in customKeyFrames)
            {
                if (k is PlaybackSpeedKeyFrame speed)
                {
                    file.keyFrames.Add(new CustomKeyFrameData
                    {
                        type = "PlaybackSpeed",
                        time = speed.time,
                        targetSpeed = speed.targetSpeed
                    });
                }
                else if (k is ImpulseKeyFrame impulse)
                {
                    file.keyFrames.Add(new CustomKeyFrameData
                    {
                        type = "Impulse",
                        time = impulse.time,
                        force = impulse.force,
                        amplitude = impulse.amplitude,
                        frequency = impulse.frequency,
                        decay = impulse.decay
                    });
                }
            }

            try
            {
                string jsonPath = GetSidecarPath(replayFilePath);

                // Use Newtonsoft.Json instead of JsonUtility
                string jsonContent = JsonConvert.SerializeObject(file, Newtonsoft.Json.Formatting.Indented);

                File.WriteAllText(jsonPath, jsonContent);
                Main.Logger.Log($"[ReplayFX] Saved {file.keyFrames.Count} custom keyframe(s) to sidecar: {jsonPath}");
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"[ReplayFX] Failed to save sidecar JSON: {ex.Message}");
            }
        }
        public static void DeleteFile(string replayFilePath) 
        {
            string jsonPath = GetSidecarPath(replayFilePath);

            if (string.IsNullOrEmpty(jsonPath)) return;

            if (!File.Exists(jsonPath))
            {
                Main.Logger.Log("Can't delete File at path: " + jsonPath + ": File doesn't exist!");
            }
            try
            {
                File.Delete(jsonPath);
                Main.Logger.Log("Successfully deleted File at " + jsonPath);
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Failed to delete File at path " + jsonPath + ": " + ex.Message);
            }
        }
        public static List<KeyFrame> Load(string replayFilePath)
        {
            var result = new List<KeyFrame>();
            string jsonPath = GetSidecarPath(replayFilePath);

            if (!File.Exists(jsonPath))
            {
                return result; // Standard replay without custom FX
            }

            try
            {
                string jsonContent = File.ReadAllText(jsonPath);

                // Use Newtonsoft.Json instead of JsonUtility
                var file = JsonConvert.DeserializeObject<CustomKeyFrameFile>(jsonContent);

                if (file?.keyFrames != null)
                {
                    foreach (CustomKeyFrameData d in file.keyFrames)
                    {
                        if (d.type == "PlaybackSpeed")
                        {
                            result.Add(new PlaybackSpeedKeyFrame(d.targetSpeed, d.time));
                        }
                        else if (d.type == "Impulse")
                        {
                            result.Add(new ImpulseKeyFrame(null, d.time, d.force, d.amplitude, d.frequency, d.decay));
                        }
                    }
                }

                Main.Logger.Log($"[ReplayFX] Loaded {result.Count} custom keyframe(s) from sidecar: {jsonPath}");
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"[ReplayFX] Failed to load sidecar JSON: {ex.Message}");
            }

            return result;
        }
        public static List<CustomKeyFrameData> LoadRawData(string replayFilePath)
        {
            string jsonPath = GetSidecarPath(replayFilePath);

            if (!File.Exists(jsonPath))
            {
                return null;
            }

            try
            {
                string jsonContent = File.ReadAllText(jsonPath);
                var file = JsonConvert.DeserializeObject<CustomKeyFrameFile>(jsonContent);
                return file?.keyFrames;
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"[ReplayFX] Failed to load sidecar JSON: {ex.Message}");
                return null;
            }
        }
    }
    
}