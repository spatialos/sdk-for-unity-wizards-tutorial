using Assets.Gamelogic.Core;
using Assets.Gamelogic.FSM;
using Improbable.Building;
using Improbable.Life;

namespace Assets.Gamelogic.Building.Barracks
{
    public class BarracksUnderConstructionState : FsmBaseState<BarracksStateMachine, BarracksState>
    {
        private readonly Health.Reader health;
        private readonly NPCSpawnerBehaviour npcSpawnerBehaviour;

        public BarracksUnderConstructionState(BarracksStateMachine owner, 
                                              Health.Reader inHealth, 
                                              NPCSpawnerBehaviour inNPCSpawnerBehaviour) : base(owner)
        {
            health = inHealth;
            npcSpawnerBehaviour = inNPCSpawnerBehaviour;
        }

        public override void Enter()
        {
            Owner.SetCanAcceptResources(true);
            npcSpawnerBehaviour.SetSpawningEnabled(false);

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
                EvaluateAndTransitionToConstructionFinishedState(update);
            }
        }

        private void EvaluateAndTransitionToConstructionFinishedState(Health.Update update)
        {
            if (update.currentHealth.Value == SimulationSettings.BarracksMaxHealth)
            {
                Owner.TriggerTransition(BarracksState.CONSTRUCTION_FINISHED);
            }
        }
    }
}
