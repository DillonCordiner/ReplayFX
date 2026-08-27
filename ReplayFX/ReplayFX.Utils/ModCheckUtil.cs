using UnityModManagerNet;

namespace ReplayFX.Utils
{
    public static class ModCheckUtil
    {
        public static bool IsXXLModInstalled = false;
        private static readonly string XXLmodID = "XXLMod3";
        public static void CheckForXXLMod()
        {
            UnityModManager.ModEntry ModEntry = UnityModManager.FindMod(XXLmodID);

            if (ModEntry != null && ModEntry.Info.Id == XXLmodID && ModEntry.Active)
            {
                IsXXLModInstalled = true;
                Main.Logger.Log("XXL Mod is Installed and Active");
            }
            else
            {
                IsXXLModInstalled = false;
                Main.Logger.Log("XXL Mod NOT Found");
            }
        }
    }
}
