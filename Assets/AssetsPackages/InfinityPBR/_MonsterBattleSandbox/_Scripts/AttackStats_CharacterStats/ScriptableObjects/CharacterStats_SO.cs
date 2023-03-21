using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewStats", menuName = "Character/Stats", order = 1)]

public class CharacterStats_SO : ScriptableObject
{
    #region Fields

    public bool statsSetManually = true;

    #region ItemPickUps
    //public ItemPickUp weapon { get; private set; }
    //public ItemPickUp headArmor { get; private set; }
    //public ItemPickUp chestArmor { get; private set; }
    //public ItemPickUp handArmor { get; private set; }
    //public ItemPickUp legArmor { get; private set; }
    //public ItemPickUp footArmor { get; private set; }
    //public ItemPickUp misc1 { get; private set; }
    //public ItemPickUp misc2 { get; private set; }
    #endregion

    [Space]
    [Header("SPAWN PREFABS")]
    [Space]
    public GameObject unitPrefab;
    public GameObject monsterPrefab;
    [Space]
    [Header("SPAWN UI/MANAGER DATA")]
    [Space]
    public string characterName;
    public string characterSpecies;
    public string characterClass;
    public Color spawnUIColor;

    [Space]
    public Sprite characterPortrait;
    public Vector3 portraitOffset;

    [Space]
    [Header("CHARACTER DATA")]
    [Space]
    public AttackDefinition baseAttack = null;
    public AttackDefinition specialAttack = null;
    public float speed = 1;
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int maxMana = 0;
    public int currentMana = 0;

    [HideInInspector] public int baseDamage = 0;
    [HideInInspector] public int currentDamage = 0;

    [HideInInspector] public float baseResistance = 0f;
    [HideInInspector] public float currentResistance = 0f;

    #region Wealth, Encumbrance, Experience
    //public int currentWealth = 0;
    //public int maxWealth = 0;
    //public float currentEncumbrance = 0f;
    //public float maxEncumbrance = 0f;
    //public int characterExperience = 0;
    //public int characterLevel = 0;
    #endregion

    [System.Serializable]
    public class CharacterLevelUps
    {
        public int maxHealth;
        public int maxMana;
        public int baseDamage;
        public float baseResistance;
    }

    [Space]
    public CharacterLevelUps[] characterLevelUps;

    #endregion

    #region Stat Increasers
    public void ApplyHealth(int healthAmount)
    {
        if((currentHealth + healthAmount) > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += healthAmount;
        }
    }

    public void ApplyMana(int manaAmount)
    {
        if((currentMana + manaAmount) > maxMana)
        {
            currentMana = maxMana;
        }
        else
        {
            currentMana += manaAmount;
        }
    }

    #region Equipping

    //public void EquipWeapon
    //    (ItemPickUp weaponPickUp, CharacterInventory characterInventory, GameObject weaponSlot)
    //{
    //    #region (Inventory Code Not Needed)
    //    //Rigidbody newWeapon;
    //    //weapon = weaponPickUp;
    //    //characterInventory.inventoryDisplaySlots[2].sprite = weaponPickUp.itemDefinition.itemIcon;
    //    //newWeapon = Instantiate(weaponPickUp.itemDefinition.weaponSlotObject.weaponRigidbody, weaponSlot.transform);
    //    //currentDamage = baseDamage + weapon.itemDefinition.itemAmount;
    //    #endregion

    //    currentDamage = baseDamage;
    //}

    //public void EquipArmor(ItemPickUp armorPickup, CharacterInventory characterInventory)
    //{
    //    switch(armorPickup.itemDefinition.itemArmorSubType)
    //    {
    //        case ItemArmorSubType.Head:
    //            headArmor = armorPickup;
    //            currentResistance += armorPickup.itemDefinition.itemAmount;
    //            break;
    //        case ItemArmorSubType.Chest:
    //            chestArmor = armorPickup;
    //            currentResistance += armorPickup.itemDefinition.itemAmount;
    //            break;
    //        case ItemArmorSubType.Hands:
    //            handArmor = armorPickup;
    //            currentResistance += armorPickup.itemDefinition.itemAmount;
    //            break;
    //        case ItemArmorSubType.Legs:
    //            legArmor = armorPickup;
    //            currentResistance += armorPickup.itemDefinition.itemAmount;
    //            break;
    //        case ItemArmorSubType.Feet:
    //            footArmor = armorPickup;
    //            currentResistance += armorPickup.itemDefinition.itemAmount;
    //            break;
    //    }
    //}

