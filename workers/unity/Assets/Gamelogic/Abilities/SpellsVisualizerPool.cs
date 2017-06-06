using System.Collections.Generic;
using Assets.Gamelogic.Core;
using UnityEngine;
using Assets.Gamelogic.UI;
using Assets.Gamelogic.Utils;
using Improbable.Abilities;

namespace Assets.Gamelogic.Abilities
{
    public class SpellsVisualizerPool : MonoBehaviour
    {
        private static IDictionary<SpellType, Stack<GameObject>> spellEffectInstances = new Dictionary<SpellType, Stack<GameObject>>();
        private static IDictionary<SpellType, GameObject> spellEffectPrefabs = new Dictionary<SpellType, GameObject>();
        private static SpellsVisualizerPool instance;

        private void Awake()
        {
            instance = this;
            InitializeSpellType(SpellType.LIGHTNING, ResourceRegistry.LightningEffectPrefab);
            InitializeSpellType(SpellType.RAIN, ResourceRegistry.RainEffectPrefab);
        }

        private void InitializeSpellType(SpellType spellType, GameObject prefab)
        {
            spellEffectInstances[spellType] = new Stack<GameObject>();
            spellEffectPrefabs[spellType] = prefab;
        }

        public static void ShowSpellEffect(Vector3 position, SpellType spellType)
        {
            var stack = spellEffectInstances[spellType];
            var effect = (stack.Count == 0) ? (GameObject)Instantiate(spellEffectPrefabs[spellType], instance.transform, true) : stack.Pop();
            effect.transform.position = position;
            if (spellType == SpellType.RAIN)
            {
                effect.transform.position += Vector3.up * SimulationSettings.RainCloudSpawnHeight;
            }
            effect.SetActive(true);
            instance.StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.SpellEffectDuration, () =>
            {
                ReturnEffectToPool(spellType, effect);
            }));
        }

        public static void ReturnEffectToPool(SpellType spellType, GameObject effect)
        {
            effect.SetActive(false);
            spellEffectInstances[spellType].Push(effect);
        }
    }
}
