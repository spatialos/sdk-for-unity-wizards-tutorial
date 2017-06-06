using Assets.Gamelogic.Building.Barracks;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Fire;
using Improbable.Building;
using Improbable.Fire;
using Improbable.Life;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Building
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class BarracksBehaviour : MonoBehaviour, IFlammable
    {
        [Require] private BarracksInfo.Writer barracksInfo;
        [Require] private StockpileDepository.Writer stockpileDepository;
        [Require] private Health.Writer health;
        [Require] private Flammable.Writer flammable;
        [Require] private NPCSpawner.Writer npcSpawner;
        
        [SerializeField] private FlammableBehaviour flammableBehaviour;
        [SerializeField] private NPCSpawnerBehaviour npcSpawnerBehaviour;

        private BarracksStateMachine barracksStateMachine;

        private void Awake()
        {
            flammableBehaviour = gameObject.GetComponentIfUnassigned(flammableBehaviour);
            npcSpawnerBehaviour = gameObject.GetComponentIfUnassigned(npcSpawnerBehaviour);
        }

        private void OnEnable()
        {
            barracksStateMachine = new BarracksStateMachine(barracksInfo, stockpileDepository, health, flammableBehaviour, npcSpawnerBehaviour);
            barracksStateMachine.OnEnable(barracksInfo.Data.barracksState);
        }

        private void OnDisable()
        {
            barracksStateMachine.OnDisable();
        }

        public void OnIgnite()
        {
            barracksStateMachine.SetCanAcceptResources(false);
        }

        public void OnExtinguish()
        {
            barracksStateMachine.SetCanAcceptResources(barracksStateMachine.EvaluateCanAcceptResources());
        }
    }
}
