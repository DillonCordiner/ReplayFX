using UnityEngine;
using ReplayTestMod.Utils;
using System.Collections;
using RapidGUI;

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
            if ((Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.R))
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
                    GUILayout.Label("Camera Profile");
                    Main.camNoiseController.targetProfile = RGUI.SelectionPopup(Main.camNoiseController.targetProfile, Main.camNoiseController.ProfileOptions);
                    GUILayout.Space(6f);
                    Main.settings.amplitude = RGUI.SliderFloat(Main.settings.amplitude, 0.0f, 10.0f, 1.0f, 82, "Amplitude");
                    GUILayout.Space(4f);
                    Main.settings.frequency = RGUI.SliderFloat(Main.settings.frequency, 0.0f, 10.0f, 1.0f, 82, "Frequency");
                    GUILayout.Space(4f);
                    UIextensions.FlexableButton("Generate new seed", Main.camNoiseController.GenerateNewSeed, Color.white);

                    GUILayout.Space(8f);

                    UIextensions.CenteredLabel("Pivot Offset");
                    GUILayout.Space(6f);
                    Main.settings.offset_x = RGUI.SliderFloat(Main.settings.offset_x, 0.0f, 10.0f, 0.0f, 72, "X Pivot");
                    GUILayout.Space(4f);
                    Main.settings.offset_y = RGUI.SliderFloat(Main.settings.offset_y, 0.0f, 10.0f, 0.0f, 72, "Y Pivot");
                    GUILayout.Space(4f);
                    Main.settings.offset_z = RGUI.SliderFloat(Main.settings.offset_z, 0.0f, 10.0f, 0.0f, 72, "Z Pivot");
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
                    UIextensions.CenteredLabel("Impluse KeyFrames");
                    GUILayout.Space(6f);
                    UIextensions.FlexableButton("Create Impluse KeyFrame", KeyFrameHelper.AddImpluseKeyFrame, Color.white);

                    GUILayout.Space(8f);

                    UIextensions.CenteredLabel("PlayBack KeyFrames");
                    GUILayout.Space(6f);
                    UIextensions.FlexableButton("Create PlayBack KeyFrame", KeyFrameHelper.AddPlayBackKeyFrame, Color.white);
                    GUILayout.Space(4f);
                    Main.settings.playBackSpeed = RGUI.SliderFloat(Main.settings.playBackSpeed, 0.0f, 2.0f, 1.0f, 72, "PlayBack Speed");
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
    }
}