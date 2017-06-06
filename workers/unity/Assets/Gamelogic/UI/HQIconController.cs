using Assets.Gamelogic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class HQIconController : MonoBehaviour
    {
        [SerializeField] private Image hqIcon;

        public void SetTeamId(int teamId)
        {
            hqIcon.color = SimulationSettings.TeamColors[teamId];
        }

        public void SetHQHealth(float fill)
        {
            hqIcon.fillAmount = fill;
        }
    }
}
