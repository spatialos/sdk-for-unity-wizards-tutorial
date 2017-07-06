using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Abilities;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Abilities
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class SpellcastAudioTriggers : MonoBehaviour
    {
        [Require] private Spells.Reader spells;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip SpellChannelAudio;
        [SerializeField] private AudioClip LightningAudio;
        [SerializeField] private AudioClip RainAudio;

        private void Awake()
        {
            audioSource = gameObject.GetComponentIfUnassigned<AudioSource>(audioSource);
        }

        private void OnEnable ()
        {
            spells.ComponentUpdated.Add(OnSpellCast);
        }

        private void OnDisable()
        {
            spells.ComponentUpdated.Remove(OnSpellCast);
        }

        private void OnSpellCast(Spells.Update spellCastUpdate)
        {
            for (var eventNum = 0; eventNum < spellCastUpdate.spellAnimationEvent.Count; eventNum++)
            {
                var spellCast = spellCastUpdate.spellAnimationEvent[eventNum];
                TriggerSpellcastAudio(spellCast.spellType, spellCast.position);
            }
        }

        public void TriggerSpellChannelAudio()
        {
            audioSource.spatialBlend = 1;
            audioSource.volume = SimulationSettings.SpellChannelVolume;
            audioSource.PlayOneShot(SpellChannelAudio);
        }

        private void TriggerSpellcastAudio(SpellType spellCastType, Coordinates spellCastPosition)
        {
            audioSource.spatialBlend = 1;
            switch (spellCastType)
            {
                case SpellType.LIGHTNING:
                    audioSource.volume = SimulationSettings.LightningStrikeVolume;
                    audioSource.PlayOneShot(LightningAudio);
                    break;
                case SpellType.RAIN:
                    audioSource.volume = SimulationSettings.RainVolume;
                    audioSource.PlayOneShot(RainAudio);
                    break;
            }
        }
    }
}
