using Assets.Gamelogic.Core;
using Improbable.Communication;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Communication
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class ChatVisualizer : MonoBehaviour
    {
        [Require] private Chat.Reader chat;

        [SerializeField] private NotificationController notification;

        private void Awake()
        {
            notification = gameObject.GetComponentCachedInChildren(notification);
            if (notification == null)
            {
                Debug.LogWarning("No notification controller!");
            }
            else
            {
                notification.HideNotification();
            }
        }

        private void OnEnable()
        {
            chat.ComponentUpdated.Add(ComponentUpdated);
        }

        private void OnDisable()
        {
            chat.ComponentUpdated.Remove(ComponentUpdated);

            if (notification != null)
            {
                notification.HideNotification();
            }
        }

        private void ComponentUpdated(Chat.Update update)
        {
            var lastIndex = update.chatSent.Count - 1;
            notification.ShowNotification(update.chatSent[lastIndex].message);
        }
    }
}
