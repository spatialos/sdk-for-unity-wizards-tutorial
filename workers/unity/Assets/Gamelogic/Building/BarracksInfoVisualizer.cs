using Improbable.Building;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Building
{
    public class BarracksInfoVisualizer : MonoBehaviour
    {
        [Require] BarracksInfo.Reader barracksInfo;
        public BarracksState BarracksState { get { return barracksInfo.Data.barracksState; } }
    }
}
