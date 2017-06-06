using System.Collections;
using Assets.Gamelogic.Utils;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.Communication
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class NotificationController : MonoBehaviour
    {
        private Coroutine timeoutCoroutine;

        [SerializeField] private Canvas baseCanvas;
        [SerializeField] private Text notificationText;

        public void ShowNotification(string text)
        {
            CancelTimeout();

            notificationText.text = text;
            baseCanvas.gameObject.SetActive(true);

            timeoutCoroutine = StartCoroutine(TimerUtils.WaitAndPerform(2.0f, HideNotification));
        }

        public void HideNotification()
        {
            CancelTimeout();
            baseCanvas.gameObject.SetActive(false);
            notificationText.text = "";
        }

        private void CancelTimeout()
        {
            if (timeoutCoroutine != null)
            {
                StopCoroutine(timeoutCoroutine);
                timeoutCoroutine = null;
            }
        }
    }
}