using System;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class TransformReceiverUnityWorker : MonoBehaviour
    {
        [Require] private Position.Reader positionComponent;
        [Require] private TransformComponent.Reader rotationComponent;

        [SerializeField] private Rigidbody myRigidbody;

        private void Awake()
        {
            myRigidbody = gameObject.GetComponentIfUnassigned(myRigidbody);
        }

        private void OnEnable()
        {
            positionComponent.CoordsUpdated.AddAndInvoke(OnPositionUpdated);
            rotationComponent.RotationUpdated.AddAndInvoke(OnRotationUpdated);
        }

        private void OnDisable()
        {
            positionComponent.CoordsUpdated.Remove(OnPositionUpdated);
            rotationComponent.RotationUpdated.Remove(OnRotationUpdated);
        }

        private void OnPositionUpdated(Coordinates coords)
        {
            if (!positionComponent.HasAuthority)
            {
                myRigidbody.MovePosition(coords.ToVector3());
            }
        }

        private void OnRotationUpdated(uint rotation)
        {
            if (!rotationComponent.HasAuthority)
            {
                myRigidbody.MoveRotation(Quaternion.Euler(0f, QuantizationUtils.DequantizeAngle(rotation), 0f));
            }
        }
    }
}
