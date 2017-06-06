using Assets.Gamelogic.Core;
using Assets.Gamelogic.UI;
using Assets.Gamelogic.Utils;
using Improbable.Core;
using Improbable.Fire;
using Improbable.Life;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Gamelogic.Player
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class PlayerInfoVisualizer : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer clientAuthorityCheck;
        [Require] private PlayerInfo.Reader playerInfo;
        [Require] private Health.Reader health;
        [Require] private Flammable.Reader flammable;

        private float healthLocalCopy;

        [SerializeField] private CharacterModelVisualizer characterModelVisualizer;

        private void Awake()
        {
            characterModelVisualizer = gameObject.GetComponentIfUnassigned(characterModelVisualizer);
        }

        private void OnEnable()
        {
            playerInfo.ComponentUpdated.Add(OnPlayerInfoUpdated);
            MainCameraController.SetTarget(gameObject);
            UIController.ShowUI();
            SceneManager.UnloadSceneAsync(SimulationSettings.SplashScreenScene);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        private void OnDisable()
        {
            playerInfo.ComponentUpdated.Remove(OnPlayerInfoUpdated);
        }

        private void OnPlayerInfoUpdated(PlayerInfo.Update update)
        {
            if (update.isAlive.HasValue)
            {
                switch (update.isAlive.Value)
                {
                    case true:
                        Resurrect();
                        break;
                    case false:
                        Die();
                        break;
                }
            }
        }

        private void Resurrect()
        {
            GameNotificationsPanelController.SetText("");
            characterModelVisualizer.SetModelVisibility(true);
        }

        private void Die()
        {
            GameNotificationsPanelController.SetText("You have died.");
        }
    }
}
