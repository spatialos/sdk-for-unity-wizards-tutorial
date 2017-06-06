using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable.Unity.Core;
using Improbable.Communication;


namespace Assets.Gamelogic.Communication
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class SendChatBehaviour : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer authCheck;
        
        public void SayChat(string message)
        {
			//Todo: Send a chat message here!
        }
    }
}