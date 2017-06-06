using Assets.Gamelogic.FSM;
using Improbable.Building;
using Improbable.Life;

namespace Assets.Gamelogic.Building.Barracks
{
    public class BarracksConstructionFinishedState : FsmBaseState<BarracksStateMachine, BarracksState>
    {
        private readonly Health.Writer health;
        private readonly NPCSpawnerBehaviour npcSpawnerBehaviour;
        
        public BarracksConstructionFinishedState(BarracksStateMachine owner, 
                                                 Health.Writer inHealth,
                                                 NPCSpawnerBehaviour inNPCSpawnerBehaviour) : base(owner)
        {
            health = inHealth;
            npcSpawnerBehaviour = inNPCSpawnerBehaviour;
        }

        public override void Enter()
        {
            Owner.SetCanAcceptResources(false);
            npcSpawnerBehaviour.SetSpawningEnabled(true);
            npcSpawnerBehaviour.ResetCooldowns();

            health.ComponentUpdated.Add(OnHealthUpdated);
        }

        public override void Tick()
        {
        }

        public override void Exit(bool disabled)
        {
            health.ComponentUpdated.Remove(OnHealthUpdated);
        }

        private void OnHealthUpdated(Health.Update update)
        {
            if (update.currentHealth.HasValue)
            {
                Owner.EvaluateAndSetFlammability(update);
                EvaluateAndTransitionToUnderConstructionState(update);
            }
        }
        
        private void EvaluateAndTransitionToUnderConstructionState(Health.Update update)
        {
            if (update.currentHealth.Value <= 0)
            {
                Owner.TriggerTransition(BarracksState.UNDER_CONSTRUCTION);
            }
        }
    }
}
