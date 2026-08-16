using UnityEngine;
using ReplayTestMod.Utils;
using System.Collections;
using RapidGUI;
using ReplayTestMod.Keyframes;

namespace ReplayTestMod
{
    public class UItab
    {
        public bool isClosed;
        public string text;
        public int font;

        public UItab(bool isClosed, string text, int font)
        {
            this.isClosed = isClosed;
            this.text = text;
            this.font = font;
        }
    }

    public class UIController : MonoBehaviour
    {
        private Color BGColor = new Color(0.85f, 0.90f, 1.0f);

        public bool showUI;
        private Rect MainWindowRect = new Rect(20, 20, Screen.width / 5, 20);

        //readonly UItab Test_Tab = new UItab(true, "Test Stuff", 14);
        readonly UItab Camera_Tab = new UItab(true, "Camera", 14);
        readonly UItab KeyFrame_Tab = new UItab(true, "KeyFrames", 14);

        private void Tabs(UItab obj, string color = "#e6ebe8")
        {
            if (GUILayout.Button($"<size={obj.font}><color={color}>" + (obj.isClosed ? "○" : "●") + obj.text + "</color>" + "</size>", "Label"))
            {
                obj.isClosed = !obj.isClosed;
                MainWindowRect.height = 20;
                MainWindowRect.width = Screen.width / 5;
                UIextensions.TabFontSwitch(obj);
            }
        }

        private void Start()
        {
            StartCoroutine(WaitForInput());
        }
        private void OnDestroy()
        {
            StopCoroutine(WaitForInput());
        }
        private IEnumerator WaitForInput()
        {
            while (!Main.enabled)
            {
                yield return null;
            }

            while (true)
            {
                yield return new WaitForEndOfFrame();
                InputSwitch();
                yield return null;
            }
        }

