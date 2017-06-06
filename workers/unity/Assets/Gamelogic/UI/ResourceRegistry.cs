using Assets.Gamelogic.Core;
using UnityEngine;

namespace Assets.Gamelogic.UI
{
    public static class ResourceRegistry
    {
        private static GameObject spellAOEIndicatorPrefab;
        public static GameObject SpellAOEIndicatorPrefab { get { return spellAOEIndicatorPrefab ?? (spellAOEIndicatorPrefab = Resources.Load<GameObject>(SimulationSettings.SpellAOEIndicatorPrefabPath)); } }

        private static GameObject lightningEffectPrefab;
        public static GameObject LightningEffectPrefab { get { return lightningEffectPrefab ?? (lightningEffectPrefab = Resources.Load<GameObject>(SimulationSettings.LightningEffectPrefabPath)); } }

        private static GameObject rainEffectPrefab;
        public static GameObject RainEffectPrefab { get { return rainEffectPrefab ?? (rainEffectPrefab = Resources.Load<GameObject>(SimulationSettings.RainEffectPrefabPath)); } }

        private static GameObject firePrefab;
        public static GameObject FirePrefab { get { return firePrefab ?? (firePrefab = Resources.Load<GameObject>(SimulationSettings.FireEffectPrefabPath)); } }

        private static GameObject smallFirePrefab;
        public static GameObject SmallFirePrefab { get { return smallFirePrefab ?? (smallFirePrefab = Resources.Load<GameObject>(SimulationSettings.SmallFireEffectPrefabPath)); } }

        private static GameObject entityInfoCanvasPrefab;
        public static GameObject EntityInfoCanvasPrefab { get { return entityInfoCanvasPrefab ?? (entityInfoCanvasPrefab = Resources.Load<GameObject>(SimulationSettings.EntityInfoCanvasPrefabPath)); } }

        private static GameObject deathEffectPrefab;
        public static GameObject DeathEffectPrefab { get { return deathEffectPrefab ?? (deathEffectPrefab = Resources.Load<GameObject>(SimulationSettings.DeathEffectPrefabPath)); } }
    }
}
