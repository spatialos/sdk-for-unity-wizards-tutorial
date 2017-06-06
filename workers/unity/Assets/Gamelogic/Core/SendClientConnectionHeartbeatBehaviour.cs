using UnityEngine;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;

namespace Assets.Gamelogic.Core
{
    /// <summary>
    /// Regularly notifies the authoritative worker that this client is still alive and connected.
    /// When this times out (in case of an event like a client crash), the authoritative will delete the entity from the world.
    /// </summary>
    [WorkerType(WorkerPlatform.UnityClient)]
    public class SendClientConnectionHeartbeatBehaviour : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer clientAuthorityCheck;

        private Coroutine heartbeatCoroutine;

        private void OnEnable()
        {
            heartbeatCoroutine = StartCoroutine(TimerUtils.CallRepeatedly(SimulationSettings.HeartbeatCheckInterval, SendHeartbeat));
        }

        private void OnDisable()
        {
            StopCoroutine(heartbeatCoroutine);
        }

        private void SendHeartbeat()
        {
			SpatialOS.Commands.SendCommand(clientAuthorityCheck, ConnectionHeartbeat.Commands.Heartbeat.Descriptor, new Nothing(), gameObject.EntityId())
                .OnFailure(HeartbeatSendFailed);
        }

        private void HeartbeatSendFailed(ICommandErrorDetails response)
        {
            Debug.LogError("Player connection heartbeat failed to send. Player may timeout.");
        }

        private void OnApplicationQuit()
        {
            if (SpatialOS.IsConnected)
            {
                SpatialOS.Commands.DeleteEntity(clientAuthorityCheck, gameObject.EntityId())
                    .OnFailure(OnDeleteFailure);
                SpatialOS.Disconnect();
            }
        }

        private void OnDeleteFailure(ICommandErrorDetails commandErrorDetails)
        {
            Debug.LogErrorFormat("Failed to Delete Player Entity (#{0}) on quit: {1}", gameObject.EntityId(), commandErrorDetails.ErrorMessage);
        }
    }
}