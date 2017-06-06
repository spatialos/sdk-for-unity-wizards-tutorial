namespace Assets.Gamelogic.FSM
{
    public abstract class FsmBaseState<TOwner, TStateEnum> : IFsmState where TOwner : FiniteStateMachine<TStateEnum>
    {
        protected FsmBaseState(TOwner owner)
        {
            Owner = owner;
        }

        public abstract void Enter();
        public abstract void Tick();
        public abstract void Exit(bool disabled);

        protected readonly TOwner Owner;
    }
}
