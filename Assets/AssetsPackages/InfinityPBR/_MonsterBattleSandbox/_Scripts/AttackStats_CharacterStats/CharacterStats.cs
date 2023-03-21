using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ---About CharacterStats---
    
    This script is mostly from the course "Swords and Shovels: Combat System."

    https://learn.unity.com/project/swords-and-shovels-combat-system

    To simplify things, I took out everything related to inventory and item pick-ups. 

    CharacterStats collects info from the character scriptable object data.

    It provides shortened accessability to other scripts: 
    for example, charStats.GetHealth()

    CharacterStats also sends out events related to OnHealthUpdated (useful for UI display),
    OnTakeDamage, OnCharacterDeath. These will be useful for triggering audio and effects.

    - characterDefinition is for an individual character. 
    - characterDefinition_Template is for a type of enemy. 

    An instance of the template is created so that the changes (for example, 
    damage to health) do not happen on all characters.
    
*/

public class CharacterStats : MonoBehaviour
{
    public CharacterStats_SO characterDefinition_Template;
    public CharacterStats_SO characterDefinition;
    private GameObject characterWeaponSlot;
    private Rigidbody characterRigidbody;
    public Rigidbody GetRigidbody() => characterRigidbody;

    [SerializeField] private Transform aimAtPoint = null;
    public Transform GetAimAtPoint() => aimAtPoint;

    [SerializeField] private Transform spellSpawnPoint = null;
    public Transform GetSpellSpawnPoint() => spellSpawnPoint;

    [SerializeField] private bool isDead = false;
    public bool IsDead => isDead;

    public event Action<int> OnHealthUpdated;
    public event Action OnTakeDamage;
    public event Action OnCharacterDeath;

    #region Constructors
    public CharacterStats()
    {
        
        //characterInventory = CharacterInventory.instance;
    }
    #endregion

    #region Initializations
    private void Awake()
    {
        isDead = false;

        if (characterDefinition_Template != null)
            characterDefinition = Instantiate(characterDefinition_Template);

        #region AutoFill Settings
        if(!characterDefinition.statsSetManually)
        {
            characterDefinition.maxHealth = 100;
            characterDefinition.currentHealth = 50;

            characterDefinition.maxMana = 25;
            characterDefinition.currentMana = 10;

            //characterDefinition.maxWealth = 500;
            //characterDefinition.currentWealth = 0;

            //characterDefinition.baseResistance = 0;
            //characterDefinition.currentResistance = 0;

            //characterDefinition.maxEncumbrance = 50f;
            //characterDefinition.currentEncumbrance = 0f;

            //characterDefinition.characterExperience = 0;
            //characterDefinition.characterLevel = 1;
        }
        #endregion
    }

    private void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
    }
    #endregion

    #region Updates
    private void Update()
    {

    }

    #endregion

    #region Stat Increasers
    //Wrapping SO methods here to shorten calling them
    public void ApplyHealth (int healthAmount)
    {
        characterDefinition.ApplyHealth(healthAmount);
    }

    public void ApplyMana(int manaAmount)
    {
        characterDefinition.ApplyMana(manaAmount);
    }

    //public void GiveWealth(int wealthAmount)
    //{
    //    characterDefinition.GiveWealth(wealthAmount);
    //}
    #endregion

    #region Stat Reducers
    public void TakeDamage(int amount)
    {
        if (isDead == true) { return; }
        characterDefinition.TakeDamage(amount);
        HandleHealthUpdated(GetHealth());
        if (GetHealth() <= 0 && isDead == false)
        {
            isDead = true;
            Debug.Log("<b> \u2620"
                + characterDefinition.characterSpecies 
                + " DEATH!"
                + " \u2620 </b>");
            OnCharacterDeath?.Invoke();
        }
        else
        {
            OnTakeDamage?.Invoke();
        }
    }

    public void TakeMana(int amount)
    {
        if (isDead == true) { return; }
        characterDefinition.TakeMana(amount);
    }
    #endregion

    #region Weapon and Armor Change
    //public void ChangeWeapon(ItemPickUp weaponPickUp)
    //{
    //    // Boolean return type, the statement is executed
    //    if (!characterDefinition.UnequipWeapon(weaponPickUp, characterInventory, characterWeaponSlot))
    //    {
    //        characterDefinition.EquipWeapon(weaponPickUp, characterInventory, characterWeaponSlot);
    //    }
    //}

    //public void ChangeArmor(ItemPickUp armorPickUp)
    //{
    //    if (!characterDefinition.UnequipArmor(armorPickUp, characterInventory))
    //    {
    //        characterDefinition.EquipArmor(armorPickUp, characterInventory);
    //    }

    //}
    #endregion

    #region Reporters

    private void HandleHealthUpdated(int newHealth)
    {
        OnHealthUpdated?.Invoke(newHealth);
    }
    public int GetHealth()
    {
        return characterDefinition.currentHealth;
    }

    public int GetMaxHealth()
    {
        return characterDefinition.maxHealth;
    }

    public AttackDefinition GetBaseAttack()
    {
        return characterDefinition.baseAttack;
    }

    public AttackDefinition GetSpecialAttack()
    {
        return characterDefinition.specialAttack;
    }

    public float GetAttackRange()
    {
        return characterDefinition.baseAttack.range;
    }

    public float GetAttackRate()
    {
        return characterDefinition.baseAttack.attackRate;
    }

    public float GetSpecialAttackRange()
    {
        return characterDefinition.specialAttack.range;
    }

    public float GetSpecialAttackRate()
    {
        return characterDefinition.specialAttack.attackRate;
    }

    public float GetSpecialAttackForce()
    {
        return characterDefinition.specialAttack.force;
    }

    public float GetNavAgentSpeed()
    {
        return characterDefinition.speed;
    }

    public int GetDamage()
    {
        return characterDefinition.baseDamage;
    }

    public float GetResistance()
    {
        return characterDefinition.currentResistance;
    }

    public string GetSpecies()
    {
        return characterDefinition.characterSpecies;
    }

    public string GetClassType()
    {
        return characterDefinition.characterClass;
    }

    public Color GetCharacterColor()
    {
        return characterDefinition.spawnUIColor;
    }

    #endregion
}
