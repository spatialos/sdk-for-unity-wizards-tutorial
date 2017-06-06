using System.Collections;
using System.Collections.Generic;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Player;
using Assets.Gamelogic.UI;
using Assets.Gamelogic.Utils;
using Improbable.Abilities;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Abilities
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class SpellsRequester : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer clientAuthorityCheck;
        [Require] private Spells.Reader spells;
        
        private GameObject spellAOEIndicatorInstance;
        public bool SpellCastingModeActive;
        private SpellType activeSpell;
        private Vector3 spellTargetPosition;
        private IDictionary<SpellType, float> spellCooldownsLocalCopy = new Dictionary<SpellType, float> { { SpellType.LIGHTNING, 0f }, { SpellType.RAIN, 0f } };
        private RaycastHit hit;
        private Ray ray;

        [SerializeField] private PlayerInputListener playerInputListener;
        [SerializeField] private PlayerAnimController playerAnimController;
        [SerializeField] private SpellcastAudioTriggers spellcastAudioTriggers;

        private void Awake()
        {
            playerInputListener = gameObject.GetComponentIfUnassigned(playerInputListener);
            playerAnimController = gameObject.GetComponentIfUnassigned(playerAnimController);
            spellcastAudioTriggers = gameObject.GetComponentIfUnassigned(spellcastAudioTriggers);
        }

        private void OnEnable()
        {
            CreateSpellAOEIndicatorInstance();
        }

        private void OnDisable()
        {
            Destroy(spellAOEIndicatorInstance);
        }

        private void CreateSpellAOEIndicatorInstance()
        {
            spellAOEIndicatorInstance = Instantiate(ResourceRegistry.SpellAOEIndicatorPrefab);
            spellAOEIndicatorInstance.transform.localScale = new Vector3(SimulationSettings.PlayerSpellAOEDiameter, 1f, SimulationSettings.PlayerSpellAOEDiameter);
            UpdateSpellAOEIndicatorVisibility();
        }

        private void UpdateSpellAOEIndicatorVisibility()
        {
            spellAOEIndicatorInstance.SetActive(SpellCastingModeActive);
        }
        
        public void ActivateSpellCastingMode(SpellType spellType)
        {
            SpellCastingModeActive = true;
            activeSpell = spellType;
            UpdateSpellTargetPosition();
            UpdateSpellAOEIndicatorVisibility();
        }

        public void DeactivateSpellCastingMode()
        {
            SpellCastingModeActive = false;
            UpdateSpellAOEIndicatorVisibility();
        }

        public void AttemptToCastSpell()
        {
            if (!SpellCastingModeActive || spellCooldownsLocalCopy[activeSpell] > 0f)
            {
                return;
            }

            playerAnimController.AnimateSpellCast();
            spellcastAudioTriggers.TriggerSpellChannelAudio();
            DisableMovementForSpellcast();

            StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.PlayerCastAnimationBuffer, CastSpell));
        }

        private void CastSpell()
        {
            var spellCastRequest = new SpellCastRequest(activeSpell, spellTargetPosition.ToCoordinates());
            SpatialOS.Commands.SendCommand(clientAuthorityCheck, Spells.Commands.SpellCastRequest.Descriptor, spellCastRequest, gameObject.EntityId());
            SetLocalSpellCooldown(activeSpell, SimulationSettings.SpellCooldown);
            DeactivateSpellCastingMode();
        }

        private void DisableMovementForSpellcast()
        {
            playerInputListener.DisableInputForSpellcast();
        }

        private void Update()
        {
            UpdateSpellTargetPosition();
            ReduceCooldowns(Time.deltaTime);
            UpdateSpellsPanelCooldowns();
        }

        private void UpdateSpellTargetPosition()
        {
            if (!SpellCastingModeActive)
            {
                return;
            }
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var layerMask = 1 << LayerMask.NameToLayer(SimulationSettings.TerrainLayerName);
            if (Physics.Raycast(ray, out hit, SimulationSettings.MaxRaycastDistance, layerMask))
            {
                spellTargetPosition = hit.point;
                var zFightOffset = new Vector3(0.0f, 0.1f, 0.0f);
                spellAOEIndicatorInstance.transform.position = hit.point + zFightOffset;
            }
        }

        private void SetLocalSpellCooldown(SpellType spellType, float value)
        {
            spellCooldownsLocalCopy[spellType] = value;
        }

        private void ReduceCooldowns(float deltaTime)
        {
            var enumerator = new List<SpellType>(spellCooldownsLocalCopy.Keys).GetEnumerator();
            while (enumerator.MoveNext())
            {
                spellCooldownsLocalCopy[enumerator.Current] = Mathf.Max(spellCooldownsLocalCopy[enumerator.Current] - deltaTime, 0f);
            }
        }

        private void UpdateSpellsPanelCooldowns()
        {
            SpellsPanelController.SetLightningIconFill(1f - spellCooldownsLocalCopy[SpellType.LIGHTNING] / SimulationSettings.SpellCooldown);
            SpellsPanelController.SetRainIconFill(1f - spellCooldownsLocalCopy[SpellType.RAIN] / SimulationSettings.SpellCooldown);
        }

        public float GetLocalSpellCooldown(SpellType spellType)
        {
            return spellCooldownsLocalCopy[spellType];
        }
    }
}
