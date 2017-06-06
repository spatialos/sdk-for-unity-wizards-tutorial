using Assets.Gamelogic.Core;

namespace Assets.Gamelogic.Utils
{
    public static class QuantizationUtils
    {
        public static uint QuantizeAngle(float angle)
        {
            return (uint)(angle * SimulationSettings.AngleQuantisationFactor);
        }

        public static float DequantizeAngle(uint angle)
        {
            return angle / SimulationSettings.AngleQuantisationFactor;
        }
    }
}
