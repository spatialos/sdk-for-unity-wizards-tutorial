using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Abilities;
using Improbable.Collections;
using Improbable.Core;
using Improbable.Entity.Component;
using Improbable.Fire;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Abilities
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class SpellsBehaviour : MonoBehaviour
    {
        [Require] private Spells.Writer spells;

        private Collider[] spellTargets;
        private Coroutine reduceCooldownsCoroutine;

        private void OnEnable()
        {
            spellTargets = new Collider[SimulationSettings.MaxSpellTargets];
            spells.CommandReceiver.OnSpellCastRequest.RegisterResponse(OnSpellCastRequest);
            reduceCooldownsCoroutine = StartCoroutine(TimerUtils.CallRepeatedly(1f, ReduceCooldowns));
        }

        private void OnDisable()
        {
            spells.CommandReceiver.OnSpellCastRequest.DeregisterResponse();
            CancelExistingReduceCooldownsCoroutine();
        }
        private Nothing OnSpellCastRequest(SpellCastRequest request, ICommandCallerInfo callerinfo)
        {
            CastSpell(request.spellType, request.position.ToVector3());
            return new Nothing();
        }

        private void CancelExistingReduceCooldownsCoroutine()
        {
            if (reduceCooldownsCoroutine != null)
            {
                StopCoroutine(reduceCooldownsCoroutine);
                reduceCooldownsCoroutine = null;
            }
        }

        public void CastSpell(SpellType spellType, Vector3 position)
        {
            if (!spells.Data.canCastSpells || spells.Data.cooldowns[spellType] > 0f)
            {
                return;
            }
            var targetCount = FindSpellTargetEntities(position);
            ApplySpellEffectOnTargets(spellType, targetCount);
            spells.Send(new Spells.Update().AddSpellAnimationEvent(new SpellAnimationEvent(spellType, position.ToCoordinates())));
            SetSpellCooldown(spellType, SimulationSettings.SpellCooldown);
        }

        private int FindSpellTargetEntities(Vector3 position)
        {
            return Physics.OverlapCapsuleNonAlloc(position, position + Vector3.up * 10f, SimulationSettings.PlayerSpellAOEDiameter * 0.5f, spellTargets);
        }

        private void ApplySpellEffectOnTargets(SpellType spellType, int targetCount)
        {
            for (var spellTargetIndex = 0; spellTargetIndex < targetCount; spellTargetIndex++)
            {
                var targetEntityId = spellTargets[spellTargetIndex].gameObject.EntityId();
				if (targetEntityId.IsValid())
                {
                    switch (spellType)
                    {
                        case SpellType.LIGHTNING:
                            SpatialOS.Commands.SendCommand(spells, Flammable.Commands.Ignite.Descriptor, new Nothing(), targetEntityId);
                            break;
                        case SpellType.RAIN:
                            SpatialOS.Commands.SendCommand(spells, Flammable.Commands.Extinguish.Descriptor, new ExtinguishRequest(true), targetEntityId);
                            break;
                    }
                }
            }
        }

        private void SetSpellCooldown(SpellType spellType, float value)
        {
            var cooldowns = new Map<SpellType, float>(spells.Data.cooldowns);
            cooldowns[spellType] = value;
            spells.Send(new Spells.Update().SetCooldowns(cooldowns));
        }

        private void ReduceCooldowns()
        {
            var cooldowns = new Map<SpellType, float>(spells.Data.cooldowns);
            var enumerator = cooldowns.Keys.GetEnumerator();
            var componentNeedsUpdate = false;
            while (enumerator.MoveNext())
            {
                if (cooldowns[enumerator.Current] > 0f)
                {
                    cooldowns[enumerator.Current] = Mathf.Max(cooldowns[enumerator.Current] - 1f, 0f);
                    componentNeedsUpdate = true;
                }
            }
            if (componentNeedsUpdate)
            {
                spells.Send(new Spells.Update().SetCooldowns(cooldowns));
            }
        }
    }
}
