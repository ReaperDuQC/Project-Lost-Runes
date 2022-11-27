using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [Serializable]
    public class CharacterStatsData 
    {
        public string _characterName;
        public bool _isMale;

        public int _characterLevel = 1;

        public int _healthLevel = 10;
        public int _enduranceLevel = 10;
        public int _constitutionLevel = 10;
        public int _strengthLevel = 10;
        public int _dexterityLevel = 10;
        public int _intelligenceLevel = 10;
        public int _faithLevel = 10;
        public CharacterStatsData(string name, bool isMale,List<int> stats)
        {
            _characterName = name;
            _isMale = isMale;

            _characterLevel = 1;

            _healthLevel = stats[0];
            _enduranceLevel = stats[1];
            _constitutionLevel = stats[2];
            _strengthLevel = stats[3];
            _dexterityLevel = stats[4];
            _intelligenceLevel = stats[5];
            _faithLevel = stats[6];
        }
        public CharacterStatsData(CharacterStats stats)
        {
            _characterName = stats.name;

            _characterLevel = stats._characterLevel;

            _healthLevel = stats._healthLevel;
            _enduranceLevel = stats._enduranceLevel;
            _constitutionLevel = stats._constitutionLevel;
            _strengthLevel = stats._strengthLevel;
            _dexterityLevel = stats._dexterityLevel;
            _intelligenceLevel = stats._intelligenceLevel;
            _faithLevel = stats._faithLevel;
        }
    }
    public class CharacterStats : MonoBehaviour
    {
        public string _characterName;
        public bool _isMale;

        public int _characterLevel = 1;

        public int _healthLevel = 10;
        public int _enduranceLevel = 10;
        public int _constitutionLevel = 10;
        public int _strengthLevel = 10;
        public int _dexterityLevel = 10;
        public int _intelligenceLevel = 10;
        public int _faithLevel = 10;
        public void InitializeCharacter(CharacterStatsData data)
        {
            _characterName = data._characterName;
            _isMale = data._isMale;
            _characterLevel = data._characterLevel;

            _healthLevel = data._healthLevel;
            _enduranceLevel = data._enduranceLevel;
            _constitutionLevel = data._constitutionLevel;
            _strengthLevel = data._strengthLevel;
            _dexterityLevel = data._dexterityLevel;
            _intelligenceLevel = data._intelligenceLevel;
            _faithLevel = data._faithLevel;
        }
    }
}