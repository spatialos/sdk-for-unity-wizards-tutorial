using Assets.Gamelogic.ComponentExtensions;
using UnityEngine;
using Assets.Gamelogic.FSM;
using Improbable.Npc;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.NPC.Lumberjack;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Tree;
using Improbable.Unity.Core;
using Improbable.Worker;

namespace Assets.Gamelogic.NPC.LumberJack
{
    public class LumberjackHarvestingState : FsmBaseState<LumberjackStateMachine, LumberjackFSMState.StateEnum>
    {
        private readonly LumberjackBehaviour parentBehaviour;
        private readonly Inventory.Writer inventory;

        private Coroutine harvestTreeDelayCoroutine;
        private Coroutine transitionToIdleDelayCoroutine;

        public LumberjackHarvestingState(LumberjackStateMachine owner,
                                         LumberjackBehaviour inParentBehaviour,
                                         Inventory.Writer inInventory)
            : base(owner)
        {
            parentBehaviour = inParentBehaviour;
            inventory = inInventory;
        }

        public override void Enter()
        {
            if (!inventory.HasResources())
            {
                harvestTreeDelayCoroutine = parentBehaviour.StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.NPCChoppingAnimationStartDelay, AttemptToHarvestTree));
            }
            else
            {
                TransitionToIdle();
            }
        }

        public override void Tick()
        {
        }

        public override void Exit(bool disabled)
        {
            StopHarvestTreeDelayRoutine();
            StopTransitionToRoutine();
        }

        private void StopHarvestTreeDelayRoutine()
        {
            if (harvestTreeDelayCoroutine != null)
            {
                parentBehaviour.StopCoroutine(harvestTreeDelayCoroutine);
                harvestTreeDelayCoroutine = null;
            }
        }

        private void StopTransitionToRoutine()
        {
            if (transitionToIdleDelayCoroutine != null)
            {
                parentBehaviour.StopCoroutine(transitionToIdleDelayCoroutine);
                transitionToIdleDelayCoroutine = null;
            }
        }

        private void AttemptToHarvestTree()
        {
            var targetGameObject = NPCUtils.GetTargetGameObject(Owner.Data.targetEntityId);
            if (targetGameObject != null && NPCUtils.IsTargetAHealthyTree(parentBehaviour.gameObject, targetGameObject))
            {
                SpatialOS.Commands.SendCommand(inventory,
                    Harvestable.Commands.Harvest.Descriptor,
                    new HarvestRequest(parentBehaviour.gameObject.EntityId()), Owner.Data.targetEntityId, OnHarvestResponse);
            }
            else
            {
                Owner.TriggerTransition(LumberjackFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
            }
        }

        private void OnHarvestResponse(ICommandCallbackResponse<HarvestResponse> response)
        {
            if (response.StatusCode != StatusCode.Success)
            {
                Debug.LogWarning("NPC failed to receive Harvest response");
            }
            else
            {
                inventory.AddToInventory(response.Response.Value.resourcesTaken);
            }
            TransitionToIdle();
        }

        private void TransitionToIdle()
        {
            var waitAndPerfromTransition = TimerUtils.WaitAndPerform(SimulationSettings.NPCChoppingAnimationFinishDelay, () =>
            {
                Owner.TriggerTransition(LumberjackFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
            });
            transitionToIdleDelayCoroutine = parentBehaviour.StartCoroutine(waitAndPerfromTransition);
        }
    }
}
