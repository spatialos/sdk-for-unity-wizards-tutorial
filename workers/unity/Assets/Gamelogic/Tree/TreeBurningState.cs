using Assets.Gamelogic.FSM;
using Improbable.Fire;
using Improbable.Life;
using Improbable.Tree;

namespace Assets.Gamelogic.Tree
{
    public class TreeBurningState : FsmBaseState<TreeStateMachine, TreeFSMState>
    {
        private readonly Flammable.Writer flammable;
        private readonly Health.Writer health;

        public TreeBurningState(TreeStateMachine owner, Flammable.Writer inFlammable, Health.Writer inHealth) 
            : base(owner)
        {
            flammable = inFlammable;
            health = inHealth;
        }

        public override void Enter()
        {
            flammable.Send(new Flammable.Update().SetCanBeIgnited(false));

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
                Owner.TriggerTransition(TreeFSMState.BURNT);
            }
        }

        private void OnFlammableUpdated(Flammable.Update update)
        {
            if (HasBeenExtinguished(update))
            {
                Owner.TriggerTransition(TreeFSMState.HEALTHY);
            }
        }

        private bool HasBeenExtinguished(Flammable.Update flammableUpdate)
        {
            return flammableUpdate.isOnFire.HasValue && !flammableUpdate.isOnFire.Value;
        }
    }
}
