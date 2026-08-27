using ReplayEditor;
using SmoothKeyframeCurves;

namespace ReplayFX.Utils
{
    public static class CurveUtil
    {
        public static FloatCurve playbackSpeedCurve = new FloatCurve();

        public static bool HasPlayBackKeys()
        {
            bool hasPlaybackKeys = false;
            if (playbackSpeedCurve.Keys.Count > 0)
            {
                hasPlaybackKeys = true;
                return hasPlaybackKeys;
            }
            return hasPlaybackKeys;
        }

        public static float EvaluatePlaybackSpeed(float time)
        {
            if (playbackSpeedCurve.Keys.Count == 0)
            {
                return 1.0f; // Default playback speed if no keyframes are present
            }
            return playbackSpeedCurve.Evaluate(time);
        }
        public static void Refresh()
        {
            //ReplayEditorController.Instance.cameraController.cameraCurve.Clear();
            ReplayEditorController.Instance.cameraController.cameraCurve.Refresh(ReplayEditorController.Instance.cameraController.keyFrames); // Refresh original curves
            ReplayEditorController.Instance.cameraController.keyframeUI.UpdateKeyframes(ReplayEditorController.Instance.cameraController.keyFrames);
        }
    }
}
