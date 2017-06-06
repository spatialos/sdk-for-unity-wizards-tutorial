using Assets.Gamelogic.Utils;
using Improbable.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable.Math;

namespace Assets.Gamelogic.Core
{
    public class DirectTransformVisualizer : MonoBehaviour
    {
        [Require] private TransformComponent.Reader transformComponent;

        private void OnEnable()
        {
            transformComponent.ComponentUpdated.Add(VisualizeTransform);
            SetPosition(transformComponent.Data.position);
            SetRotation(transformComponent.Data.rotation);
        }

        private void OnDisable()
        {
            transformComponent.ComponentUpdated.Remove(VisualizeTransform);
        }

        private void VisualizeTransform(TransformComponent.Update update)
        {
            if(update.position.HasValue)
            {
                SetPosition(update.position.Value);
            }

            if(update.rotation.HasValue)
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
