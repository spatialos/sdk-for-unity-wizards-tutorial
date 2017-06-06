using Assets.Gamelogic.Core;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.NPC.LumberJack;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Npc;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Lumberjack
{
    public class LumberjackMoveToTargetState : FsmBaseState<LumberjackStateMachine, LumberjackFSMState.StateEnum>
    {
        private readonly TargetNavigation.Writer targetNavigation;
        private readonly LumberjackBehaviour parentBehaviour;
        private readonly TargetNavigationBehaviour navigation;
        
        private Coroutine interactionWithTargetDelayCoroutine;

        public LumberjackMoveToTargetState(LumberjackStateMachine owner,
                                           LumberjackBehaviour inParentBehaviour,
                                           TargetNavigation.Writer inTargetNavigation,
                                           TargetNavigationBehaviour inNavigation)
            : base(owner)
        {
            targetNavigation = inTargetNavigation;
            parentBehaviour = inParentBehaviour;
            navigation = inNavigation;
        }

        public override void Enter()
        {
            targetNavigation.ComponentUpdated.Add(OnTargetNavigationUpdated);
            StartMovingTowardsTarget();
        }

        public override void Tick()
        {
        }

        public override void Exit(bool disabled)
        {
            targetNavigation.ComponentUpdated.Remove(OnTargetNavigationUpdated);
            StopInteractionWithTargetDelayCoroutine();
        }

        private void StopInteractionWithTargetDelayCoroutine()
        {
            if (interactionWithTargetDelayCoroutine != null)
            {
                parentBehaviour.StopCoroutine(interactionWithTargetDelayCoroutine);
                interactionWithTargetDelayCoroutine = null;
            }
        }

        private void StartMovingTowardsTarget()
        {
            if (TargetIsEntity())
            {
                StartMovingTowardsTargetEntity();
            }
            else
            {
                StartMovingTowardsTargetPosition();
            }
        }

        private void StartMovingTowardsTargetEntity()
        {
            var targetGameObject = NPCUtils.GetTargetGameObject(Owner.Data.targetEntityId);
            if (targetGameObject == null)
            {
                Owner.TriggerTransition(LumberjackFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
                return;
            }
            if (NPCUtils.IsWithinInteractionRange(parentBehaviour.transform.position, targetGameObject.transform.position, SimulationSettings.NPCDefaultInteractionSqrDistance))
            {
                InitiateInteractionWithTarget();
                return;
            }
            navigation.StartNavigation(Owner.Data.targetEntityId, SimulationSettings.NPCDefaultInteractionSqrDistance);
        }

        private void InitiateInteractionWithTarget()
        {
            StopInteractionWithTargetDelayCoroutine();
            interactionWithTargetDelayCoroutine = parentBehaviour.StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.NPCInteractionDelay, AttemptInteractionWithTarget));
        }

        private void StartMovingTowardsTargetPosition()
        {
            var targetPosition = Owner.Data.targetPosition.ToVector3();
            if (MathUtils.CompareEqualityEpsilon(targetPosition, SimulationSettings.InvalidPosition))
            {
                Owner.TriggerTransition(LumberjackFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
                return;
            }
            if (NPCUtils.IsWithinInteractionRange(parentBehaviour.transform.position, targetPosition, SimulationSettings.NPCDefaultInteractionSqrDistance))
            {
                Owner.TriggerTransition(LumberjackFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
                return;
            }
            navigation.StartNavigation(targetPosition, SimulationSettings.NPCDefaultInteractionSqrDistance);
        }

        private void OnTargetNavigationUpdated(TargetNavigation.Update update)
        {
            if (update.navigationFinished.Count > 0)
            {
                var success = update.navigationFinished[update.navigationFinished.Count - 1].success;
                if (success)
                {
                    if (TargetIsEntity())
                    {
                        InitiateInteractionWithTarget();
                    }
                    else
                    {
                        Owner.TriggerTransition(LumberjackFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
                    }
                }
                else
                {
                    Owner.TriggerTransition(LumberjackFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
                }
            }
        }

        private void AttemptInteractionWithTarget()
        {
            var targetGameObject = NPCUtils.GetTargetGameObject(Owner.Data.targetEntityId);
            if (targetGameObject == null)
            {
                Owner.TriggerTransition(LumberjackFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
                return;
            }
            if (NPCUtils.IsTargetAHealthyTree(parentBehaviour.gameObject, targetGameObject) &&
                NPCUtils.IsWithinInteractionRange(parentBehaviour.transform.position, targetGameObject.transform.position, SimulationSettings.NPCDefaultInteractionSqrDistance))
            {
                Owner.TriggerTransition(LumberjackFSMState.StateEnum.HARVESTING, Owner.Data.targetEntityId, SimulationSettings.InvalidPosition);
                return;
            }
            if (NPCUtils.IsTargetATeamStockpile(parentBehaviour.gameObject, targetGameObject) &&
                NPCUtils.IsWithinInteractionRange(parentBehaviour.transform.position, targetGameObject.transform.position, SimulationSettings.NPCDefaultInteractionSqrDistance))
            {
                Owner.TriggerTransition(LumberjackFSMState.StateEnum.STOCKPILING, Owner.Data.targetEntityId, SimulationSettings.InvalidPosition);
                return;
            }
            Owner.TriggerTransition(LumberjackFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
        }

        private bool TargetIsEntity()
        {
            return Owner.Data.targetEntityId.IsValid();
        }
    }
}
