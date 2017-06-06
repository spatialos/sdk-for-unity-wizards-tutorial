using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class GameNotificationsPanelController : MonoBehaviour
    {
        [SerializeField] private Text text;
        private static GameNotificationsPanelController instance;
        
        private void Awake()
        {
            instance = this;
        }

        public static void SetText(string t)
        {
            instance.text.text = t;
        }
    }
}
