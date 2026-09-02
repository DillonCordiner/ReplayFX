using ReplayEditor;
using SmoothKeyframeCurves;
using System.Security.Cryptography;
using UnityEngine;

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
            FloatCurve curve = playbackSpeedCurve;
            if (curve.Keys.Count == 0)
            {
                return 1.0f;
            }
            for (int i = 0; i < curve.Keys.Count - 1; i++)
            {
                CurveKey<float> leftKey = curve.Keys[i];
                CurveKey<float> rightKey = curve.Keys[i + 1];

                if (time >= leftKey.Time && time <= rightKey.Time)
                {
                    if (Mathf.Approximately(leftKey.Value, rightKey.Value))
                    {
                        return leftKey.Value;
                    }
                    break;
                }
            }
            return curve.Evaluate(time);
        }

        public static void Refresh()
        {
            //ReplayEditorController.Instance.cameraController.cameraCurve.Clear();
            ReplayEditorController.Instance.cameraController.cameraCurve.Refresh(ReplayEditorController.Instance.cameraController.keyFrames);
            ReplayEditorController.Instance.cameraController.keyframeUI.UpdateKeyframes(ReplayEditorController.Instance.cameraController.keyFrames);

        }
    }
}
