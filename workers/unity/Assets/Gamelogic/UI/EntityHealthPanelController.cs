using Assets.Gamelogic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class EntityHealthPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject entityHealthPanel;
        [SerializeField] private Image entityHealthIcon;

        public void Show()
        {
            entityHealthPanel.SetActive(true);
        }

        public void Hide()
        {
            entityHealthPanel.SetActive(false);
        }

        public void SetHealth(float fill)
        {
            entityHealthIcon.fillAmount = fill;
        }

        public void SetHealthColorFromTeam(uint teamId)
        {
            entityHealthIcon.GetComponent<Image>().color = SimulationSettings.TeamColors[teamId];
        }
    }
}
