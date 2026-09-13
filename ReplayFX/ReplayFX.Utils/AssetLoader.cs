using GameManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using ModIO.UI;
using System.Reflection;
using System.IO;

namespace ReplayFX.Utils
{
    public static class AssetLoader
    {  
        public static AssetBundle assetBundle;

        //public static NoiseSettings[] noiseSettings = new NoiseSettings[8];
        public static List<NoiseSettings> noiseSettingsAssets = new List<NoiseSettings>();
        public static byte[] GetResources(string filename)
        {
            using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename))
            {
                if (manifestResourceStream == null)
                    return null;

                byte[] buffer = new byte[manifestResourceStream.Length];
                manifestResourceStream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
        public static void LoadBundles()
        {
            // Check if a type from the Unity assembly has been loaded
            Type unityObjectType = Type.GetType("UnityEngine.Object, UnityEngine");

            if (unityObjectType != null && GameStateMachine.Instance != null)
            {
                GameStateMachine.Instance.StartCoroutine(LoadAssetBundle());
            }
            else
            {
                Main.Logger.Log("Unable to start LoadAssetBundle Routine");
            }
        }
        private static IEnumerator LoadAssetBundle()
        {
            byte[] assetBundleData = GetResources("ReplayFX.Resources.noiseassets");
            if (assetBundleData == null)
            {
                Main.Logger.Log("Failed to extract ReplayFX Asset Bundle");
                yield break;
            }
            AssetBundleCreateRequest abCreateRequest = AssetBundle.LoadFromMemoryAsync(assetBundleData);
            yield return abCreateRequest;

            assetBundle = abCreateRequest.assetBundle;
            if (assetBundle == null)
            {
                Main.Logger.Log("Failed to load ReplayFX Asset Bundle Request");
                yield break;
            }
            yield return GameStateMachine.Instance.StartCoroutine(LoadAssetFromBundle());
        }
        private static IEnumerator LoadAssetFromBundle()
        {
            /*
            noiseSettings[0] = assetBundle.LoadAsset<NoiseSettings>("Shake");
            noiseSettings[1] = assetBundle.LoadAsset<NoiseSettings>("Handheld_normal_extreme");
            noiseSettings[2] = assetBundle.LoadAsset<NoiseSettings>("Handheld_normal_mild");
            noiseSettings[3] = assetBundle.LoadAsset<NoiseSettings>("Handheld_normal_strong");
            noiseSettings[4] = assetBundle.LoadAsset<NoiseSettings>("Handheld_tele_mild");
            noiseSettings[5] = assetBundle.LoadAsset<NoiseSettings>("Handheld_tele_strong");
            noiseSettings[6] = assetBundle.LoadAsset<NoiseSettings>("Handheld_wideangle_mild");
            noiseSettings[7] = assetBundle.LoadAsset<NoiseSettings>("Handheld_wideangle_strong");
            */

            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Shake"));
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Normal_extreme"));
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Normal_mild"));
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Normal_strong"));
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Tele_mild"));
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Tele_strong"));
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Wideangle_mild"));
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Wideangle_strong"));

            yield return null;
        }
        public static void UnloadAssetBundle()
        {
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
                assetBundle = null;
            }
        }
        private static void OnDestroy()
        {
            UnloadAssetBundle();
        }
        
    }
}
