using Assets.Gamelogic.Core;
using Assets.Gamelogic.Fire;
using Assets.Gamelogic.Life;
using Improbable.Fire;
using Improbable.Life;
using Improbable.Tree;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Tree
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class TreeBehaviour : MonoBehaviour
    {
        [Require] private TreeState.Writer tree;
        [Require] private Flammable.Writer flammable;
        [Require] private Health.Writer health;

        [SerializeField] private FlammableBehaviour flammableInterface;

        private TreeStateMachine stateMachine;

        private void Awake()
        {
            flammableInterface = gameObject.GetComponentIfUnassigned(flammableInterface);
        }

        private void OnEnable()
        {
            stateMachine = new TreeStateMachine(this, 
                tree,
                health,
                flammableInterface,
                flammable);

            stateMachine.OnEnable(tree.Data.currentState);
        }

        private void OnDisable()
        {
            stateMachine.OnDisable();
        }

    }
}
