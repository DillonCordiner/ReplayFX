using HarmonyLib;
using ReplayEditor;
using SmoothKeyframeCurves;

namespace ReplayFX.Utils
{
    public static class PlayBackUtil
    {
        public static void SetPlayBackSpeedValue(float speed)
        {
            Traverse.Create(ReplayEditorController.Instance).Field("playbackSpeed").SetValue(speed);
        }
    }
}
