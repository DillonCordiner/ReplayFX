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
            base.gameObject.SetActive(true);
            GameStateMachine.Instance.SemiTransparentLayer.SetActive(false);
            GameStateMachine.Instance.PauseObject.SetActive(true);
            //Main.rfxSettings.SetCurrentCategory(PageBuilder.cameraSettings);
        }

        public override void OnExit(GameState nextState)
        {
            base.gameObject.SetActive(false);
            GameStateMachine.Instance.PauseObject.SetActive(false);
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
