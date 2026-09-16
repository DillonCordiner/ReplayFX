using System.Linq;
using Rewired;
using TMPro;
using UnityEngine;

namespace ReplayFX.UI
{
    public class AdaptiveLabels : MonoBehaviour
    {
        public string mainLabelText;
        public string xboxText;
        public string psText;

        public bool isModeLabel;

        private TextMeshProUGUI mainLabel;
        private TextMeshProUGUI modeLabel;
        private TextMeshProUGUI buttonLabel;

        private string lastMainLabel;
        private string lastModeLabel;
        private string lastButtonLabel;
        private float lastPlaybackSpeed;

        private bool IsControllerPlaystation()
        {
            Joystick joystick = ReInput.players.GetPlayer(0).controllers.Joysticks.FirstOrDefault();
            string name = joystick?.name ?? "unknown";
            string lowerName = name.ToLower();

            return lowerName.Contains("dual") || lowerName.Contains("sony") || lowerName.Contains("dualshock") || lowerName.Contains("dual shock");
        }
        private void Awake()
        {
            GetLabels();
        }
        private void LateUpdate()
        {
            if (mainLabel != null && !string.IsNullOrEmpty(mainLabelText))
            {
                if (lastMainLabel != mainLabel.text)
                {
                    mainLabel.text = mainLabelText;
                    lastMainLabel = mainLabel.text;
                }
            }
            if (modeLabel != null && isModeLabel)
            {
                if (lastPlaybackSpeed != Main.settings.replay_playback_speed || lastModeLabel != modeLabel.text)
                {
                    //float convertedValue = Main.settings.replay_playback_speed * 100f;
                    //modeLabel.text = convertedValue.ToString();
                    modeLabel.text = Main.settings.replay_playback_speed.ToString("P0"); // converts to 100% format
                    lastPlaybackSpeed = Main.settings.replay_playback_speed;
                    lastModeLabel = modeLabel.text;
                }
            }
            if (buttonLabel != null)
            {
                if (lastButtonLabel != buttonLabel.text)
                {
                    buttonLabel.text = IsControllerPlaystation() ? psText : xboxText;
                    lastButtonLabel = buttonLabel.text;
                }
            }
        }
        private void LateUpdate2()
        {
            if (mainLabel != null && !string.IsNullOrEmpty(mainLabelText))
            {
                if (lastMainLabel != mainLabel.text)
                {
                    mainLabel.text = mainLabelText;
                    lastMainLabel = mainLabel.text;
                }
            }
            if (buttonLabel != null)
            {
                if (isModeLabel)
                {
                    if (lastPlaybackSpeed  != Main.settings.replay_playback_speed || lastModeLabel != buttonLabel.text)
                    {
                        float convertedValue = Main.settings.replay_playback_speed * 100;
                        buttonLabel.text = convertedValue.ToString() + "%";
                        lastPlaybackSpeed = Main.settings.replay_playback_speed;
                        lastModeLabel = buttonLabel.text;
                    }
                }
                else
                {
                    if (lastButtonLabel != buttonLabel.text)
                    {
                        buttonLabel.text = IsControllerPlaystation() ? psText : xboxText;
                        lastButtonLabel = buttonLabel.text;
                    }
                }
            }
        }
        private void GetLabels()
        {
            TextMeshProUGUI[] labelList = GetComponentsInChildren<TextMeshProUGUI>();

            foreach (TextMeshProUGUI label in labelList)
            {
                string name = label.name;

                if (name == "Action Label" || name == "TextMeshPro Text")
                {
                    mainLabel = label;
                }
                else if (name == "Button Label")
                {
                    buttonLabel = label;
                }
                else if (name == "Mode Label")
                {
                    modeLabel = label;
                    isModeLabel = true;
                }
            }
        }
        public void Setup(string mainText, string xboxLabel, string psLabel)
        {
            mainLabelText = mainText;
            xboxText = xboxLabel;
            psText = psLabel;
        }
    }
}