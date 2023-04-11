using LostRunes.Menu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    [System.Serializable]
    public class CharacterStatsData 
    {
        public string _characterName = "";

        public bool _isMale = true;

        public int _characterLevel = 1;
        public int _remainingAttributePoints = 0;
        public float _currentExp = 0f;

        public float _remainingHealth = 0f;
        public float _remainingMana = 0f;
        public float _remainingStamina = 0f;

        public int _healthBaseLevel = 10;
        public int _enduranceBaseLevel = 10;
        public int _constitutionBaseLevel = 10;
        public int _strengthBaseLevel = 10;
        public int _dexterityBaseLevel = 10;
        public int _intelligenceBaseLevel = 10;
        public int _faithBaseLevel = 10;
        public int _luckBaseLevel = 100;

        public CharacterStatsData() { }
        public void SetRolledStats(List<int> stats)
        {
            _characterLevel = 1;

            _healthBaseLevel = stats[0];
            _enduranceBaseLevel = stats[1];
            _constitutionBaseLevel = stats[2];
            _strengthBaseLevel = stats[3];
            _dexterityBaseLevel = stats[4];
            _intelligenceBaseLevel = stats[5];
            _faithBaseLevel = stats[6];
            _luckBaseLevel = stats[7];
        }
        public CharacterStatsData(CharacterStats stats)
        {
            _healthBaseLevel = stats._vitalityStat.BaseLevel;
            _enduranceBaseLevel = stats._enduranceStat.BaseLevel;
            _constitutionBaseLevel = stats._constitutionStat.BaseLevel;
            _strengthBaseLevel = stats._strengthStat.BaseLevel;
            _dexterityBaseLevel = stats._dexterityStat.BaseLevel;
            _intelligenceBaseLevel = stats._intelligenceStat.BaseLevel;
            _faithBaseLevel = stats._faithStat.BaseLevel;
            _luckBaseLevel = stats._luckStat.BaseLevel;
        }
    }

    [CreateAssetMenu(fileName = "CharacterData", menuName = "LostRunes/Character/CharacterData")]
    public class CharacterStats : ScriptableObject
    {
        public string _characterName = "";
        public bool _isMale = true;

        [Header("Level")]
        public CharacterLevel _characterLevel;

        [Header("Attributes")]
        public Health _health;
        public Mana _mana;
        public Stamina _stamina;

        [Header("Stats")]
        public VitalityStat _vitalityStat;
        public EnduranceStat _enduranceStat;
        public ConstitutionStat _constitutionStat;
        public StrengthStat _strengthStat;
        public DexterityStat _dexterityStat;
        public IntelligenceStat _intelligenceStat;
        public FaithStat _faithStat;
        public LuckStat _luckStat;

        [Header("Defenses")]
        public CharacterDefense _characterDefense = new();

        [Header("Skills")]
        public List<int> _skillIds = new();
        public List<Skill> _skills = new(); // need to apply skills on initialize

        public void InitializeCharacter(PlayerData data, Transform transform)
        {
            _characterName = data._statData._characterName;
            _isMale = data._statData._isMale;

            _characterLevel = new CharacterLevel(data._statData._characterLevel, data._statData._remainingAttributePoints, data._statData._currentExp);

            _vitalityStat = new VitalityStat(data._statData._healthBaseLevel);
            _enduranceStat =  new EnduranceStat( data._statData._enduranceBaseLevel);
            _constitutionStat = new ConstitutionStat( data._statData._constitutionBaseLevel);
            _strengthStat = new StrengthStat( data._statData._strengthBaseLevel);
            _dexterityStat = new DexterityStat( data._statData._dexterityBaseLevel);
            _intelligenceStat = new IntelligenceStat( data._statData._intelligenceBaseLevel);
            _faithStat = new FaithStat( data._statData._faithBaseLevel);
            _luckStat = new LuckStat( data._statData._luckBaseLevel);

            _characterDefense.Initialize(this);

            // Apply Skills Effects

            // Initialize health, mana, and stamina
            _health = new Health();
            _health.Initialize(this);
            _mana = new Mana();
            _mana.Initialize(this); 
            _stamina = new Stamina();
            _stamina.Initialize(this);

            


            transform.position = data.GetPosition();
            transform.rotation = data.GetRotation();
        }
    }
}