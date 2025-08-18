using Diablo.InventorySystem.Inventory;
using Diablo.InventorySystem.Items;
using System;
using System.Data;
using UnityEngine;

namespace Diablo.InventorySystem.Character
{
    public class PlayerEquipment : MonoBehaviour
    {
        public InventoryObject equipment;

        //[Header("Equipment Transforms")]
        //[SerializeField]
        //private Transform leftShieldTransform;
        //[SerializeField]
        //private Transform leftWeaponTransform;
        //[SerializeField]
        //private Transform rightWeaponTransform;

        private EquipmentCombiner combiner;

        private ItemInstances[] itemInstances = new ItemInstances[8];

        [Header("Default Equipments: H = 0, A = 1, G = 2, W = 3, P = 4, S = 5")]
        public ItemObject[] defaultItemObjects = new ItemObject[8];

        // Use this for initialization
        void Awake()
        {
            combiner = new EquipmentCombiner(gameObject);

            for (int i = 0; i < equipment.Slots.Length; ++i)
            {
                equipment.Slots[i].OnPreUpdate += OnRemoveItem;
                equipment.Slots[i].OnPostUpdate += OnEquipItem;
            }
        }

        private void Start()
        {
            foreach (InventorySlot slot in equipment.Slots)
            {
                OnEquipItem(slot);
            }
        }

        private void OnEquipItem(InventorySlot slot)
        {
            ItemObject itemObject = slot.ItemObject;
            if (itemObject == null)
            {
                EquipDefaultItemBy(slot.AllowedItems[0]);
                return;
            }

            int index = (int)slot.AllowedItems[0];

            switch (slot.AllowedItems[0])
            {
                case ItemType.Helmet:
                case ItemType.Armor:
                case ItemType.Gloves:
                case ItemType.Pants:
                case ItemType.Shoes:
                    itemInstances[index] = EquipSkinnedItem(itemObject);
                    break;
                case ItemType.Weapon:
                    itemInstances[index] = EquipMeshItem(itemObject);
                    break;
            }

            if (itemInstances[index] != null)
            {
                itemInstances[index].name = slot.AllowedItems[0].ToString();
            }
        }

        private ItemInstances EquipSkinnedItem(ItemObject itemObject)
        {
            if (itemObject == null)
            {
                return null;
            }

            Transform itemTransform = combiner.AddLimb(itemObject.modelPrefab, itemObject.boneNames);

            ItemInstances instances = itemTransform.gameObject.AddComponent<ItemInstances>();
            if (instances != null)
            {
                instances.items.Add(itemTransform);
            }

            return instances;
        }

        private ItemInstances EquipMeshItem(ItemObject itemObject)
        {
            if (itemObject == null)
            {
                return null;
            }

            Transform[] itemTransforms = combiner.AddMesh(itemObject.modelPrefab);
            if (itemTransforms.Length > 0)
            {
                ItemInstances instances = new GameObject().AddComponent<ItemInstances>();
                foreach (Transform t in itemTransforms)
                {
                    instances.items.Add(t);
                }

                instances.transform.parent = transform;

                return instances;
            }

            return null;
        }

        private void EquipDefaultItemBy(ItemType type)
        {
            int index = (int)type;

            ItemObject itemObject = defaultItemObjects[index];
            switch (type)
            {
                case ItemType.Helmet:
                case ItemType.Armor:
                case ItemType.Gloves:
                    itemInstances[index] = EquipSkinnedItem(itemObject);
                    break;
                case ItemType.Weapon:
                    itemInstances[index] = EquipMeshItem(itemObject);
                    break;
                case ItemType.Pants:
                case ItemType.Shoes:
                    break;
            }
        }

        private void RemoveItemBy(ItemType type)
        {
            int index = (int)type;
            if (itemInstances[index] != null)
            {
                Destroy(itemInstances[index].gameObject);
                itemInstances[index] = null;
            }
        }

        private void OnRemoveItem(InventorySlot slot)
        {
            ItemObject itemObject = slot.ItemObject;
            if (itemObject == null)
            {
                // destroy deafult items
                RemoveItemBy(slot.AllowedItems[0]);
                return;
            }

            if (slot.ItemObject.modelPrefab != null)
            {
                RemoveItemBy(slot.AllowedItems[0]);
            }
        }
    }
}