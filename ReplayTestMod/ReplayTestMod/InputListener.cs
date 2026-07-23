using System;
using UnityModManagerNet;
using UnityEngine;
using Rewired;
using System.Linq;

namespace ReplayTestMod
{
    public class InputListener : MonoBehaviour
    {
        public bool changeHotKey = false;

        private readonly KeyCode[] keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().Where(k => ((int)k < (int)KeyCode.Mouse0)).ToArray();

        public KeyCode? GetCurrentKeyDown()
        {
            if (!Input.anyKeyDown)
            {
                return null;
            }

            // skips alt and ctrl keys
            for (int i = 0; i < keyCodes.Length; i++)
            {
                KeyCode keyCode = keyCodes[i];

                if (keyCode == KeyCode.LeftControl ||
                    keyCode == KeyCode.RightControl ||
                    keyCode == KeyCode.LeftAlt ||
                    keyCode == KeyCode.RightAlt ||
                    keyCode == KeyCode.AltGr ||
                    keyCode == KeyCode.LeftCommand ||
                    keyCode == KeyCode.RightCommand)
                {
                    continue;
                }

                if (Input.GetKey(keyCode))
                {
                    return keyCode;
                }
            }

            return null;
        }

        public void Update()
        {
            if (Input.GetKeyDown(Main.settings.noiseHotkey.keyCode))
            {
                Main.camNoiseController.ToggleNoise(!Main.settings.IsNoiseEnabled);
            }
            else if (PlayerController.Instance.inputController.player.GetButtonDown(0)) // A button
            {
                Main.camNoiseController.ToggleNoise(!Main.settings.IsNoiseEnabled);
            }
        }
    }
}