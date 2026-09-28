using HarmonyLib;
using ReplayEditor;
using ReplayFX.Keyframes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayFX.Patches
{
    // insert custom json when .replay file is written
    [HarmonyPatch(typeof(SaveManager), "SaveFile")]
    public static class SaveManager_SaveFile_Patch
    {
        [HarmonyPrefix]
        static void Prefix(string path)
        {
            if (!string.IsNullOrEmpty(path) && path.EndsWith(".replay", StringComparison.OrdinalIgnoreCase))
            {
                var camController = ReplayEditorController.Instance?.cameraController;
                if (camController != null && camController.keyFrames != null)
                {
                    CustomKeyFrameManager.Save(path, camController.keyFrames);
                }
            }
        }
    }
    [HarmonyPatch(typeof(SaveManager), "DeleteFile")]
    public static class SaveManager_DeleteFile_Patch
    {
        public static void Postfix(string filePath, ref Task<bool> __result)
        {
            Task<bool> originalTask = __result;
            __result = HandleDeleteFile(originalTask, filePath);
        }

        private static async Task<bool> HandleDeleteFile(Task<bool> originalTask, string filePath)
        {
            bool result = await originalTask;

            if (result)
            {
                CustomKeyFrameManager.DeleteFile(filePath);
                Main.Logger.Log("[DeleteFile] completed: " + filePath);
            }
            return result;
        }
    }
}
