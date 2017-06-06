using Assets.Gamelogic.Utils;
using Improbable.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    public class InitialPositionVisualizer : MonoBehaviour {

        [Require] private TransformComponent.Reader transformComponent;

        private void OnEnable ()
        {
            InitializeTransform();
        }

        private void InitializeTransform()
        {
            transform.position = transformComponent.Data.position.ToVector3();
            transform.rotation = Quaternion.Euler(0f, transformComponent.Data.rotation, 0f);
        }
    }
}
