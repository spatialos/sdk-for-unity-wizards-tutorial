using Assets.Gamelogic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class SpellsPanelController : MonoBehaviour
    {
        [SerializeField] private Image lightningCircleIcon;
        [SerializeField] private Image rainCircleIcon;
        [SerializeField] private Image lightningSpellIcon;
        [SerializeField] private Image rainSpellIcon;
        private static SpellsPanelController instance;
        
        private void Awake()
        {
            instance = this;
        }

        public static void SetLightningIconFill(float fill)
        {
            instance.lightningCircleIcon.fillAmount = fill;
            instance.lightningSpellIcon.color = fill < 1f ? SimulationSettings.TransparentWhite : Color.white;
        }

        public static void SetRainIconFill(float fill)
        {
            instance.rainCircleIcon.fillAmount = fill;
            instance.rainSpellIcon.color = fill < 1f ? SimulationSettings.TransparentWhite : Color.white;
        }
    }
}
