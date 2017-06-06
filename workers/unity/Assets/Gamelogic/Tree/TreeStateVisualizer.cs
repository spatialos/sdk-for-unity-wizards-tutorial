using Improbable.Tree;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Tree
{
    public class TreeStateVisualizer : MonoBehaviour
    {
        [Require] private TreeState.Reader treeState;
        public TreeState.Reader CurrentState { get { return treeState; } }
    }
}
