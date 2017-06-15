using Assets.Gamelogic.Utils;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class TransformReceiverUnityWorker : MonoBehaviour
    {
        [Require] private TransformComponent.Reader transformComponent;

        [SerializeField] private Rigidbody myRigidbody;

        private void Awake()
        {
            myRigidbody = gameObject.GetComponentIfUnassigned(myRigidbody);
        }

        private void OnEnable()
        {
            UpdateTransform();
            transformComponent.ComponentUpdated.Add(OnComponentUpdated);
        }

        private void OnDisable()
        {
            transformComponent.ComponentUpdated.Remove(OnComponentUpdated);
        }

        private void OnComponentUpdated(TransformComponent.Update update)
        {
            if (!transformComponent.HasAuthority)
            {
                UpdateTransform();
            }
        }

        private void UpdateTransform()
        {
            myRigidbody.MovePosition(transformComponent.Data.position.ToVector3());
            myRigidbody.MoveRotation(Quaternion.Euler(0f, QuantizationUtils.DequantizeAngle(transformComponent.Data.rotation), 0f));
        }        
    }
}
