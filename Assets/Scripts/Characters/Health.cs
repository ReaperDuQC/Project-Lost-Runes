using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class Health : MonoBehaviour
    {
        public delegate void OnHealthChange(int health);
        public OnHealthChange _onHealthChange;
        public delegate void OnDeath();
        public OnDeath _onDeath;

        int _maxHealth;
        int _remainingHealth;
        public int RemainingHealth 
        { 
            get { return _remainingHealth; } 
            set { _remainingHealth = Mathf.Clamp(_remainingHealth + value, 0, _maxHealth); } 
        }
        public void TakeDamage(int damage)
        {
            RemainingHealth -= damage;

            if (_onHealthChange != null)
            {
                _onHealthChange(_remainingHealth);
            }

            if (_remainingHealth <= 0)
            {
                Death();
            }
        }
        void Death()
        {
            if (_onDeath != null)
            {
                _onDeath();
            }
        }
    }
}