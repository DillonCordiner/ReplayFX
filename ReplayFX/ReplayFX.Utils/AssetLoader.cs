using GameManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;

namespace ReplayFX.Utils
{
    public static class AssetLoader
    {  
        public static AssetBundle assetBundle;

        //public static NoiseSettings[] noiseSettings = new NoiseSettings[8];
        public static List<NoiseSettings> noiseSettingsAssets = new List<NoiseSettings>();

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
            byte[] assetBundleData = ResourceExtractor.GetResources("ReplayFX.Resources.noiseassets");
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
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Handheld_normal_extreme"));
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Handheld_normal_mild"));
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Handheld_normal_strong"));
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Handheld_tele_mild"));
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Handheld_tele_strong"));
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Handheld_wideangle_mild"));
            noiseSettingsAssets.Add(assetBundle.LoadAsset<NoiseSettings>("Handheld_wideangle_strong"));

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
