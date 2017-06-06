using Assets.Gamelogic.Fire;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.Life;
using Improbable.Fire;
using Improbable.Tree;
using System.Collections.Generic;
using Improbable.Life;
using UnityEngine;

namespace Assets.Gamelogic.Tree
{
    public class TreeStateMachine : FiniteStateMachine<TreeFSMState>
    {
        private readonly TreeState.Writer tree;
        public TreeStateData Data;

        public TreeStateMachine(
              TreeBehaviour owner,
              TreeState.Writer inTree,
              Health.Writer health,
              FlammableBehaviour flammableInterface,
              Flammable.Writer flammable
        )
        {
            tree = inTree;

            var healthyState = new TreeHealthyState(this, flammable, health);
            var burningState = new TreeBurningState(this, flammable, health);
            var burntState = new TreeBurntState(this, owner, flammable, flammableInterface);
            var stumpState = new TreeStumpState(this, owner, flammable);

            var stateList = new Dictionary<TreeFSMState, IFsmState>();
            stateList.Add(TreeFSMState.HEALTHY, healthyState);
            stateList.Add(TreeFSMState.BURNING, burningState);
            stateList.Add(TreeFSMState.BURNT, burntState);
            stateList.Add(TreeFSMState.STUMP, stumpState);

            SetStates(stateList);

            var allowedTransitions = new Dictionary<TreeFSMState, IList<TreeFSMState>>();

            allowedTransitions.Add(TreeFSMState.HEALTHY, new List<TreeFSMState>()
            {
                TreeFSMState.BURNING,
                TreeFSMState.STUMP
            });

            allowedTransitions.Add(TreeFSMState.BURNING, new List<TreeFSMState>()
            {
                TreeFSMState.HEALTHY,
                TreeFSMState.BURNT
            });

            allowedTransitions.Add(TreeFSMState.BURNT, new List<TreeFSMState>()
            {
                TreeFSMState.HEALTHY
            });

            allowedTransitions.Add(TreeFSMState.STUMP, new List<TreeFSMState>()
            {
                TreeFSMState.HEALTHY
            });

            SetTransitions(allowedTransitions);
        }

        protected override void OnEnableImpl()
        {
            Data = tree.Data.DeepCopy();
        }

        public void TriggerTransition(TreeFSMState newState)
        {
            if (tree == null)
            {
                Debug.LogError("Trying to change state without authority.");
                return;
            }

            if (IsValidTransition(newState))
            {
                Data.currentState = newState;

                var update = new TreeState.Update();
                update.SetCurrentState(Data.currentState);
                tree.Send(update);

                TransitionTo(newState);
            }
            else
            {
                Debug.LogErrorFormat("Tree: Invalid transition from {0} to {1} detected.", Data.currentState, newState);
            }
        }
    }

}
