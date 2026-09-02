using UnityModManagerNet;

namespace ReplayFX.Utils
{
    public static class ModCheckUtil
    {
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
    }
}
