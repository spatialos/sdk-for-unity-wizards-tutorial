using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Communication
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class NotificationRotator : MonoBehaviour
    {
        private void LateUpdate()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                transform.forward = Camera.main.transform.forward;
            }
        }
    }
}