    #endregion

    #endregion
    
    #region Stat Decreasers
    public void TakeDamage (int amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0)
        {
            Death();
        }
    }

    public void TakeMana(int amount)
    {
        currentMana -= amount;
        if (currentMana < 0)
        {
            currentMana = 0;
        }
    }

    #region Unequipping

    //public bool UnequipWeapon(ItemPickUp weaponPickUp, CharacterInventory characterInventory, GameObject weaponSlot)
    //{
    //    bool previousWeaponSame = false;
    //    if (weapon != null)
    //    {
    //        if (weapon == weaponPickUp)
    //        {
    //            previousWeaponSame = true;
    //        }
    //    //DestroyObject(weaponSlot.transform.GetChild(0).gameObject);
    //    weapon = null;
    //    currentDamage = baseDamage;
    //    }
    //    return previousWeaponSame;
    //}

    //public bool UnequipArmor(ItemPickUp armorPickUp, CharacterInventory characterInventory)
    //{
    //    bool previousArmorSame = false;
    //    switch(armorPickUp.itemDefinition.itemArmorSubType)
    //    {
    //        case ItemArmorSubType.Head:
    //            if(headArmor != null)
    //            {
    //                if(headArmor == armorPickUp)
    //                {
    //                    previousArmorSame = true;
    //                }
    //                currentResistance -= armorPickUp.itemDefinition.itemAmount;
    //                headArmor = null;
    //            }
    //            break;
    //        case ItemArmorSubType.Chest:
    //            if (chestArmor != null)
    //            {
    //                if (chestArmor == armorPickUp)
    //                {
    //                    previousArmorSame = true;
    //                }
    //                currentResistance -= armorPickUp.itemDefinition.itemAmount;
    //                chestArmor = null;
    //            }
    //            break;
    //        case ItemArmorSubType.Hands:
    //            if (handArmor != null)
    //            {
    //                if (handArmor == armorPickUp)
    //                {
    //                    previousArmorSame = true;
    //                }
    //                currentResistance -= armorPickUp.itemDefinition.itemAmount;
    //                handArmor = null;
    //            }
    //            break;
    //        case ItemArmorSubType.Legs:
    //            if (legArmor != null)
    //            {
    //                if (legArmor == armorPickUp)
    //                {
    //                    previousArmorSame = true;
    //                }
    //                currentResistance -= armorPickUp.itemDefinition.itemAmount;
    //                legArmor = null;
    //            }
    //            break;
    //        case ItemArmorSubType.Feet:
    //            if (footArmor != null)
    //            {
    //                if (footArmor == armorPickUp)
    //                {
    //                    previousArmorSame = true;
    //                }
    //                currentResistance -= armorPickUp.itemDefinition.itemAmount;
    //                footArmor = null;
    //            }
    //            break;
    //    }

    //    return previousArmorSame;
    //}
    #endregion

    #endregion

    #region Character Level Up and Death
    private void Death()
    {
        
    }

    #region LevelUps
    //private void LevelUp()
    //{
    //    characterLevel += 1;
    //    // display levelup visualization

    //    maxHealth = characterLevelUps[characterLevel - 1].maxHealth;
    //    maxMana = characterLevelUps[characterLevel - 1].maxMana;
    //    // maxWealth = characterLevelUps[characterLevel - 1].maxWealth;
    //    baseDamage = characterLevelUps[characterLevel - 1].baseDamage;
    //    // baseResistance = characterLevelUps[characterLevel - 1].baseResistance;
    //    // maxEncumbrance = characterLevelUps[characterLevel - 1].maxEncumbrance;
    //}
    #endregion
    #endregion
}