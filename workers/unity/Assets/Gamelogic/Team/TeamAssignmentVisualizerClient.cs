using Assets.Gamelogic.Core;
using Improbable.Team;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Team
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class TeamAssignmentVisualizerClient : MonoBehaviour
    {
        [Require] private TeamAssignment.Reader teamAssignment;

        public uint TeamId { get { return teamAssignment.Data.teamId; } }

        [SerializeField] private Material RedMaterial;
        [SerializeField] private Material BlueMaterial;
        [SerializeField] private Renderer[] ModelRenderers;

        private void OnEnable()
        {
            teamAssignment.ComponentUpdated.Add(TeamAssigned);
            SetTeamColour(teamAssignment.Data.teamId);
        }

        private void OnDisable()
        {
            teamAssignment.ComponentUpdated.Remove(TeamAssigned);
        }

        private void TeamAssigned(TeamAssignment.Update teamAssigned)
        {
            SetTeamColour(teamAssigned.teamId.Value);
        }

        private void SetTeamColour(uint teamIdValue)
        {
            Material newMaterial = null;
            switch (teamIdValue)
            {
                case SimulationSettings.RedTeamId:
                    newMaterial = RedMaterial;
                    break;
                case SimulationSettings.BlueTeamId:
                    newMaterial = BlueMaterial;
                    break;
            }

            for (int rendererNum = 0; rendererNum < ModelRenderers.Length; rendererNum++)
            {
                Material[] newMats = ModelRenderers[rendererNum].sharedMaterials;

                for (int i = 0; i < newMats.Length; i++)
                {
                    newMats[i] = newMaterial;
                }

                ModelRenderers[rendererNum].sharedMaterials = newMats;
            }
        }
    }
}
