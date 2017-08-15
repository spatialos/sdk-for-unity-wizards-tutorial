using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable.Unity.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class SplashScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject NotReadyWarning;
        [SerializeField] private Button ConnectButton;
        [SerializeField] private GameObject Spinner;

        public void AttemptToConnect()
        {
            DisableConnectButton();

            Spinner.SetActive(true);
            NotReadyWarning.SetActive(false);

            AttemptConnection();
        }

        private void DisableConnectButton()
        {
            ConnectButton.interactable = false;
        }

        private void AttemptConnection()
        {
            Bootstrap bootstrap = FindObjectOfType<Bootstrap>();
            if (!bootstrap)
            {
                throw new Exception("Couldn't find Bootstrap script on GameEntry in UnityScene");
            }
            bootstrap.ConnectToClient();
            StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.ClientConnectionTimeoutSecs, ConnectionTimeout));
        }

        private void ConnectionTimeout()
        {
            if (SpatialOS.IsConnected)
            {
                SpatialOS.Disconnect();
            }

            NotReadyWarning.SetActive(true);
            Spinner.SetActive(false);
            ConnectButton.interactable = true;
        }
    }
}

