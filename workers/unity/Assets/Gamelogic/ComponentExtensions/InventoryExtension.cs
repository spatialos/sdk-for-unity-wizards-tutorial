using Improbable.Core;
using UnityEngine;

namespace Assets.Gamelogic.ComponentExtensions
{
    public static class InventoryExtension
    {
        public static bool HasResources(this Inventory.Reader inventory)
        {
            return inventory.Data.resources > 0;
        }

        public static void AddToInventory(this Inventory.Writer inventory, int quantity)
        {
            inventory.Send(new Inventory.Update().SetResources(inventory.Data.resources + quantity));
        }

        public static void RemoveFromInventory(this Inventory.Writer inventory, int quantity)
        {
            inventory.Send(new Inventory.Update().SetResources(Mathf.Max(0, inventory.Data.resources - quantity)));
        }
    }
}
