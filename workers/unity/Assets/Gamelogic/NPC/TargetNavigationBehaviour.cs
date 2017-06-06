using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Fire;
using Improbable.Npc;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class TargetNavigationBehaviour : MonoBehaviour
    {
        [Require] private TargetNavigation.Writer targetNavigation;
        [Require] private Flammable.Reader flammable;

        [SerializeField] private Rigidbody myRigidbody;
        [SerializeField] private Transform myTransform;

        private Vector3 targetPosition = SimulationSettings.InvalidPosition;
        
        private void Awake()
        {
            myRigidbody = gameObject.GetComponentIfUnassigned(myRigidbody);
            myTransform = gameObject.GetComponentIfUnassigned(myTransform);
        }

        public static bool IsInTransit(TargetNavigation.Reader targetNavigation)
        {
            return targetNavigation.Data.navigationState != NavigationState.INACTIVE;
        }

        public void StartNavigation(Vector3 position, float interactionSqrDistance)
        {
            var flatPosition = position.FlattenVector();
            targetNavigation.Send(new TargetNavigation.Update()
                .SetNavigationState(NavigationState.POSITION)
                .SetTargetPosition(flatPosition.ToVector3f())
				.SetTargetEntityId(new EntityId())
                .SetInteractionSqrDistance(interactionSqrDistance));
        }

        public void StartNavigation(EntityId targetEntityId, float interactionSqrDistance)
        {
            targetNavigation.Send(new TargetNavigation.Update()
                .SetNavigationState(NavigationState.ENTITY)
                .SetTargetPosition(SimulationSettings.InvalidPosition.ToVector3f())
                .SetTargetEntityId(targetEntityId)
                .SetInteractionSqrDistance(interactionSqrDistance));
        }

        public void StopNavigation()
        {
            if (IsInTransit(targetNavigation))
            {
                targetNavigation.Send(new TargetNavigation.Update()
                    .SetNavigationState(NavigationState.INACTIVE)
                    .SetTargetPosition(SimulationSettings.InvalidPosition.ToVector3f())
					.SetTargetEntityId(new EntityId())
                    .SetInteractionSqrDistance(0f));
            }
        }

        public void FinishNavigation(bool success)
        {
            StopNavigation();
            targetNavigation.Send(new TargetNavigation.Update().AddNavigationFinished(new NavigationFinished(success)));
        }

        private void Update()
        {
            TargetNavigationTick();
        }

        private void TargetNavigationTick()
        {
            if (!IsInTransit(targetNavigation))
            {
                return;
            }
            
            if (targetNavigation.Data.navigationState == NavigationState.ENTITY)
            {
                var targetGameObject = NPCUtils.GetTargetGameObject(targetNavigation.Data.targetEntityId);
                targetPosition = targetGameObject != null ? targetGameObject.transform.position.FlattenVector() : SimulationSettings.InvalidPosition;
            }

            if (targetNavigation.Data.navigationState == NavigationState.POSITION)
            {
                targetPosition = targetNavigation.Data.targetPosition.ToVector3();
            }

            if (MathUtils.CompareEqualityEpsilon(targetPosition, SimulationSettings.InvalidPosition))
            {
                FinishNavigation(false);
            }

            if (TargetPositionReached())
            {
                FinishNavigation(true);
            }

            MoveTowardsTargetPosition(Time.deltaTime);
        }

        private bool TargetPositionReached()
        {
            return MathUtils.SqrDistance(myTransform.position, targetPosition) < targetNavigation.Data.interactionSqrDistance;
        }

        private void MoveTowardsTargetPosition(float deltaTime)
        {
            var movementSpeed = SimulationSettings.NPCMovementSpeed * (flammable.Data.isOnFire ? SimulationSettings.OnFireMovementSpeedIncreaseFactor : 1f);
            var sqrDistanceToTarget = MathUtils.SqrDistance(targetPosition, myTransform.position);
            var distanceToTravel = movementSpeed * deltaTime;
            if ((distanceToTravel * distanceToTravel) < sqrDistanceToTarget)
            {
                myRigidbody.MovePosition(myTransform.position + (targetPosition - myTransform.position).normalized*distanceToTravel);
            }
            else
            {
                myRigidbody.MovePosition(targetPosition);
            }
            if (sqrDistanceToTarget > 0.01f)
            {
                myTransform.LookAt(targetPosition, Vector3.up);
            }
        }
    }
}
