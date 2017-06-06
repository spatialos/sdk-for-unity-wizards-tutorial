using System.Collections;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable.Tree;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Tree
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class TreeModelVisualizer : MonoBehaviour
    {
        [Require] private TreeState.Reader treeState;

        [SerializeField] private GameObject HealthyTree;
        [SerializeField] private GameObject Stump;
        [SerializeField] private GameObject BurntTree;
        [SerializeField] private Mesh[] meshes;

        private void OnEnable()
        {
            SetupTreeModel();
            treeState.ComponentUpdated.Add(UpdateVisualization);
            ShowTreeModel(treeState.Data.currentState);
        }

        private void OnDisable()
        {
            treeState.ComponentUpdated.Remove(UpdateVisualization);
        }

        private void SetupTreeModel()
        {
            var treeModel = meshes[(int)treeState.Data.treeType];
            HealthyTree.GetComponent<MeshFilter>().mesh = treeModel;
        }

        private void UpdateVisualization(TreeState.Update newState)
        {
            ShowTreeModel(newState.currentState.Value);
        }

        private void ShowTreeModel(TreeFSMState currentState)
        {
            switch (currentState)
            {
                case TreeFSMState.HEALTHY:
                    StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.TreeExtinguishTimeBuffer, () =>
                    {
                        TransitionTo(HealthyTree);
                    }));
                    break;
                case TreeFSMState.STUMP:
                    StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.TreeCutDownTimeBuffer, () =>
                    {
                        TransitionTo(Stump); 
                    }));
                    break;
                case TreeFSMState.BURNING:
                    StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.TreeIgnitionTimeBuffer, () =>
                    {
                        TransitionTo(HealthyTree);
                    }));
                    break;
                case TreeFSMState.BURNT:
                    TransitionTo(BurntTree);
                    break;
            }
        }

        private void TransitionTo(GameObject newModel)
        {
            HideAllModels();
            newModel.SetActive(true);
        }

        private void HideAllModels()
        {
            HealthyTree.SetActive(false);
            Stump.SetActive(false);
            BurntTree.SetActive(false);
        }
    }
}
