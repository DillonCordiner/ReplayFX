using UnityModManagerNet;

namespace ReplayFX.Utils
{
    public static class ModCheckUtil
    {
        //public static bool IsXXLModInstalled = false;
        //private static readonly string XXLmodID = "XXLMod3";

        public static bool CheckForMod(string modID)
        {
            UnityModManager.ModEntry ModEntry = UnityModManager.FindMod(modID);

            if (ModEntry != null && ModEntry.Info.Id == modID && ModEntry.Active)
            {
                Main.Logger.Log($"{modID} is Installed and Active");
                return true;
            }
            else
            {
                Main.Logger.Log($"{modID} Not Found");
                return false;
            }
        }

        /*
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
        */
    }
}
