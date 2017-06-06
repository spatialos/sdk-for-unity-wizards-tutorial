using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class ChatPanelController : MonoBehaviour
    {
        private static string lastMessage = "";
        public static bool ChatModeActive;

        [SerializeField] private InputField chatInputField;
        private static ChatPanelController instance;
        
        private void Awake()
        {
            instance = this;
        }

        private void OnEnable()
        {
            DeactivateChatMode();
        }

        private void OnDisable()
        {
            DeactivateChatMode();
        }

        private void LateUpdate()
        {
            if (chatInputField.isFocused && !ChatModeActive)
            {
                ActivateChatMode();
            }
            if (!chatInputField.isFocused && ChatModeActive)
            {
                DeactivateChatMode();
            }
        }

        public static void ActivateChatMode()
        {
            instance.chatInputField.text = "";
            instance.chatInputField.ActivateInputField();
            ChatModeActive = true;
        }

        public static void DeactivateChatMode()
        {
            instance.chatInputField.text = "";
            instance.chatInputField.DeactivateInputField();
            ChatModeActive = false;
        }

        public static void ReuseLastMessage()
        {
            instance.chatInputField.text = lastMessage;
            instance.chatInputField.MoveTextEnd(false);
        }

        public static string SubmitChat()
        {
            var chatMessage = instance.chatInputField.text;
            lastMessage = chatMessage;
            DeactivateChatMode();
            return chatMessage;
        }
    }
}
