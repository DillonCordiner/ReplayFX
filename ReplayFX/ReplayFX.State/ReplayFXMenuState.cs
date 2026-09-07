using GameManagement;
using ModIO.UI;
using ReplayFX;
using System.Reflection.Emit;
using UnityEngine.EventSystems;
using UnityEngine;
using ReplayFX.UI;

namespace ReplayFX.State
{
    public class ReplayFXMenuState : GameState
    {
        public override void OnEnter(GameState prevState)
        {
            base.gameObject.SetActive(true);
            //GameStateMachine.Instance.SemiTransparentLayer.SetActive(false);
            //GameStateMachine.Instance.PauseObject.SetActive(true);
            //Main.rfxSettings.SetCurrentCategory(PageBuilder.cameraSettings);
            //Main.rfxSettings.SetStartPage(PageBuilder.cameraSettings);
            Main.replayfxMenu.UpdateUI();
        }

        public override void OnExit(GameState nextState)
        {
            base.gameObject.SetActive(false);
            //GameStateMachine.Instance.PauseObject.SetActive(false);
            UISounds.Instance.PlayOneShotExit();
        }
    }
}
