using Assets.Gamelogic.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Gamelogic.UI
{
    public class HQsPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject hqIconPrefab;
        private static HQsPanelController instance;
        private IList<HQIconController> teamHqToIcons = new List<HQIconController>();
        private uint localPlayerTeamId;

        private void Awake()
        {
            instance = this;
            PopulateHqIcons();
        }

        public static void SetLocalPlayerTeamId(uint teamId)
        {
            instance.localPlayerTeamId = teamId;
            AlignHqIcons();
        }

        public static void SetHQHealth(uint teamId, float health)
        {
            var hqIcon = instance.teamHqToIcons[(int)teamId];
            var hqController = hqIcon.GetComponent<HQIconController>();

            var normalizedHealth = health / SimulationSettings.HQMaxHealth;
            hqController.SetHQHealth(normalizedHealth);
            AlignHqIcons();
        }

        private static void SetupPlayerHqIcon(HQIconController hqIconController)
        {
            var hqIconRectTransform = hqIconController.GetComponent<RectTransform>();
            hqIconRectTransform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
            hqIconRectTransform.anchorMin = new Vector2(0.0f, 1.0f);
            hqIconRectTransform.anchorMax = new Vector2(0.0f, 1.0f);
            hqIconRectTransform.pivot = new Vector2(0.0f, 1.0f);
            hqIconRectTransform.anchoredPosition = Vector3.zero;
        }

        private static void SetupOpponentTeamsHqIcons(HQIconController hqIconController, int iconIndex)
        {
            var hqIconRectTransform = hqIconController.GetComponent<RectTransform>();
            var scaleAdjustment = 0.897f;
            hqIconRectTransform.anchoredPosition = new Vector3(-15.0f + (-iconIndex * hqIconRectTransform.rect.width * hqIconRectTransform.localScale.x * scaleAdjustment), -11.0f, 0.0f);
        }

        private static void SetupSingleOpponentTeamHqIcon(HQIconController hqIconController)
        {
            var hqIconRectTransform = hqIconController.GetComponent<RectTransform>();
            hqIconRectTransform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
            hqIconRectTransform.anchoredPosition = new Vector3(-6.0f, -5.0f, 0.0f);
        }

        private void PopulateHqIcons()
        {
            for (var index = 0; index < SimulationSettings.TeamCount; index++)
            {
                var rectTransform = GetComponent<RectTransform>();
                var hqIconController = ((GameObject)Instantiate(hqIconPrefab, rectTransform)).GetComponent<HQIconController>();
                teamHqToIcons.Add(hqIconController);
            }
            AlignHqIcons();
        }

        private static void AlignHqIcons()
        {
            var teamCount = SimulationSettings.TeamCount;

            var opponentTeamIndex = 0;
            for (var index = 0; index < teamCount; index++)
            {
                var hqIconController = instance.teamHqToIcons[index];

                var hqController = hqIconController.GetComponent<HQIconController>();
                hqController.SetTeamId(index);

                if (index == instance.localPlayerTeamId)
                {
                    SetupPlayerHqIcon(hqIconController);
                }
                else
                {
                    if (teamCount == 2)
                    {
                        SetupSingleOpponentTeamHqIcon(hqIconController);
                    }
                    else
                    {
                        SetupOpponentTeamsHqIcons(hqIconController, opponentTeamIndex);
                        opponentTeamIndex++;
                    }
                }
            }
        }
    }
}
