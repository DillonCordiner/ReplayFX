using GameManagement;
using Rewired;

namespace ReplayFX.State
{
    public class RFXMenuState : GameState
    {
        public override void OnEnter(GameState prevState)
        {
            base.gameObject.SetActive(true);
            //GameStateMachine.Instance.RequestTransitionTo(Main.replayfxMenu.rfxMenuState);
            //GameStateMachine.Instance.SemiTransparentLayer.SetActive(false);
            //GameStateMachine.Instance.PauseObject.SetActive(true);
            //Main.rfxSettings.SetCurrentCategory(PageBuilder.cameraSettings);
            //Main.rfxSettings.SetStartPage(PageBuilder.cameraSettings);
            Main.replayfxMenu.UpdateUI();
        }

        public override void OnExit(GameState nextState)
        {
            base.gameObject.SetActive(false);
            //GameStateMachine.Instance.RequestPreviousState();
            //GameStateMachine.Instance.PauseObject.SetActive(false);
            UISounds.Instance.PlayOneShotExit();
        }

        public override void OnUpdate()
        {
            Player player = Main.inputListener.player;
            if(player == null)
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

    }
}
