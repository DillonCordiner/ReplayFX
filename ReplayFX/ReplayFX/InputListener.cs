using System;
using UnityModManagerNet;
using UnityEngine;
using Rewired;
using System.Linq;
using GameManagement;
using MapEditor;
using ReplayFX.Keyframes;
using Rewired.Integration.UnityUI;
using UnityEngine.EventSystems;

namespace ReplayFX
{
    public class InputListener : MonoBehaviour
    {
        public Player player { get; private set; }

        private bool playerFound = false;

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

        private void Start()
        {
            //Player player = RewiredInput.PrimaryPlayer; // 1.2.2.8 player
            //Player player = ReInput.players.AllPlayers.FirstOrDefault();
            player = ReInput.players.GetPlayer(0);
        }

        private void LateUpdate()
        {

            playerFound = player != null;
            if (!playerFound)
                return;

            GameState currentState = GameStateMachine.Instance.CurrentState;

            if ((currentState is ReplayState) || (currentState is PlayState))
            {
                bool isControlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                if (!isControlPressed && Input.GetKeyDown(Main.settings.noiseHotkey.keyCode))
                {
                    Main.camNoiseController.ToggleNoise();
                }
            }
            if (currentState is ReplayState)
            {
                if (player.GetButton("LB") && player.GetButtonDown("A"))
                {
                    Main.camNoiseController.ToggleNoise();
                }
                else if (player.GetButton("LB") && player.GetButtonDown("Y"))
                {
                    KeyFrameHelper.AddPlayBackKeyFrame();
                }
                else if (player.GetButton("RB") && player.GetButtonDown("Y"))
                {
                    KeyFrameHelper.AddImpluseKeyFrame();
                }
            }
        }
        /*
        public void Update()
        {
            GameState currentState = GameStateMachine.Instance.CurrentState;

            if ((currentState is ReplayState) || (currentState is PlayState))
            {
                bool isControlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                if (!isControlPressed && Input.GetKeyDown(Main.settings.noiseHotkey.keyCode))
                {
                    Main.camNoiseController.ToggleNoise();
                }
            }
            if ((currentState is ReplayState))
            {
                if (RewiredInput.PrimaryPlayer.GetButton("LB") && RewiredInput.PrimaryPlayer.GetButtonDown("A"))
                {
                    Main.camNoiseController.ToggleNoise();
                }
                else if (RewiredInput.PrimaryPlayer.GetButton("LB") && RewiredInput.PrimaryPlayer.GetButtonDown("Y"))
                {
                    KeyFrameHelper.AddPlayBackKeyFrame();
                }
                else if (RewiredInput.PrimaryPlayer.GetButton("RB") && RewiredInput.PrimaryPlayer.GetButtonDown("Y"))
                {
                    KeyFrameHelper.AddImpluseKeyFrame();
                }
            }
        }
        */
    }
}