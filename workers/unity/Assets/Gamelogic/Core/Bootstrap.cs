using Improbable.Unity;
using Improbable.Unity.Configuration;
using Improbable.Unity.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Gamelogic.Core
{
    /// <summary>
    /// Manages the lifecycle of the connection to SpatialOS as a worker, such as connection and disconnection.
    /// </summary>
    public class Bootstrap : MonoBehaviour
    {
        public WorkerConfigurationData Configuration = new WorkerConfigurationData();

        private void Start()
        {
            SceneManager.LoadScene(SimulationSettings.SplashScreenScene, LoadSceneMode.Additive);

            SpatialOS.ApplyConfiguration(Configuration);

            Time.fixedDeltaTime = 1.0f / SimulationSettings.FixedFramerate;

            switch (SpatialOS.Configuration.WorkerPlatform)
            {
                case WorkerPlatform.UnityWorker:
                    Application.targetFrameRate = SimulationSettings.TargetFramerateFSim;
                    SpatialOS.OnDisconnected += reason => Application.Quit();
                    SpatialOS.Connect(gameObject);
                    break;
                case WorkerPlatform.UnityClient:
                    Application.targetFrameRate = SimulationSettings.TargetFramerate;
                    SpatialOS.OnConnected += ClientPlayerSpawner.SpawnPlayer;
                    break;
            }
        }

        public void ConnectToClient()
        {
            SpatialOS.Connect(gameObject);
        }
    }
}
