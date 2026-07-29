using ReplayEditor;
using UnityEngine;
using ReplayTestMod.Utils;

namespace ReplayTestMod
{
    public class TimelineManager : MonoBehaviour
    {
        private float lastPlaybackTime = -1f;

        void Update()
        {
            ReplayEditorController replayEditor = ReplayEditorController.Instance;

            if (replayEditor == null || replayEditor.cameraController == null) 
                return;

            if (!replayEditor.cameraController.CamFollowKeyFrames)
            {
                lastPlaybackTime = replayEditor.PlaybackTime;
                return;
            }

            float currentTime = replayEditor.PlaybackTime;

            if (Mathf.Abs(currentTime - lastPlaybackTime) > 0.5f)
            {
                lastPlaybackTime = currentTime;
                return;
            }

            foreach (var keyframe in replayEditor.cameraController.keyFrames)
            {
                if (keyframe is ImpulseKeyFrame impulseKey)
                {
                    if (lastPlaybackTime <= impulseKey.time && currentTime > impulseKey.time)
                    {
                        impulseKey.TriggerImpulse();
                    }
                }
            }
            lastPlaybackTime = currentTime;
        }
    }
}