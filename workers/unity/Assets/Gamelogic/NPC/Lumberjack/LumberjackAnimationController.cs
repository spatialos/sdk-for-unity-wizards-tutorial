using System;
using Assets.Gamelogic.Core;
using Improbable.Core;
using Improbable.Fire;
using Improbable.Npc;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Lumberjack
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class LumberjackAnimationController : MonoBehaviour
    {
        [Require] private NPCLumberjack.Reader npcLumberjack;
        [Require] private TargetNavigation.Reader targetNavigation;
        [Require] private Flammable.Reader flammable;
        [Require] private Inventory.Reader inventory;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Animator anim;
        [SerializeField] private ParticleSystem WoodChipsAnimation;
        [SerializeField] private GameObject HandSocket;
        [SerializeField] private GameObject Axe;
        [SerializeField] private GameObject Log;
        [SerializeField] private AudioClip[] ChoppingSounds;
        [SerializeField] private int cachedResourcesCount;
        [SerializeField] private LumberjackFSMState.StateEnum cachedFsmState;

        private void Awake()
        {
            audioSource = gameObject.GetComponentIfUnassigned(audioSource);
            anim = gameObject.GetComponentIfUnassigned(anim);
        }

        private void OnEnable()
        {
            npcLumberjack.ComponentUpdated.Add(OnNpcLumberjackComponentUpdate);
            targetNavigation.ComponentUpdated.Add(NavigationUpdated);
            flammable.ComponentUpdated.Add(FlammableUpdated);
            inventory.ComponentUpdated.Add(OnInventoryUpdated);
            ResetAllLocalState();
            SetAnimationState(npcLumberjack.Data.currentState);
            SetForwardSpeed(TargetNavigationBehaviour.IsInTransit(targetNavigation));
        }

        private void OnDisable()
        {
            npcLumberjack.ComponentUpdated.Remove(OnNpcLumberjackComponentUpdate);
            targetNavigation.ComponentUpdated.Remove(NavigationUpdated);
            inventory.ComponentUpdated.Remove(OnInventoryUpdated);
            flammable.ComponentUpdated.Remove(FlammableUpdated);
        }

        public void OnAxeConnect()
        {
            WoodChipsAnimation.Play();
            PlayRandomSound(ChoppingSounds);
        }

        private void OnInventoryUpdated(Inventory.Update update)
        {
            if (update.resources.HasValue)
            {
                cachedResourcesCount = update.resources.Value;
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

        private void OnNpcLumberjackComponentUpdate(NPCLumberjack.Update newState)
        {
            if (newState.currentState.HasValue)
            {
                if (cachedFsmState != newState.currentState.Value)
                {
                    cachedFsmState = newState.currentState.Value;
                    SetAnimationState(cachedFsmState);
                }
            }
            
        }

        private void SetAnimationState(LumberjackFSMState.StateEnum currentState)
        {
            StopCurrentAnimation();
            switch (currentState)
            {
                case LumberjackFSMState.StateEnum.STOCKPILING:
                    anim.SetBool("Dropping", true);
                    break;
                case LumberjackFSMState.StateEnum.HARVESTING:
                    anim.SetBool("Chopping", true);
                    Axe.SetActive(true);
                    HandSocket.GetComponent<FixedJoint>().connectedBody = Axe.GetComponent<Rigidbody>();
                    break;
                case LumberjackFSMState.StateEnum.MOVING_TO_TARGET:
                    var isCarryingLog = cachedResourcesCount > 0;

                    anim.SetBool("Carrying", isCarryingLog);
                    anim.SetBool("Walking", !isCarryingLog);
                    Log.SetActive(isCarryingLog);

                    HandSocket.GetComponent<FixedJoint>().connectedBody = Log.GetComponent<Rigidbody>();
                    break;
            }
        }

        private void StopCurrentAnimation()
        {
            anim.SetBool("Dropping", false);
            anim.SetBool("Chopping", false);
            anim.SetBool("Carrying", false);
            anim.SetBool("Walking", false);
            Axe.SetActive(false);
            Log.SetActive(false);
        }

        private void PlayRandomSound(AudioClip[] choppingSounds)
        {
            int soundToPlay = new System.Random().Next(choppingSounds.Length);
            audioSource.PlayOneShot(choppingSounds[soundToPlay]);
        }

        private void ResetAllLocalState()
        {
            anim.SetBool("Dropping", false);
            anim.SetBool("Chopping", false);
            anim.SetBool("Carrying", false);
            anim.SetBool("Walking", false);
            anim.SetBool("OnFire", false);
            Axe.SetActive(false);
            Log.SetActive(false);
            cachedResourcesCount = 0;
            cachedFsmState = LumberjackFSMState.StateEnum.IDLE;
        }
    }
}
