using Improbable.Team;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Team
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class TeamAssignmentVisualizerFSim : MonoBehaviour
    {
        [Require] private TeamAssignment.Reader teamAssignmentReader;
        public uint TeamId { get { return teamAssignmentReader.Data.teamId; } }
    }
}
