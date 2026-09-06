using GameManagement;
using ModIO.UI;
using ReplayFX;
using System.Reflection.Emit;
using UnityEngine.EventSystems;
using UnityEngine;
using ReplayFX.UI;

namespace ReplayFX
{
    public class ReplayFXSettingsState : GameState
    {
        public override void OnEnter(GameState prevState)
        {
            gameObject.SetActive(true);
            GameStateMachine.Instance.SemiTransparentLayer.SetActive(false);
            //Main.rfxSettings.SetCurrentCategory(PageBuilder.cameraSettings);
        }

        public override void OnExit(GameState nextState)
        {
            gameObject.SetActive(false);
            UISounds.Instance.PlayOneShotExit();
        }

        public override void OnUpdate()
        {
            CheckForInput();
        }

        private void CheckForInput()
        {
            GameState currentState = GameStateMachine.Instance.CurrentState;
            if (currentState == null || !(currentState is ReplayFXSettingsState))
                return;

            if (Main.inputListener.player.GetButtonDown("Start"))
            {
                RequestTransitionBack();
            }
            else if (Main.inputListener.player.GetButtonDown("B"))
            {
                RequestTransitionBack();
            }
        }
    }
}
