using System.Collections.Generic;
using UnityEngine;
using Assets.Gamelogic.UI;
using Assets.Gamelogic.Utils;

namespace Assets.Gamelogic.Core
{
    public class DeathAnimVisualizerPool : MonoBehaviour
    {
        private static Stack<GameObject> deathEffectInstances;
        private static GameObject deathEffectPrefab;
        private static DeathAnimVisualizerPool instance;

        private void Awake()
        {
            instance = this;
            InitializePool(ResourceRegistry.DeathEffectPrefab);
        }

        private void InitializePool(GameObject prefab)
        {
            deathEffectInstances = new Stack<GameObject>();
            deathEffectPrefab = prefab;
        }

        public static void ShowEffect(Vector3 position)
        {
            var effect = (deathEffectInstances.Count == 0) ? (GameObject)Instantiate(deathEffectPrefab, instance.transform, true) : deathEffectInstances.Pop();
            effect.transform.position = position + Vector3.up * SimulationSettings.DeathEffectSpawnHeight;
            effect.SetActive(true);
            instance.StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.DeathEffectDuration, () =>
            {
                ReturnEffectToPool(effect);
            }));
        }

        public static void ReturnEffectToPool(GameObject effect)
        {
            effect.SetActive(false);
            deathEffectInstances.Push(effect);
        }
    }
}
