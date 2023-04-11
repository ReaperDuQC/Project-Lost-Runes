using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostRunes.SaveSystem;
using LostRunes.Menu;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;

namespace LostRunes
{
    public class GameManager : NetworkBehaviour
    {
        static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }

        [SerializeField] GameMenu _gameMenu;
        public GameMenu GameMenu { get { return _gameMenu; } }

        [SerializeField] CharacterCreator _characterCreator;

        [SerializeField] HealthBar _healthBar;
        public HealthBar HealthBar { get { return _healthBar; } }
        [SerializeField] ManaBar _manaBar;
        public ManaBar ManaBar { get { return _manaBar; } }
        [SerializeField] StaminaBar _staminaBar;
        public StaminaBar StaminaBar { get { return _staminaBar; } }

        [SerializeField] CharacterStatUI _characterStatUI;
        public CharacterStatUI CharacterStatUI { get { return _characterStatUI; } }

        [SerializeField] bool _needOnline;
        private void Awake()
        {
            if (_needOnline)
            {
                if (NetworkManager.Singleton == null)
                {
                    SceneManager.LoadScene("Intro");
                    return;
                }
            }

            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
    }
}