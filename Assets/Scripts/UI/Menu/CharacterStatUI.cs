using UnityEngine;

namespace LostRunes
{
    public class CharacterStatUI : MonoBehaviour
    {
        [SerializeField] CharacterStats _stats;

        [Header("Character name")]
        [SerializeField] CustomButton _characterName;

        [Header("Character Level")]
        [SerializeField] CustomButton _currentLevel;
        [SerializeField] CustomButton _remainingAttributePoints;
        [SerializeField] CustomButton _currentExp;
        [SerializeField] CustomButton _expThreshold;

        [Header("Character Attribute")]
        [SerializeField] CustomButton _health;
        float _currentHealth = 0f;
        float _maxHealth = 0f;
        [SerializeField] CustomButton _mana;
        float _currentMana = 0f;
        float _maxMana = 0f;
        [SerializeField] CustomButton _stamina;
        float _currentStamina = 0f;
        float _maxStamina = 0f;

        [Header("Character Stats")]
        [SerializeField] CustomButton _vitality;
        [SerializeField] CustomButton _endurance;
        [SerializeField] CustomButton _constitution;
        [SerializeField] CustomButton _strength;
        [SerializeField] CustomButton _dexterity;
        [SerializeField] CustomButton _intelligence;
        [SerializeField] CustomButton _faith;
        [SerializeField] CustomButton _luck;

        [Header("Character Damages")]
        [SerializeField] CustomButton _physicalDamage;
        [SerializeField] CustomButton _magicDamage;
        [SerializeField] CustomButton _fireDamage;
        [SerializeField] CustomButton _thunderDamage;
        [SerializeField] CustomButton _iceDamage;
        [SerializeField] CustomButton _holyDamage;
        [SerializeField] CustomButton _darkDamage;
        [SerializeField] CustomButton _poisonDamage;
        [SerializeField] CustomButton _bleedDamage;

        [Header("Character Damages Reduction")]
        [SerializeField] CustomButton _physicalDamageReduction;
        [SerializeField] CustomButton _magicDamageReduction;
        [SerializeField] CustomButton _fireDamageReduction;
        [SerializeField] CustomButton _thunderDamageReduction;
        [SerializeField] CustomButton _iceDamageReduction;
        [SerializeField] CustomButton _holyDamageReduction;
        [SerializeField] CustomButton _darkDamageReduction;
        [SerializeField] CustomButton _poisonDamageReduction;
        [SerializeField] CustomButton _bleedDamageReduction;

        [Header("Character defenses")]
        [SerializeField] CustomButton _physicalDefense;
        [SerializeField] CustomButton _magicDefense;
        [SerializeField] CustomButton _fireDefense;
        [SerializeField] CustomButton _thunderDefense;
        [SerializeField] CustomButton _iceDefense;
        [SerializeField] CustomButton _holyDefense;
        [SerializeField] CustomButton _darkDefense;
        [SerializeField] CustomButton _poisonDefense;
        [SerializeField] CustomButton _bleedDefense;
        public void Initialize(CharacterStats stats)
        {
            if (stats == null) { return; }

            _stats = stats;

            InitializeCharacterName();
            InitializeCharacterLevel();

            InitializeAttributes();

            InitializeStats(stats);
        }

        private void InitializeStats(CharacterStats stats)
        {
            _vitality.ButtonText.text = stats._vitalityStat.TotalLevel.ToString();

            _endurance.ButtonText.text = stats._enduranceStat.TotalLevel.ToString();

            _constitution.ButtonText.text = stats._constitutionStat.TotalLevel.ToString();

            _strength.ButtonText.text = stats._strengthStat.TotalLevel.ToString();

            _dexterity.ButtonText.text = stats._dexterityStat.TotalLevel.ToString();

            _intelligence.ButtonText.text = stats._intelligenceStat.TotalLevel.ToString();

            _faith.ButtonText.text = stats._faithStat.TotalLevel.ToString();

            _luck.ButtonText.text = stats._luckStat.TotalLevel.ToString();
        }

        private void InitializeAttributes()
        {
            Health health = _stats._health;
            _currentHealth = health.Remaining;
            _maxHealth = health.Max;
            health._healthChanged += UpdateRemainingHealth;
            health._maxHealthChanged += UpdateMaxHealth;
            UpdateHealthText();

            Mana mana = _stats._mana;
            _currentMana = mana.Remaining;
            _maxMana = mana.Max;
            mana._manaChanged += UpdateRemainingMana;
            mana._maxManaChanged += UpdateMaxMana;
            UpdateManaText();

            Stamina stamina = _stats._stamina;
            _currentStamina = stamina.Remaining;
            _maxStamina = stamina.Max;
            stamina._staminaChanged += UpdateRemainingStamina;
            stamina._maxStaminaChanged += UpdateMaxStamina;
            UpdateStaminaText();
        }

        private void UpdateStaminaText()
        {
            _stamina.ButtonText.text = ((int)_currentStamina).ToString() + " / " + ((int)_maxStamina).ToString();
        }

        private void UpdateMaxStamina(float maxStamina)
        {
            _maxStamina = maxStamina;
            UpdateStaminaText();
        }

        private void UpdateRemainingStamina(float stamina)
        {
            _currentStamina =stamina;
            UpdateStaminaText();
        }

        private void UpdateManaText()
        {
            _mana.ButtonText.text = ((int)_currentMana).ToString() + " / " + ((int)_maxMana).ToString();
        }

        private void UpdateMaxMana(float maxMana)
        {
            _maxMana = maxMana;
            UpdateManaText();
        }

        private void UpdateRemainingMana(float mana)
        {
            _currentMana = mana;
            UpdateManaText();
        }

        private void UpdateHealthText()
        {
            _health.ButtonText.text = ((int)_currentHealth).ToString() + " / " + ((int)_maxHealth).ToString();
        }

        private void UpdateMaxHealth(float maxHealth)
        {
            _maxHealth = maxHealth;
            UpdateHealthText();
        }

        private void UpdateRemainingHealth(float health)
        {
            _currentHealth = health;
            UpdateHealthText();
        }

        private void InitializeCharacterLevel()
        {
            CharacterLevel level = _stats._characterLevel;
            _currentLevel.ButtonText.text = level._currentLevel.ToString();
            _remainingAttributePoints.ButtonText.text = level._remainingAttributePoints.ToString();
            _currentExp.ButtonText.text = ((int)level._currentExp).ToString();
            _expThreshold.ButtonText.text = ((int)level._expThreshold).ToString();

            _stats._characterLevel.CharacterLevelUp += UpdateLevel;
        }

        private void InitializeCharacterName()
        {
            _characterName.ButtonText.text = _stats._characterName;
        }

        void UpdateLevel(int level, int remainingAttributePoints, float currentExp, float expThreshold)
        {
            _currentLevel.ButtonText.text = level.ToString();
            _remainingAttributePoints.ButtonText.text = remainingAttributePoints.ToString();
            _currentExp.ButtonText.text = ((int)currentExp).ToString();
            _expThreshold.ButtonText.text = ((int)expThreshold).ToString();
        }
    }
}