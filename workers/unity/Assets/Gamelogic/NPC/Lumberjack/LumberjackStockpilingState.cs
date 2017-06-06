using Assets.Gamelogic.ComponentExtensions;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.NPC.Lumberjack;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Building;
using Improbable.Core;
using Improbable.Npc;
using Improbable.Unity.Core;
using Improbable.Worker;
using UnityEngine;

namespace Assets.Gamelogic.NPC.LumberJack
{
    public class LumberjackStockpilingState : FsmBaseState<LumberjackStateMachine, LumberjackFSMState.StateEnum>
    {
        private readonly LumberjackBehaviour parentBehaviour;
        private readonly Inventory.Writer inventory;

        private Coroutine addToStockpileDelayCoroutine;
        private Coroutine transitionToIdleDelayCoroutine;

        public LumberjackStockpilingState(LumberjackStateMachine owner,
                                          LumberjackBehaviour inParentBehaviour,
                                          Inventory.Writer inInventory)
            : base(owner)
        {
            parentBehaviour = inParentBehaviour;
            inventory = inInventory;
        }

        public override void Enter()
        {
            if (inventory.HasResources())
            {
                addToStockpileDelayCoroutine = parentBehaviour.StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.NPCStockpilingAnimationStartDelay, AddToStockpile));
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
            StopAddToStockpileDelayCoroutine();
            StopTransitionToIdleDelayCoroutine();
        }

        private void StopAddToStockpileDelayCoroutine()
        {
            if (addToStockpileDelayCoroutine != null)
            {
                parentBehaviour.StopCoroutine(addToStockpileDelayCoroutine);
                addToStockpileDelayCoroutine = null;
            }
        }

        private void StopTransitionToIdleDelayCoroutine()
        {
            if (transitionToIdleDelayCoroutine != null)
            {
                parentBehaviour.StopCoroutine(transitionToIdleDelayCoroutine);
                transitionToIdleDelayCoroutine = null;
            }
        }

        private void AddToStockpile()
        {
            var targetGameObject = NPCUtils.GetTargetGameObject(Owner.Data.targetEntityId);
            if (targetGameObject != null && NPCUtils.IsTargetATeamStockpile(parentBehaviour.gameObject, targetGameObject))
            {
                var resourcesToAdd = inventory.Data.resources;
                SpatialOS.Commands.SendCommand(inventory, StockpileDepository.Commands.AddResource.Descriptor,
                    new AddResource(resourcesToAdd), Owner.Data.targetEntityId, response => OnStockpileResponse(response, resourcesToAdd));
            }
            else
            {
                Owner.TriggerTransition(LumberjackFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
            }
        }

        private void OnStockpileResponse(ICommandCallbackResponse<Nothing> response, int resourcesToAdd)
        {
            if (response.StatusCode != StatusCode.Success)
            {
                Debug.LogError("NPC failed to receive Stockpile response");
            }
            else
            {
                inventory.RemoveFromInventory(resourcesToAdd);
            }
            TransitionToIdle();
        }

        private void TransitionToIdle()
        {
            var waitAndPerfromTransition = TimerUtils.WaitAndPerform(SimulationSettings.NPCStockpilingAnimationFinishdelay, () =>
            {
                Owner.TriggerTransition(LumberjackFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
            });
            transitionToIdleDelayCoroutine = parentBehaviour.StartCoroutine(waitAndPerfromTransition);
        }
    }
}
