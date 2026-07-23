using ReplayEditor;
using ReplayTestMod.Utils;
using SmoothKeyframeCurves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayTestMod.Utils
{
    public class CustomCameraCurve : CameraCurve
    {    
        public FloatCurve playbackSpeedCurve = new FloatCurve();

        public float EvaluatePlaybackSpeed(float time)
        {
            if (playbackSpeedCurve.Keys.Count == 0)
            {
                return 1.0f; // Default playback speed if no keyframes are present
            }
            return playbackSpeedCurve.Evaluate(time);
        }

        public void CustomRefresh(IEnumerable<KeyFrame> keyFrames)
        {
            ReplayEditorController.Instance.cameraController.cameraCurve.Refresh(keyFrames); // Refresh original curves
            playbackSpeedCurve.Clear();

            foreach (var keyFrame in keyFrames)
            {
                if (keyFrame is PlaybackSpeedKeyFrame speedKeyFrame)
                {
                    playbackSpeedCurve.InsertCurveKey(speedKeyFrame.playbackSpeed, speedKeyFrame.time);
                }
            }
        }
        
    }
}
