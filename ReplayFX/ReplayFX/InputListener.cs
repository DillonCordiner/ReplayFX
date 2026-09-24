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
using ReplayFX.UI;

namespace ReplayFX
{
    public class InputListener : MonoBehaviour
    {
        public Player player { get; private set; }

        private bool playerFound = false;

        public bool changeHotKey = false;

        public bool isBumperPressed = false;
        private float holdDelayTimer = 0f;
        private float InitialHoldDelay = 0.25f;

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
        
        private void Update() 
        {
            playerFound = player != null;
            if (!playerFound)
                return;

            if (Main.replayfxMenu.clonedMenu == null || !Main.replayfxMenu.clonedMenu.gameObject.activeSelf)
            return;

            if (player.GetButtonDown(7))
            {
                Main.replayfxMenu.NextCategory();
            }
            if (player.GetButtonDown(6))
            {
                Main.replayfxMenu.PreviousCategory();
            }
        }
        
        private void LateUpdate()
        {
            playerFound = player != null;
            if (!playerFound)
                return;

            GameState currentState = GameStateMachine.Instance.CurrentState;

            if (currentState is ReplayState)
            {
                bool isControlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                if (isControlPressed && Input.GetKeyDown(Main.settings.noiseHotkey.keyCode))
                {
                    Main.camController.ToggleNoise();
                }

                if (player.GetButton("RB") && player.GetButtonDown("A"))
                {
                    KeyFrameHelper.AddPlayBackKeyFrame();
                }
                else if (player.GetButton("RB") && player.GetButtonDown("Y"))
                {
                    KeyFrameHelper.AddImpluseKeyFrame();
                }
                else if (player.GetButton("RB") && !Main.replayfxMenu.clonedMenu.gameObject.activeSelf)
                {
                    float speedChangeRate = 0.5f;

                    if (player.GetButtonDown(67))
                    {
                        Main.settings.replay_playback_speed += 0.01f;
                        holdDelayTimer = 0f;
                    }
                    else if (player.GetButton(67))
                    {
                        holdDelayTimer += Time.unscaledDeltaTime;
                        if (holdDelayTimer > InitialHoldDelay)
                        {
                            Main.settings.replay_playback_speed += speedChangeRate * Time.unscaledDeltaTime;
                        }
                    }
                    else if (player.GetButtonDown(68))
                    {
                        Main.settings.replay_playback_speed -= 0.01f;
                        holdDelayTimer = 0f;
                    }
                    else if (player.GetButton(68))
                    {
                        holdDelayTimer += Time.unscaledDeltaTime;
                        if (holdDelayTimer > InitialHoldDelay)
                        {
                            Main.settings.replay_playback_speed -= speedChangeRate * Time.unscaledDeltaTime;
                        }
                    }
                    else
                    {
                        holdDelayTimer = 0f;
                    }

                    Main.settings.replay_playback_speed = Mathf.Clamp(Main.settings.replay_playback_speed, 0.0f, 2.0f);
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