using Assets.Gamelogic.Abilities;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Abilities;
using Improbable.Npc;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard
{
    public class WizardAttackingState : FsmBaseState<WizardStateMachine, WizardFSMState.StateEnum>
    {
        private readonly SpellsBehaviour spellsBehaviour;
        private readonly WizardBehaviour parentBehaviour;

        private Coroutine spellCastFinishDelayCoroutine;

        public WizardAttackingState(WizardStateMachine owner,
                                    WizardBehaviour inParentBehaviour,
                                    SpellsBehaviour inSpellsBehaviour) 
            : base(owner)
        {
            parentBehaviour = inParentBehaviour;
            spellsBehaviour = inSpellsBehaviour;
        }

        public override void Enter()
        {
            CastLightningOnTarget();
        }

        public override void Tick()
        {
        }

        public override void Exit(bool disabled)
        {
            StopSpellCastFinishDelayCoroutine();
        }

        private void StopSpellCastFinishDelayCoroutine()
        {
            if (spellCastFinishDelayCoroutine != null)
            {
                parentBehaviour.StopCoroutine(spellCastFinishDelayCoroutine);
                spellCastFinishDelayCoroutine = null;
            }
        }

        private void CastLightningOnTarget()
        {
            var targetGameObject = NPCUtils.GetTargetGameObject(Owner.Data.targetEntityId);
            if (targetGameObject == null)
            {
                Owner.TriggerTransition(WizardFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
                return;
            }
            if (NPCUtils.IsTargetAttackable(parentBehaviour.gameObject, targetGameObject))
            {
                spellsBehaviour.CastSpell(SpellType.LIGHTNING, targetGameObject.transform.position);
            }
            spellCastFinishDelayCoroutine = parentBehaviour.StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.PlayerCastAnimationTime, () => {
                Owner.TriggerTransition(WizardFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition);
            }));
        }
    }
}
