using System.Collections;
using Assets.Gamelogic.Abilities;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.UI;
using Improbable.Abilities;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable.Communication;
using Assets.Gamelogic.Communication;

namespace Assets.Gamelogic.Player
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class PlayerInputListener : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer clientAuthorityCheck;
        [Require] private PlayerInfo.Reader playerInfo;

        private bool controlsEnabled;
        private Vector3 inputDirection = Vector3.zero;

        [SerializeField] private PlayerControlsSender playerControlsSender;
        [SerializeField] private SpellsRequester spellsBehaviour;

        private void Awake()
        {
            playerControlsSender = gameObject.GetComponentIfUnassigned(playerControlsSender);
            spellsBehaviour = gameObject.GetComponentIfUnassigned(spellsBehaviour);
        }

        private void OnEnable()
        {
            EnableControls();
            playerInfo.ComponentUpdated.Add(OnPlayerInfoUpdated);
        }

        private void OnDisable()
        {
            DisableControls();
            playerInfo.ComponentUpdated.Remove(OnPlayerInfoUpdated);
        }

        private void OnPlayerInfoUpdated(PlayerInfo.Update update)
        {
            if (update.isAlive.HasValue)
            {
                switch (update.isAlive.Value)
                {
                    case true:
                        EnableControls();
                        break;
                    case false:
                        DisableControls();
                        break;
                }
            }
        }

        public void EnableControls()
        {
            controlsEnabled = true;
        }

        public void DisableControls()
        {
            playerControlsSender.SetInputDirection(Vector3.zero);
            spellsBehaviour.DeactivateSpellCastingMode();
            controlsEnabled = false;
        }

        public void ActivateChat()
        {
            ChatPanelController.ActivateChatMode();
            spellsBehaviour.DeactivateSpellCastingMode();
        }

        public void DeactivateChat()
        {
            ChatPanelController.DeactivateChatMode();
        }

        private void Update()
        {
            UpdateMovementDirection();
            UpdateSpellControls();
            UpdateChatControls();
        }

        public void DisableInputForSpellcast()
        {
            playerControlsSender.SetInputDirection(Vector3.zero);
            enabled = false;
            StartCoroutine(EnableControlsAfter(SimulationSettings.PlayerCastAnimationTime));

        }

        private IEnumerator EnableControlsAfter(float playerCastAnimationTime)
        {
            yield return new WaitForSeconds(playerCastAnimationTime);
            enabled = true;
        }

        private void UpdateMovementDirection()
        {
            if (!controlsEnabled || ChatPanelController.ChatModeActive)
            {
                return;
            }
            inputDirection.x = Input.GetAxis("Horizontal");
            inputDirection.z = Input.GetAxis("Vertical");
            playerControlsSender.SetInputDirection(inputDirection);
        }

        private void UpdateSpellControls()
        {
            if (!controlsEnabled || ChatPanelController.ChatModeActive)
            {
                return;
            }
            if (Input.GetKeyDown(SimulationSettings.CastLightningKey))
            {
                if (!spellsBehaviour.SpellCastingModeActive && spellsBehaviour.GetLocalSpellCooldown(SpellType.LIGHTNING) <= 0f)
                {
                    spellsBehaviour.ActivateSpellCastingMode(SpellType.LIGHTNING);
                }
                else
                {
                    spellsBehaviour.DeactivateSpellCastingMode();
                }
            }
            if (Input.GetKeyDown(SimulationSettings.CastRainKey))
            {
                if (!spellsBehaviour.SpellCastingModeActive && spellsBehaviour.GetLocalSpellCooldown(SpellType.RAIN) <= 0f)
                {
                    spellsBehaviour.ActivateSpellCastingMode(SpellType.RAIN);
                }
                else
                {
                    spellsBehaviour.DeactivateSpellCastingMode();
                }
            }
            if (Input.GetMouseButtonDown(SimulationSettings.CastSpellMouseButton) && spellsBehaviour.SpellCastingModeActive) spellsBehaviour.AttemptToCastSpell();
            if (Input.GetKeyDown(SimulationSettings.AbortKey) && spellsBehaviour.SpellCastingModeActive) spellsBehaviour.DeactivateSpellCastingMode();
        }

        private void UpdateChatControls()
        {
            //Todo: Handle enabling and disabling ChatMode here!
            if (Input.GetKeyDown(KeyCode.UpArrow) && ChatPanelController.ChatModeActive) ChatPanelController.ReuseLastMessage();
            if (Input.GetKeyDown(SimulationSettings.AbortKey) && ChatPanelController.ChatModeActive) DeactivateChat();
        }
    }
}
