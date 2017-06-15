using Assets.Gamelogic.Core;
using Improbable.Fire;
using Improbable.Npc;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class WizardAnimationController : MonoBehaviour {

        [Require] private NPCWizard.Reader npcWizard;
        [Require] private TargetNavigation.Reader targetNavigation;
        [Require] private Flammable.Reader flammable;

        [SerializeField] private Animator anim;
        public ParticleSystem CastAnim;

        private void Awake()
        {
            anim = gameObject.GetComponentIfUnassigned(anim);
        }

        private void OnEnable()
        {
            npcWizard.ComponentUpdated.Add(StateUpdated);
            targetNavigation.ComponentUpdated.Add(NavigationUpdated);
            flammable.ComponentUpdated.Add(FlammableUpdated);
            ResetAllAnimationState();
            SetAnimationState(npcWizard.Data.currentState);
            SetForwardSpeed(TargetNavigationBehaviour.IsInTransit(targetNavigation));
        }

        private void OnDisable()
        {
            npcWizard.ComponentUpdated.Remove(StateUpdated);
            targetNavigation.ComponentUpdated.Remove(NavigationUpdated);
            flammable.ComponentUpdated.Remove(FlammableUpdated);
        }

        public void StateUpdated(NPCWizard.Update stateUpdate)
        {
            if (stateUpdate.currentState.HasValue)
            {
                SetAnimationState(stateUpdate.currentState.Value);
            }
        }

        private void NavigationUpdated(TargetNavigation.Update navigationUpdate)
        {
            if (navigationUpdate.navigationState.HasValue)
            {
                SetForwardSpeed(TargetNavigationBehaviour.IsInTransit(targetNavigation));
            }
        }

        private void FlammableUpdated(Flammable.Update update)
        {
            if (update.isOnFire.HasValue)
            {
                anim.SetBool("OnFire", update.isOnFire.Value);
            }
        }

        private void SetForwardSpeed(bool hasTarget)
        {
            if (hasTarget)
            {
                anim.SetFloat("ForwardSpeed", 1);
            }
            else
            {
                anim.SetFloat("ForwardSpeed", 0);
            }
        }


        private void SetAnimationState(WizardFSMState.StateEnum currentState)
        {
            if (currentState.Equals(WizardFSMState.StateEnum.ATTACKING_TARGET) ||
                currentState.Equals(WizardFSMState.StateEnum.DEFENDING_TARGET))
            {
                anim.SetTrigger("Casting");
                CastAnim.Play();
            }
            else
            {
                CastAnim.Stop();
            }
        }

        private void ResetAllAnimationState()
        {
            anim.SetBool("Casting", false);
            anim.SetBool("OnFire", false);
        }
    }
}
