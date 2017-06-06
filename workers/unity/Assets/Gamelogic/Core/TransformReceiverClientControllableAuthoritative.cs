using Assets.Gamelogic.Utils;
using Improbable.Core;
using Improbable.Fire;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class TransformReceiverClientControllableAuthoritative : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer clientAuthorityCheck;
        [Require] private TransformComponent.Reader transformComponent;
        [Require] private Flammable.Reader flammable;

        private Vector3 targetVelocity;

        [SerializeField] private Rigidbody myRigidbody;

        private void Awake()
        {
            myRigidbody = gameObject.GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            transformComponent.ComponentUpdated.Add(OnTransformComponentUpdated);
        }

        private void OnDisable()
        {
            transformComponent.ComponentUpdated.Remove(OnTransformComponentUpdated);
        }

        private void OnTransformComponentUpdated(TransformComponent.Update update)
        {
            for (int i = 0; i < update.teleportEvent.Count; i++)
            {
                TeleportTo(update.teleportEvent[i].targetPosition.ToVector3());
            }
        }

        private void TeleportTo(Vector3 position)
        {
            myRigidbody.velocity = Vector3.zero;
            myRigidbody.MovePosition(position);
        }

        public void SetTargetVelocity(Vector3 direction)
        {
            bool isOnFire = flammable != null && flammable.Data.isOnFire;
            var movementSpeed = SimulationSettings.PlayerMovementSpeed * (isOnFire ? SimulationSettings.OnFireMovementSpeedIncreaseFactor : 1f);
            targetVelocity = direction * movementSpeed;
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        public void MovePlayer()
        {
            var currentVelocity = myRigidbody.velocity;
            var velocityChange = targetVelocity - currentVelocity;
            if (ShouldMovePlayerAuthoritativeClient(velocityChange))
            {
                transform.LookAt(myRigidbody.position + targetVelocity);
                myRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            }
        }

        private bool ShouldMovePlayerAuthoritativeClient(Vector3 velocityChange)
        {
            return velocityChange.sqrMagnitude > Mathf.Epsilon && PlayerMovementCheatSafeguardPassedAuthoritativeClient(velocityChange);
        }

        private bool PlayerMovementCheatSafeguardPassedAuthoritativeClient(Vector3 velocityChange)
        {
            var result = velocityChange.sqrMagnitude < SimulationSettings.PlayerPositionUpdateMaxSqrDistance;
            if (!result)
            {
                Debug.LogError("Player movement cheat safeguard failed on Client. " + velocityChange.sqrMagnitude);
            }
            return result;
        }
    }
}
