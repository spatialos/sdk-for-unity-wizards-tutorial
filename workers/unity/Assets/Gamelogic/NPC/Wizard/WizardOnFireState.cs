using Assets.Gamelogic.Core;
using Assets.Gamelogic.FSM;
using Improbable.Npc;

namespace Assets.Gamelogic.NPC.Wizard
{
    public class WizardOnFireState : FsmBaseState<WizardStateMachine, WizardFSMState.StateEnum>
    {
        private readonly TargetNavigation.Writer targetNavigation;
        private readonly TargetNavigationBehaviour navigation;

        public WizardOnFireState(WizardStateMachine owner,
                                 TargetNavigationBehaviour inNavigation,
                                 TargetNavigation.Writer inTargetNavigation) 
            : base(owner)
        {
            navigation = inNavigation;
            targetNavigation = inTargetNavigation;
        }

        public override void Enter()
        {
            targetNavigation.ComponentUpdated.Add(OnTargetNavigationComponentUpdate);
            NPCUtils.NavigateToRandomNearbyPosition(navigation, navigation.transform.position, SimulationSettings.NPCOnFireWaypointDistance, SimulationSettings.NPCDefaultInteractionSqrDistance);
        }

        public override void Tick()
        {
        }

        public override void Exit(bool disabled)
        {
            targetNavigation.ComponentUpdated.Remove(OnTargetNavigationComponentUpdate);
            if (!disabled)
            {
                navigation.StopNavigation();
            }
        }

        private void OnTargetNavigationComponentUpdate(TargetNavigation.Update update)
        {
            if (update.navigationFinished.Count > 0)
            {
                NPCUtils.NavigateToRandomNearbyPosition(navigation, navigation.transform.position, SimulationSettings.NPCOnFireWaypointDistance, SimulationSettings.NPCDefaultInteractionSqrDistance);
            }
        }
    }
}
