using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class TransformReceiverClient : MonoBehaviour
    {
        [Require] private Position.Reader positionComponent;
        [Require] private TransformComponent.Reader transformComponent;

        private bool isRemote;

        [SerializeField] private Rigidbody myRigidbody;

        private void Awake()
        {
            myRigidbody = gameObject.GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            transformComponent.ComponentUpdated.Add(OnTransformComponentUpdated);
            if (IsNotAnAuthoritativePlayer())
            {
                SetUpRemoteTransform();
            }     
        }

        private void OnDisable()
        {
            transformComponent.ComponentUpdated.Remove(OnTransformComponentUpdated);
            if (isRemote)
            {
                TearDownRemoveTransform();
            }
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

        private bool IsNotAnAuthoritativePlayer()
        {
            return gameObject.GetAuthority(ClientAuthorityCheck.ComponentId) == Authority.NotAuthoritative;
        }

        private void Update()
        {
            if (IsNotAnAuthoritativePlayer())
            {
                myRigidbody.MovePosition(Vector3.Lerp(myRigidbody.position, positionComponent.Data.coords.ToVector3(), 0.2f));
                myRigidbody.MoveRotation(Quaternion.Euler(0f, QuantizationUtils.DequantizeAngle(transformComponent.Data.rotation), 0f));
            }
            else if(isRemote)
            {
                TearDownRemoveTransform();
            }
        }

        private void SetUpRemoteTransform()
        {
            isRemote = true;
            myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            myRigidbody.isKinematic = true;
        }

        private void TearDownRemoveTransform()
        {
            isRemote = false;
            myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            myRigidbody.isKinematic = false;
        }
    }
}
