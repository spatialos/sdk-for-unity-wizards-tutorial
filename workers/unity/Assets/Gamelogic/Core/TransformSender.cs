using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class TransformSender : MonoBehaviour
    {
        [Require] private Position.Writer positionComponent;
        [Require] private TransformComponent.Writer transformComponent;

        private int fixedFramesSinceLastUpdate = 0;

        public void TriggerTeleport(Vector3 position)
        {
            transform.position = position;
            positionComponent.Send(new Position.Update().SetCoords(position.ToCoordinates()));
            transformComponent.Send(new TransformComponent.Update().AddTeleportEvent(new TeleportEvent(position.ToCoordinates())));
        }

        private void OnEnable()
        {
            transform.position = positionComponent.Data.coords.ToVector3();
        }

        private void FixedUpdate()
        {
            var newPosition = transform.position.ToCoordinates();
            var newRotation = QuantizationUtils.QuantizeAngle(transform.rotation.eulerAngles.y);
            fixedFramesSinceLastUpdate++;
            if ((PositionNeedsUpdate(newPosition) || RotationNeedsUpdate(newRotation)) && fixedFramesSinceLastUpdate > SimulationSettings.TransformUpdatesToSkipBetweenSends)
            {
                fixedFramesSinceLastUpdate = 0;
                positionComponent.Send(new Position.Update().SetCoords(newPosition));
                transformComponent.Send(new TransformComponent.Update().SetRotation(newRotation));
            }
        }

        private bool PositionNeedsUpdate(Coordinates newPosition)
        {
            return !MathUtils.CompareEqualityEpsilon(newPosition, positionComponent.Data.coords);
        }

        private bool RotationNeedsUpdate(float newRotation)
        {
            return !MathUtils.CompareEqualityEpsilon(newRotation, transformComponent.Data.rotation);
        }
    }
}
