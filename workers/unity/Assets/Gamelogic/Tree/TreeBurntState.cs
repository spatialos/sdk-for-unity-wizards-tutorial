using System.Collections;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Fire;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.Utils;
using Improbable.Fire;
using Improbable.Tree;
using UnityEngine;

namespace Assets.Gamelogic.Tree
{
    public class TreeBurntState : FsmBaseState<TreeStateMachine, TreeFSMState>
    {
        private readonly TreeBehaviour parentBehaviour;
        private readonly Flammable.Writer flammable;
        private readonly FlammableBehaviour flammableInterface;

        private Coroutine regrowingCoroutine;

        public TreeBurntState(TreeStateMachine owner, TreeBehaviour inParentBehaviour, Flammable.Writer inFlammable, FlammableBehaviour inFlammableInterface) : base(owner)
        {
            parentBehaviour = inParentBehaviour;
            flammable = inFlammable;
            flammableInterface = inFlammableInterface;
        }

        public override void Enter()
        {
            flammableInterface.SelfExtinguish(flammable, false);
            if (regrowingCoroutine == null)
            {
                regrowingCoroutine = parentBehaviour.StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.BurntTreeRegrowthTimeSecs, Regrow));
            }
        }

        private void Regrow()
        {
            Owner.TriggerTransition(TreeFSMState.HEALTHY);
        }

        public override void Tick()
        {
        }

        public override void Exit(bool disabled)
        {
            if (regrowingCoroutine != null)
            {
                parentBehaviour.StopCoroutine(regrowingCoroutine);
                regrowingCoroutine = null;
            }
        }
    }
}
