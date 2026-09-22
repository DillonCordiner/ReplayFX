using GameManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

namespace ReplayFX.Utils
{
    public static class AssetLoader
    {
        public static AssetBundle assetBundle;
        public static List<NoiseSettings> noiseSettingsAssets = new List<NoiseSettings>();

        private static readonly string[] NoiseSettingNames =
        {
            "Shake",
            "Normal_extreme",
            "Normal_mild",
            "Normal_strong",
            "Tele_mild",
            "Tele_strong",
            "Wideangle_mild",
            "Wideangle_strong"
        };

        private static bool _loading;

        public static void LoadBundles()
        {
            if (_loading || assetBundle != null) return;

            Type unityObjectType = Type.GetType("UnityEngine.Object, UnityEngine");

            if (unityObjectType != null && GameStateMachine.Instance != null)
            {
                _loading = true;
                GameStateMachine.Instance.StartCoroutine(LoadAssetBundle());
            }
            else
            {
                Main.Logger.Log("[AssetLoader] Unable to start LoadAssetBundle Routine");
            }
        }

        private static IEnumerator LoadAssetBundle()
        {
            try
            {
                Task<byte[]> readTask = Task.Run(() => GetResources("ReplayFX.Resources.noiseassets"));
                yield return new WaitUntil(() => readTask.IsCompleted);

                byte[] assetBundleData = readTask.Result;
                if (assetBundleData == null)
                {
                    Main.Logger.Log("[AssetLoader] Failed to extract ReplayFX Asset Bundle");
                    yield break;
                }

                AssetBundleCreateRequest abCreateRequest = AssetBundle.LoadFromMemoryAsync(assetBundleData);
                yield return abCreateRequest;

                assetBundle = abCreateRequest.assetBundle;
                if (assetBundle == null)
                {
                    Main.Logger.Log("[AssetLoader] Failed to load ReplayFX Asset Bundle Request");
                    yield break;
                }

                yield return GameStateMachine.Instance.StartCoroutine(LoadAssetFromBundle());
            }
            finally
            {
                _loading = false;
            }
        }

        private static IEnumerator LoadAssetFromBundle()
        {
            AssetBundleRequest request = assetBundle.LoadAllAssetsAsync<NoiseSettings>();
            yield return request;

            Dictionary<string, NoiseSettings> noiseName = request.allAssets.Cast<NoiseSettings>().Where(a => a != null).ToDictionary(a => a.name, a => a);

            noiseSettingsAssets.Clear();
            foreach (string name in NoiseSettingNames)
            {
                if (noiseName.TryGetValue(name, out NoiseSettings asset))
                {
                    noiseSettingsAssets.Add(asset);
                }
                else
                {
                    Main.Logger.Log($"[AssetLoader] Missing noise asset: {name}");
                }
            }
        }
        public static byte[] GetResources(string filename)
        {
            using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename))
            {
                if (manifestResourceStream == null) return null;

                using (MemoryStream memoryStream = new MemoryStream((int)manifestResourceStream.Length))
                {
                    manifestResourceStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
        public static void UnloadAssetBundle()
        {
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
                assetBundle = null;
            }
            noiseSettingsAssets.Clear();
        }
    }
}