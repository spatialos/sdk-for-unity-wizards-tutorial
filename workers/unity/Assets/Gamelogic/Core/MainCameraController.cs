using UnityEngine;

namespace Assets.Gamelogic.Core
{
    class MainCameraController : MonoBehaviour
    {
        private static MainCameraController instance;
        private static GameObject targetPlayer;

        private float distance;
        private float yaw;
        private float pitch;
        private Vector3 cameraEulerRotation = Vector3.zero;
        private Quaternion playerToCameraRotation = Quaternion.identity;
        private Vector3 cameraTargetPosition;

        private void Awake()
        {
            distance = SimulationSettings.CameraDefaultDistance;
            pitch = SimulationSettings.CameraDefaultPitch;
            if (instance != null)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        private void Update()
        {
            if (targetPlayer != null)
            {
                UpdatePlayerToCameraRotation();
            }
        }

        private void LateUpdate()
        {
            if (targetPlayer != null)
            {
                MoveCamera();
                UpdateCameraRotation();
            }
        }

        public static void SetTarget(GameObject t)
        {
            targetPlayer = t;
        }

        private void UpdatePlayerToCameraRotation()
        {
            var distanceChange = distance - Input.GetAxis("Mouse ScrollWheel") * SimulationSettings.CameraDistanceSensitivity;
            distance = Mathf.Clamp(distanceChange, SimulationSettings.CameraMinDistance, SimulationSettings.CameraMaxDistance);
            if (Input.GetMouseButton(SimulationSettings.RotateCameraMouseButton))
            {
                yaw = (yaw + Input.GetAxis("Mouse X") * SimulationSettings.CameraSensitivity) % 360f;
                pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * SimulationSettings.CameraSensitivity, SimulationSettings.CameraMinPitch, SimulationSettings.CameraMaxPitch);
            }
            cameraEulerRotation.x = pitch;
            cameraEulerRotation.y = yaw;
            playerToCameraRotation = Quaternion.Euler(cameraEulerRotation);
        }

        private void MoveCamera()
        {
            cameraTargetPosition = targetPlayer.transform.position + playerToCameraRotation * Vector3.back * distance;
            Camera.main.transform.position = cameraTargetPosition;
        }

        private void UpdateCameraRotation()
        {
            Camera.main.transform.LookAt(targetPlayer.transform.position);
        }
    }
}
