using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class EquipmentUIManager : MonoBehaviour
    {
        [Header("Equip Gear")]
        [SerializeField] EquipmentSlotUI _headGearSlot;
        [SerializeField] EquipmentSlotUI chestGearSlot;
        [SerializeField] EquipmentSlotUI _glovesGearSlot;
        [SerializeField] EquipmentSlotUI _pantsGearSlot;
        [SerializeField] EquipmentSlotUI _bootsGearSlot;
        [Header("Weapon Gear")]
        [SerializeField] EquipmentSlotUI _leftWeapon1Slot;
        [SerializeField] EquipmentSlotUI _leftWeapon2Slot;
        [SerializeField] EquipmentSlotUI _rightWeapon1Slot;
        [SerializeField] EquipmentSlotUI _rightWeapon2Slot;
        [Header("Magic")]
        [SerializeField] EquipmentSlotUI _magicSlot;
        [Header("Consumable")]
        [SerializeField] EquipmentSlotUI _consumable1Slot;
        [SerializeField] EquipmentSlotUI _consumable2Slot;
        [SerializeField] EquipmentSlotUI _consumable3Slot;
        [SerializeField] EquipmentSlotUI _consumable4Slot;
        [SerializeField] EquipmentSlotUI _consumable5Slot;

        [SerializeField] QuickSlotUIManager _quickSlotManager;
    }
}