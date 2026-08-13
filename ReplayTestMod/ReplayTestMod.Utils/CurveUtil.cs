using ReplayEditor;
using SmoothKeyframeCurves;

namespace ReplayTestMod.Utils
{
    public static class CurveUtil
    {
        public static FloatCurve playbackSpeedCurve = new FloatCurve();

        public static float EvaluatePlaybackSpeed(float time)
        {
            if (playbackSpeedCurve.Keys.Count == 0)
            {
                return 1.0f; // Default playback speed if no keyframes are present
            }
            return playbackSpeedCurve.Evaluate(time);
        }

        public static void CustomRefresh()
        {
            ReplayEditorController.Instance.cameraController.cameraCurve.Refresh(ReplayEditorController.Instance.cameraController.keyFrames); // Refresh original curves
            playbackSpeedCurve.Clear();
        }
    }
}
