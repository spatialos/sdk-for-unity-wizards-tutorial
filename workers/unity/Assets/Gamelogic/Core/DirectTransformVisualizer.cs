using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    public class DirectTransformVisualizer : MonoBehaviour
    {
        [Require] private Position.Reader positionComponent;
        [Require] private TransformComponent.Reader transformComponent;

        private void OnEnable()
        {
            positionComponent.ComponentUpdated.Add(VisualizePosition);
            transformComponent.ComponentUpdated.Add(VisualizeTransform);
            SetPosition(positionComponent.Data.coords);
            SetRotation(transformComponent.Data.rotation);
        }

        private void OnDisable()
        {
            positionComponent.ComponentUpdated.Remove(VisualizePosition);
            transformComponent.ComponentUpdated.Remove(VisualizeTransform);
        }

        private void VisualizePosition(Position.Update update)
        {
            if(update.coords.HasValue)
            {
                SetPosition(update.coords.Value);
            }
        }

        private void VisualizeTransform(TransformComponent.Update update)
        {
            if (update.rotation.HasValue)
            {
                SetRotation(update.rotation.Value);
            }
        }

        private void SetPosition(Coordinates position)
        {
            transform.position = position.ToVector3();
        }

        private void SetRotation(uint rotation)
        {
            transform.rotation = Quaternion.Euler(0f, QuantizationUtils.DequantizeAngle(rotation), 0f);
        }
    }
}