        private void InputSwitch()
        {
            if ((Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(Main.settings.noiseHotkey.keyCode))
            {
                ToggleUI();
            }
        }

        private void ToggleUI()
        {
            if (!showUI)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
        private void Open()
        {
            showUI = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Close()
        {
            showUI = false;
            Cursor.visible = false;
            Main.settings.Save(Main.modEntry);
        }

        private void OnGUI()
        {
            if (!showUI)
                return;

            GUI.backgroundColor = BGColor;
            MainWindowRect = GUILayout.Window(42879, MainWindowRect, MainWindow, "<b> ReplayTestMod </b>");
        }

        // Creates the GUI window
        private void MainWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            MainUI();
            CameraUI();
            KeyFrameUI();
        }
        private void MainUI()
        {
            GUILayout.Label($"ReplayTestMod");
            GUILayout.Space(8f);
        }
        /*
        private void CameraShakeButton()
        {
            Main.settings.enableNoise = !Main.settings.enableNoise;
            Main.camNoiseController.ToggleNoise();
        }
        */
        
        private void SetPlayBackSpeedButton()
        {
            if (ModCheckUtil.IsXXLModInstalled)
            {
                XXLModExtention.SetXXLSpeed(Main.settings.replayplayback_speed);
            }
            else
            {
                PlayBackUtil.SetPlayBackSpeedValue(Main.settings.replayplayback_speed);
            }
        }
        private void CameraUI()
        {
            Tabs(Camera_Tab, UIextensions.TabColorSwitch(Camera_Tab));
            if (Camera_Tab.isClosed)
                return;

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("Camera Shake");
                    UIextensions.FlexableButton(Main.settings.enableNoise ? "<b> Enabled </b>" : "<b><color=#171717> Disabled </color></b>", Main.camNoiseController.ToggleNoise, Color.white);

                    GUILayout.Space(6f);
                    GUILayout.Label("Camera Profile");
                    if (Main.settings.enableNoise)
                    {
                        Main.camNoiseController.targetProfile = RGUI.SelectionPopup(Main.camNoiseController.targetProfile, Main.camNoiseController.ProfileOptions);
                    }
                    else
                    {
                        GUILayout.Label("<b><color=#171717> Disabled </color></b>");
                    }
                    GUILayout.Space(6f);
                    Main.settings.noise_amplitude = RGUI.SliderFloat(Main.settings.noise_amplitude, 0.0f, 10.0f, 1.0f, 82, "Amplitude");
                    GUILayout.Space(4f);
                    Main.settings.noise_frequency = RGUI.SliderFloat(Main.settings.noise_frequency, 0.0f, 10.0f, 1.0f, 82, "Frequency");
                    GUILayout.Space(4f);
                    UIextensions.FlexableButton("Generate new seed", Main.camNoiseController.GenerateNewSeed, Color.white);

                    GUILayout.Space(8f);

                    UIextensions.CenteredLabel("Pivot Offset");
                    GUILayout.Space(6f);
                    Main.settings.noise_offset_x = RGUI.SliderFloat(Main.settings.noise_offset_x, 0.0f, 10.0f, 0.0f, 72, "X Pivot");
                    GUILayout.Space(4f);
                    Main.settings.noise_offset_y = RGUI.SliderFloat(Main.settings.noise_offset_y, 0.0f, 10.0f, 0.0f, 72, "Y Pivot");
                    GUILayout.Space(4f);
                    Main.settings.noise_offset_z = RGUI.SliderFloat(Main.settings.noise_offset_z, 0.0f, 10.0f, 0.0f, 72, "Z Pivot");
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
        private void KeyFrameUI()
        {
            Tabs(KeyFrame_Tab, UIextensions.TabColorSwitch(KeyFrame_Tab));
            if (KeyFrame_Tab.isClosed)
                return;

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(10f);
                    UIextensions.FlexableButton("Refresh Timeline", CurveUtil.Refresh, Color.white);
                    GUILayout.Space(8f);

                    GUILayout.BeginVertical("Box");
                    {
                        UIextensions.CenteredLabel("Impluse KeyFrames");
                        GUILayout.Space(6f);
                        UIextensions.FlexableButton("Create Impluse KeyFrame", KeyFrameHelper.AddImpluseKeyFrame, Color.white);
                        GUILayout.Space(4f);
                        Main.settings.impulse_force = RGUI.SliderFloat(Main.settings.impulse_force, 0.0f, 10.0f, 1.0f, 90, "Impulse Force");
                        GUILayout.Space(4f);
                        Main.settings.impulse_listener_gain = RGUI.SliderFloat(Main.settings.impulse_listener_gain, 0.0f, 10.0f, 2.0f, 90, "Gain");
                        GUILayout.Space(4f);
                        Main.settings.impulse_source_amplitude = RGUI.SliderFloat(Main.settings.impulse_source_amplitude, 0.0f, 10.0f, 2.0f, 90, "Amplitude");
                        GUILayout.Space(4f);
                        Main.settings.impulse_source_frequency = RGUI.SliderFloat(Main.settings.impulse_source_frequency, 0.0f, 10.0f, 1.0f, 90, "Frequency");
                        GUILayout.Space(4f);
                        Main.settings.impulse_source_decaytime = RGUI.SliderFloat(Main.settings.impulse_source_decaytime, 0.0f, 2.0f, 0.5f, 90, "Decay");
                        GUILayout.Space(8f);
                        UIextensions.FlexableButton("Test Impulse", Main.camNoiseController.GenerateImpluse, Color.white);
                        GUILayout.Space(4f);
                        UIextensions.FlexableButton("Delete All Keys", KeyFrameHelper.RemoveAllImpulseKeys, Color.white);
                    }
                    GUILayout.EndVertical();

                    GUILayout.Space(8f);

                    GUILayout.BeginVertical("Box");
                    {
                        UIextensions.CenteredLabel("PlayBack KeyFrames");
                        GUILayout.Space(6f);
                        UIextensions.FlexableButton("Create PlayBack KeyFrame", KeyFrameHelper.AddPlayBackKeyFrame, Color.white);
                        GUILayout.Space(4f);
                        Main.settings.replayplayback_speed = RGUI.SliderFloat(Main.settings.replayplayback_speed, 0.0f, 2.0f, 1.0f, 92, "Replay Speed");
                        GUILayout.Space(8f);
                        UIextensions.FlexableButton("Set Speed to Slider Value", SetPlayBackSpeedButton, Color.white);
                        GUILayout.Space(4f);
                        UIextensions.FlexableButton("Delete All Keys", KeyFrameHelper.RemoveAllPlaybackKeys, Color.white);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
    }
}