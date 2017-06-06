using System.Collections;
using UnityEngine;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable.Abilities;
using Improbable.Fire;
using Improbable.Unity;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Player
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class PlayerAnimController : MonoBehaviour
    {
        [Require] private Spells.Reader spells;
        [Require] private Flammable.Reader flammable;

        private Vector3 lastPosition;

        [SerializeField] private Animator anim;
        [SerializeField] private ParticleSystem CastAnim;
        [SerializeField] private GameObject playerModel;

        private void Awake()
        {
            anim = gameObject.GetComponentIfUnassigned(anim);
            anim.enabled = true;
        }

        private void OnEnable()
        {
            flammable.ComponentUpdated.Add(FlammableUpdated);
            lastPosition = transform.position;
        }

        private void OnDisable()
        {
            flammable.ComponentUpdated.Remove(FlammableUpdated);
        }

        private void FlammableUpdated(Flammable.Update update)
        {
            if (update.isOnFire.HasValue)
            {
                anim.SetBool("OnFire", update.isOnFire.Value);
            }
        }

        private void Update()
        {
            float movementTargetDistance = (lastPosition - transform.position).magnitude;
            float animSpeed = Mathf.Min(1, movementTargetDistance / SimulationSettings.PlayerMovementTargetSlowingThreshold);
            anim.SetFloat("ForwardSpeed", animSpeed);
            lastPosition = transform.position;
        }

        public void AnimateSpellCast()
        {
            CastAnim.Play();
            anim.SetTrigger("CastLightning");
            StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.PlayerCastAnimationTime, CancelCastAnim));
        }

        private void CancelCastAnim()
        {
            CastAnim.Stop();
        }

        public void SetModelVisibility(bool isVisible)
        {
            playerModel.SetActive(isVisible);
        }
    }
}
