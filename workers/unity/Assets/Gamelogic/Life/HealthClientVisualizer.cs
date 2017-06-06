using Assets.Gamelogic.UI;
using Improbable.Life;
using Improbable.Team;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Life
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class HealthClientVisualizer : MonoBehaviour
    {
        [Require] private Health.Reader health;
        [Require] private TeamAssignment.Reader teamAssigment;

        public int CurrentHealth { get { return health.Data.currentHealth; } }
        public int MaxHealth { get { return health.Data.maxHealth; } }
        private GameObject entityInfoCanvasInstance;
        private EntityHealthPanelController entityHealthPanelController;

        private void Awake()
        {
            entityInfoCanvasInstance = (GameObject) Instantiate(ResourceRegistry.EntityInfoCanvasPrefab, transform);
            Collider modelCollider = GetComponent<Collider>();
            if (modelCollider == null)
            {
                modelCollider = GetComponentInChildren<Collider>();
            }
            entityInfoCanvasInstance.transform.localPosition = (modelCollider != null) ? Vector3.up * (modelCollider.bounds.size.y + 1.5f) : Vector3.up * 3f;
            entityHealthPanelController = entityInfoCanvasInstance.GetComponent<EntityHealthPanelController>();
        }

        private void OnEnable()
        {
            UpdateEntityHealthPanel();
            entityHealthPanelController.SetHealthColorFromTeam(teamAssigment.Data.teamId);
            health.ComponentUpdated.Add(OnComponentUpdated);
        }

        private void OnDisable()
        {
            health.ComponentUpdated.Remove(OnComponentUpdated);
        }

        private void OnComponentUpdated(Health.Update update)
        {
            UpdateEntityHealthPanel();
        }

        private void UpdateEntityHealthPanel()
        {
            if (CurrentHealth == MaxHealth)
            {
                entityHealthPanelController.Hide();
            }
            else
            {
                entityHealthPanelController.Show();
                entityHealthPanelController.SetHealth(((float)CurrentHealth) / MaxHealth);
            }
        }
    }
}
