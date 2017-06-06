using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable.Core;
using Improbable.Communication;
using Improbable.Entity.Component;

namespace Assets.Gamelogic.Communication
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class ChatBroadcasterBehaviour : MonoBehaviour
    {
        private void OnEnable()
        {
            //Todo: Handle sent chat messages here!
        }

        private void OnDisable()
        {
	        //Todo: Tidy up after chat implementation here!
        }
    }
}
