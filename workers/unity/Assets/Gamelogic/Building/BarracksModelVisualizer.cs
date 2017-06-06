using Improbable.Building;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Building
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class BarracksModelVisualizer : MonoBehaviour
    {
        [Require] BarracksInfo.Reader barracksInfo;
        
        [SerializeField] private ParticleSystem transition;
        [SerializeField] private GameObject buildingModel;
        [SerializeField] private GameObject stockpileModel;

        private void OnEnable()
        {
            SwitchToBarracksState(barracksInfo.Data.barracksState);
            barracksInfo.ComponentUpdated.Add(OnComponentUpdated);
        }

        private void OnDisable()
        {
            barracksInfo.ComponentUpdated.Remove(OnComponentUpdated);
        }

        private void OnComponentUpdated(BarracksInfo.Update update)
        {
            if (update.barracksState.HasValue)
            {
                transition.Play();
                SwitchToBarracksState(update.barracksState.Value);
            }
        }

        private void SwitchToBarracksState(BarracksState barracksState)
        {
            switch (barracksState)
            {
                case BarracksState.UNDER_CONSTRUCTION:
                    {
                        buildingModel.SetActive(false);
                        stockpileModel.SetActive(true);
                    }
                    break;
                case BarracksState.CONSTRUCTION_FINISHED:
                    {
                        buildingModel.SetActive(true);
                        stockpileModel.SetActive(false);
                    }
                    break;
            }
        }
    }
}
