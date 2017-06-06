using Assets.Gamelogic.Utils;
using UnityEngine;

namespace Assets.Gamelogic.UI
{
    public class BillboardRotation : MonoBehaviour
    {
        public bool FixVerticalAxis = false;

        private void LateUpdate()
        {
            if (FixVerticalAxis)
            {
                transform.forward = Camera.main.transform.forward.FlattenVector();
            }
            else
            {
                transform.forward = Camera.main.transform.forward;
            }
        }
    }
}
