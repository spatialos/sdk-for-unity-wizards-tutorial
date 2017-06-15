using Improbable.Team;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Team
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class TeamAssignmentVisualizerUnityWorker : MonoBehaviour
    {
        [Require] private TeamAssignment.Reader teamAssignmentReader;
        public uint TeamId { get { return teamAssignmentReader.Data.teamId; } }
    }
}
