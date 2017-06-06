using Assets.Gamelogic.Core;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.Life;
using Improbable.Tree;
using Improbable.Fire;
using Improbable.Life;
using Assets.Gamelogic.ComponentExtensions;

namespace Assets.Gamelogic.Tree
{
    public class TreeHealthyState : FsmBaseState<TreeStateMachine, TreeFSMState>
    {
        private readonly Flammable.Writer flammable;
        private readonly Health.Writer health;

        public TreeHealthyState(TreeStateMachine owner, Flammable.Writer inFlammable, Health.Writer inHealth) 
            : base(owner)
        {
            flammable = inFlammable;
            health = inHealth;
        }

        public override void Enter()
        {
            health.SetCurrentHealth(SimulationSettings.TreeMaxHealth);
            flammable.Send(new Flammable.Update().SetCanBeIgnited(true));

            flammable.ComponentUpdated.Add(OnFlammableUpdated);
            health.ComponentUpdated.Add(OnHealthUpdated);
        }

        public override void Tick()
        {

        }

        public override void Exit(bool disabled)
        {
            health.ComponentUpdated.Remove(OnHealthUpdated);
            flammable.ComponentUpdated.Remove(OnFlammableUpdated);
        }

        private void OnHealthUpdated(Health.Update update)
        {
            if (update.currentHealth.HasValue && update.currentHealth.Value <= 0) 
            {
                Owner.TriggerTransition(TreeFSMState.STUMP);
            }
        }

        private void OnFlammableUpdated(Flammable.Update update)
        {
            if (HasBeenIgnited(update))
            {
                Owner.TriggerTransition(TreeFSMState.BURNING);
            }
        }

        private bool HasBeenIgnited(Flammable.Update flammableUpdate)
        {
            return flammableUpdate.isOnFire.HasValue && flammableUpdate.isOnFire.Value;
        }
    }
}
