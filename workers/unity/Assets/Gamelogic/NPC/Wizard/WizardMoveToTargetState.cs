using Assets.Gamelogic.Core;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Npc;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard
{
    public class WizardMoveToTargetState : FsmBaseState<WizardStateMachine, WizardFSMState.StateEnum>
    {
        private readonly WizardBehaviour parentBehaviour;
        private readonly TargetNavigation.Writer targetNavigation;
        private readonly TargetNavigationBehaviour navigation;

        private Coroutine checkForNearbyEnemiesOrAlliesCoroutine;

        public WizardMoveToTargetState(WizardStateMachine owner,
                                       WizardBehaviour inParentBehaviour,
                                       TargetNavigation.Writer inTargetNavigation,
                                       TargetNavigationBehaviour inNavigation)
            : base(owner)
        {
            parentBehaviour = inParentBehaviour;
            targetNavigation = inTargetNavigation;
            navigation = inNavigation;
        }

        public override void Enter()
        {
            targetNavigation.ComponentUpdated.Add(OnTargetNavigationUpdated);
            checkForNearbyEnemiesOrAlliesCoroutine = parentBehaviour.StartCoroutine(TimerUtils.CallRepeatedly(SimulationSettings.NPCPerceptionRefreshInterval, CheckForNearbyEnemiesOrAllies));
            StartMovingTowardsTarget();
        }

        public override void Tick()
        {
        }

        public override void Exit(bool disabled)
        {
            targetNavigation.ComponentUpdated.Remove(OnTargetNavigationUpdated);
            StopCheckForNearbyEnemiesOrAlliesCoroutine();
        }

        private void StopCheckForNearbyEnemiesOrAlliesCoroutine()
        {
            if (checkForNearbyEnemiesOrAlliesCoroutine != null)
            {
                parentBehaviour.StopCoroutine(checkForNearbyEnemiesOrAlliesCoroutine);
                checkForNearbyEnemiesOrAlliesCoroutine = null;
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
				Owner.TriggerTransition(WizardFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
                return;
            }
            if (NPCUtils.IsWithinInteractionRange(parentBehaviour.transform.position, targetGameObject.transform.position, SimulationSettings.NPCWizardSpellCastingSqrDistance))
            {
                AttemptInteractionWithTarget();
                return;
            }
            navigation.StartNavigation(Owner.Data.targetEntityId, SimulationSettings.NPCWizardSpellCastingSqrDistance);
        }

        private void StartMovingTowardsTargetPosition()
        {
            var targetPosition = Owner.Data.targetPosition.ToVector3();
            if (MathUtils.CompareEqualityEpsilon(targetPosition, SimulationSettings.InvalidPosition))
            {
                Owner.TriggerTransition(WizardFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
                return;
            }
            navigation.StartNavigation(targetPosition, SimulationSettings.NPCDefaultInteractionSqrDistance);
        }

        private void CheckForNearbyEnemiesOrAllies()
        {
            var nearestTarget = FindNearestTargetToAttackOrDefend();
            if (nearestTarget.IsValid())
            {
                if (!TargetIsEntity() || nearestTarget != Owner.Data.targetEntityId)
                {
                    Owner.TriggerTransition(WizardFSMState.StateEnum.MOVING_TO_TARGET, nearestTarget, SimulationSettings.InvalidPosition);
                }
            }
        }

        private EntityId FindNearestTargetToAttackOrDefend()
        {
            var layerMask = ~(1 << LayerMask.NameToLayer(SimulationSettings.TreeLayerName));
            var nearestDefendableTarget = NPCUtils.FindNearestTarget(parentBehaviour.gameObject, SimulationSettings.NPCViewRadius, NPCUtils.IsTargetDefendable, layerMask);
            var nearestAttackableTarget = NPCUtils.FindNearestTarget(parentBehaviour.gameObject, SimulationSettings.NPCViewRadius, NPCUtils.IsTargetAttackable, layerMask);

            if (nearestDefendableTarget == null && nearestAttackableTarget == null)
            {
				return new EntityId();
            }

            var sqrDistanceToNearestDefendableTarget = (nearestDefendableTarget != null) ? MathUtils.SqrDistance(parentBehaviour.transform.position, nearestDefendableTarget.transform.position) : float.MaxValue;
            var sqrDistanceToNearestAttackableTarget = (nearestAttackableTarget != null) ? MathUtils.SqrDistance(parentBehaviour.transform.position, nearestAttackableTarget.transform.position) : float.MaxValue;

            return (sqrDistanceToNearestDefendableTarget < sqrDistanceToNearestAttackableTarget) ? nearestDefendableTarget.gameObject.EntityId() : nearestAttackableTarget.gameObject.EntityId();
        }

        private void AttemptInteractionWithTarget()
        {
            var targetGameObject = NPCUtils.GetTargetGameObject(Owner.Data.targetEntityId);
            if (targetGameObject == null)
            {
				Owner.TriggerTransition(WizardFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
                return;
            }
            if (NPCUtils.IsTargetAttackable(parentBehaviour.gameObject, targetGameObject) &&
                NPCUtils.IsWithinInteractionRange(parentBehaviour.transform.position, targetGameObject.transform.position, SimulationSettings.NPCWizardSpellCastingSqrDistance))
            {
                Owner.TriggerTransition(WizardFSMState.StateEnum.ATTACKING_TARGET, Owner.Data.targetEntityId, SimulationSettings.InvalidPosition);
                return;
            }
            if (NPCUtils.IsTargetDefendable(parentBehaviour.gameObject, targetGameObject) &&
                NPCUtils.IsWithinInteractionRange(parentBehaviour.transform.position, targetGameObject.transform.position, SimulationSettings.NPCWizardSpellCastingSqrDistance))
            {
                Owner.TriggerTransition(WizardFSMState.StateEnum.DEFENDING_TARGET, Owner.Data.targetEntityId, SimulationSettings.InvalidPosition);
                return;
            }
			Owner.TriggerTransition(WizardFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
        }
        
        private void OnTargetNavigationUpdated(TargetNavigation.Update update)
        {
            if (update.navigationFinished.Count > 0)
            {
                if (TargetIsEntity())
                {
                    AttemptInteractionWithTarget();
                }
                else
                {
					Owner.TriggerTransition(WizardFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
                }
            }
        }

        private bool TargetIsEntity()
        {
            return Owner.Data.targetEntityId.IsValid();
        }
    }
}
