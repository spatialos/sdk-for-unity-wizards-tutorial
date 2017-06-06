using UnityEngine;

namespace Assets.Gamelogic.UI
{
    public class UIController : MonoBehaviour
    {
        private static UIController instance;

        private void Awake()
        {
            instance = this;
            HideUI();
        }

        public static void ShowUI()
        {
            instance.gameObject.SetActive(true);
        }

        public static void HideUI()
        {
            instance.gameObject.SetActive(false);
        }
    }
}

