using System;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    [System.Serializable]
    public class CharacterLevel 
    {
        public delegate void OnCharacterLevelUp(int level, int remainingAttributePoints, float currentExp, float expThreshold);
        public OnCharacterLevelUp CharacterLevelUp;

        public Action OnLevelUp;

        public int _currentLevel = 1;
        public int _remainingAttributePoints = 0;

        public float _currentExp = 0;
        public float _expThreshold = 0f;


        public List<float> _expReceivedModifier;
        // bonus attributes point modifier
        // expthreshold modifier

        public CharacterLevel(int currentLevel, int remainingAttributePoint, float currentExp)
        {
            _currentLevel = currentLevel;
            _remainingAttributePoints = remainingAttributePoint;
            _currentExp = currentExp;
            SetExpThresholdForCharacterLevel();
        }

        public void ReceiveExperience(float exp)
        {
            float bonusExp = 0f;

            foreach(float modifier in _expReceivedModifier)
            {
                bonusExp += modifier * exp;
            }
            float totalExpGained = exp + bonusExp;
            _currentExp += totalExpGained;

            if(_currentExp >= _expThreshold)
            {
                _currentExp = _currentExp - _expThreshold;

                LevelUp();
            }
        }
        public void LevelUp()
        {
            _currentLevel++;
            _remainingAttributePoints++;

            SetExpThresholdForCharacterLevel();
            OnLevelUp?.Invoke();
            CharacterLevelUp?.Invoke(_currentLevel, _remainingAttributePoints, _currentExp, _expThreshold);
        }
        private void SetExpThresholdForCharacterLevel()
        {
            _expThreshold = CalculateExpRequired(_currentLevel + 1);
        }

        public float CalculateExpRequired(int currentLevel)
        {
            // Calculate XP required using the formula XP Required = 0.0725(Level - 1)^3 + 10(Level - 1)^2 + 10(Level - 1)
            float expRequired = 0.0725f * Mathf.Pow((currentLevel - 1f), 3f) + 10f * Mathf.Pow((currentLevel - 1f), 2f) + 10f * (currentLevel - 1f);

            // Round the XP required to the nearest integer and return it
            return expRequired;
        }
    }
}