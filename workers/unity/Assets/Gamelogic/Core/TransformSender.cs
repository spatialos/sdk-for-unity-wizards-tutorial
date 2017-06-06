using Assets.Gamelogic.Utils;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class TransformSender : MonoBehaviour
    {
        [Require] private TransformComponent.Writer transformComponent;

        private int fixedFramesSinceLastUpdate = 0;

        public void TriggerTeleport(Vector3 position)
        {
            transform.position = position;
            transformComponent.Send(new TransformComponent.Update().SetPosition(position.ToCoordinates()).AddTeleportEvent(new TeleportEvent(position.ToCoordinates())));
        }

        private void OnEnable()
        {
            transform.position = transformComponent.Data.position.ToVector3();
        }

        private void FixedUpdate()
        {
            var newPosition = transform.position.ToCoordinates();
            var newRotation = QuantizationUtils.QuantizeAngle(transform.rotation.eulerAngles.y);
            fixedFramesSinceLastUpdate++;
            if ((PositionNeedsUpdate(newPosition) || RotationNeedsUpdate(newRotation)) && fixedFramesSinceLastUpdate > SimulationSettings.TransformUpdatesToSkipBetweenSends)
            {
                fixedFramesSinceLastUpdate = 0;
                transformComponent.Send(new TransformComponent.Update().SetPosition(newPosition).SetRotation(newRotation));
            }
        }

        private bool PositionNeedsUpdate(Coordinates newPosition)
        {
            return !MathUtils.CompareEqualityEpsilon(newPosition, transformComponent.Data.position);
        }

        private bool RotationNeedsUpdate(float newRotation)
        {
            return !MathUtils.CompareEqualityEpsilon(newRotation, transformComponent.Data.rotation);
        }
    }
}
