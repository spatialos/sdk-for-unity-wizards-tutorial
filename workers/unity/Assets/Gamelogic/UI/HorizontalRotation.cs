using Assets.Gamelogic.Core;
using UnityEngine;

namespace Assets.Gamelogic.UI
{
    public class HorizontalRotation : MonoBehaviour
    {
        private float RotationSpeed = SimulationSettings.SpellIndicatorRotationSpeed;

        void Update()
        {
            transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
        }
    }
}
