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

        //public static NoiseSettings _6DShake;
        //public static NoiseSettings _handheld_normal_extreme;
        //public static NoiseSettings _handheld_normal_mild;
        //public static NoiseSettings _handheld_normal_strong;
        //public static NoiseSettings _handheld_tele_mild;
        //public static NoiseSettings _handheld_tele_strong;
        //public static NoiseSettings _handheld_windangle_mild;
        //public static NoiseSettings _handheld_wideangle_strong;

        public static NoiseSettings[] noiseSettings = new NoiseSettings[8];

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
            byte[] assetBundleData = ResourceExtractor.GetResources("ReplayTestMod.Resources.noiseassets");
            if (assetBundleData == null)
            {
                Main.Logger.Log("Failed to extract ReplayTestMod Asset Bundle");
                yield break;
            }
            AssetBundleCreateRequest abCreateRequest = AssetBundle.LoadFromMemoryAsync(assetBundleData);
            yield return abCreateRequest;

            assetBundle = abCreateRequest.assetBundle;
            if (assetBundle == null)
            {
                Main.Logger.Log("Failed to load ReplayTestMod Asset Bundle Request");
                yield break;
            }
            yield return GameStateMachine.Instance.StartCoroutine(LoadAssetFromBundle());
        }
        private static IEnumerator LoadAssetFromBundle()
        {
            //_6DShake = assetBundle.LoadAsset<NoiseSettings>("6D Shake");
            //_handheld_normal_extreme = assetBundle.LoadAsset<NoiseSettings>("Handheld_normal_extreme");
            //_handheld_normal_mild = assetBundle.LoadAsset<NoiseSettings>("Handheld_normal_mild");
            //_handheld_normal_strong = assetBundle.LoadAsset<NoiseSettings>("Handheld_normal_strong");
            //_handheld_tele_mild = assetBundle.LoadAsset<NoiseSettings>("Handheld_tele_mild");
            //_handheld_tele_strong = assetBundle.LoadAsset<NoiseSettings>("Handheld_tele_strong");
            //_handheld_windangle_mild = assetBundle.LoadAsset<NoiseSettings>("Handheld_wideangle_mild");
            //_handheld_wideangle_strong = assetBundle.LoadAsset<NoiseSettings>("Handheld_wideangle_strong");

            noiseSettings[0] = assetBundle.LoadAsset<NoiseSettings>("6D Shake");
            noiseSettings[1] = assetBundle.LoadAsset<NoiseSettings>("Handheld_normal_extreme");
            noiseSettings[2] = assetBundle.LoadAsset<NoiseSettings>("Handheld_normal_mild");
            noiseSettings[3] = assetBundle.LoadAsset<NoiseSettings>("Handheld_normal_strong");
            noiseSettings[4] = assetBundle.LoadAsset<NoiseSettings>("Handheld_tele_mild");
            noiseSettings[5] = assetBundle.LoadAsset<NoiseSettings>("Handheld_tele_strong");
            noiseSettings[6] = assetBundle.LoadAsset<NoiseSettings>("Handheld_wideangle_mild");
            noiseSettings[7] = assetBundle.LoadAsset<NoiseSettings>("Handheld_wideangle_strong");

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
