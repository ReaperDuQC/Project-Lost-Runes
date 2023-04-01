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
        public int _characterLevel = 1;

        public int _healthBaseLevel = 10;
        public int _enduranceBaseLevel = 10;
        public int _constitutionBaseLevel = 10;
        public int _strengthBaseLevel = 10;
        public int _dexterityBaseLevel = 10;
        public int _intelligenceBaseLevel = 10;
        public int _faithBaseLevel = 10;
        public int _luckBaseLevel = 100;
        public CharacterStatsData(List<int> stats)
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
            _characterLevel = stats._characterLevel;

            _healthBaseLevel = stats._health.BaseLevel;
            _enduranceBaseLevel = stats._endurance.BaseLevel;
            _constitutionBaseLevel = stats._constitution.BaseLevel;
            _strengthBaseLevel = stats._strength.BaseLevel;
            _dexterityBaseLevel = stats._dexterity.BaseLevel;
            _intelligenceBaseLevel = stats._intelligence.BaseLevel;
            _faithBaseLevel = stats._faith.BaseLevel;
            _luckBaseLevel = stats._luck.BaseLevel;
        }
    }
    public class CharacterStats : ScriptableObject
    {
        public string _characterName;
        public bool _isMale;

        public int _characterLevel = 1;

        public int _currentExp = 0;
        public int _expThreshold = 10;
        public int _remainingAttributePoints = 0;

        public HealthAttribute _health;
        public EnduranceAttribute _endurance;
        public ConstitutionAttribute _constitution;
        public StrengthAttribute _strength;
        public DexterityAttribute _dexterity;
        public IntelligenceAttribute _intelligence;
        public FaithAttribute _faith;
        public LuckAttribute _luck;

        public void InitializeCharacter(PlayerData data, Transform transform)
        {
            _characterName = data._name;
            _isMale = data._gender == 0;
            _characterLevel = data._stats._characterLevel;

            _health = new HealthAttribute(data._stats._healthBaseLevel);
            _endurance =  new EnduranceAttribute( data._stats._enduranceBaseLevel);
            _constitution = new ConstitutionAttribute( data._stats._constitutionBaseLevel);
            _strength = new StrengthAttribute( data._stats._strengthBaseLevel);
            _dexterity = new DexterityAttribute( data._stats._dexterityBaseLevel);
            _intelligence = new IntelligenceAttribute( data._stats._intelligenceBaseLevel);
            _faith = new FaithAttribute( data._stats._faithBaseLevel);
            _luck = new LuckAttribute( data._stats._luckBaseLevel);

            transform.position = data.GetPosition();
            transform.rotation = data.GetRotation();
        }
    }
}