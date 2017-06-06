using UnityEngine;

namespace Assets.Gamelogic.Core
{
    public static class GameObjectComponentCaching
    {
        public static T GetComponentIfUnassigned<T>(this GameObject obj, T componentReference)
        {
            if (componentReference == null)
            {
                componentReference = obj.GetComponent<T>();
                if (componentReference == null)
                {
                    Debug.LogError("Failed to get component reference on " + obj.name);
                }
                else
                {
                    Debug.LogWarning("Component reference " + componentReference + " on " + obj.name + " wasn't serialized but recovered via GetComponent call");
                }
            }
            return componentReference;
        }

        public static T[] GetComponentsIfUnassigned<T>(this GameObject obj, T[] componentReferences)
        {
            if (componentReferences == null || componentReferences.Length == 0)
            {
                componentReferences = obj.GetComponents<T>();
                if (componentReferences.Length == 0)
                {
                    Debug.LogError("Failed to get component references on " + obj.name);
                }
            }
            return componentReferences;
        }

        public static T GetComponentCachedInChildren<T>(this GameObject obj, T componentReference)
        {
            if (componentReference == null)
            {
                componentReference = obj.GetComponentInChildren<T>();
                if (componentReference == null)
                {
                    Debug.LogError("Failed to get child component reference on " + obj.name);
                }
            }
            return componentReference;
        }
    }
}

