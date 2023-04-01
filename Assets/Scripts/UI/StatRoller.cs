using LostRunes.Menu;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AudioSettings = LostRunes.Menu.AudioSettings;

namespace LostRunes
{
    public class StatRoller : MonoBehaviour
    {
        public delegate void OnActionDone();
        public OnActionDone _allStatRolled;
        public OnActionDone _continuePressed;
        public OnActionDone _rollAllStatsPressed;

        [SerializeField] TextMeshProUGUI[] _statsLevelText;
        [SerializeField] TextMeshProUGUI[] _statsText;

        [SerializeField] CustomButton _currentStatButton;
        [SerializeField] CustomButton _autoRollButton;
        [SerializeField] CustomButton _startButton;
        [SerializeField] int _minStatValue = 1;
        [SerializeField] int _maxStatValue = 20;

        PlayerControls _playerControls;

        [SerializeField] float _autoRollInterval = 0.5f;
        [SerializeField] float _colorLerpDuration = 1.5f;

        AudioSource _audioSource;

        [SerializeField] AudioClip _lowStatSound;
        [SerializeField] AudioClip _highStatSound;
        [SerializeField] AudioClip _statSound;

        int _currentStatIndex = 0;
        int _baseLuckValue = 100;
        bool _autoRoll = false;
        List<int> _statLevels;

        [SerializeField] CharacterCreator _characterCreator;
        [SerializeField] GameMenu _gameMenu;
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            InitializeStats();
        }
        private void Start()
        {
            _gameMenu.OptionMenuUI.AudioSettings.SubscribeToSfxAudioSource(_audioSource);
        }
        private void OnEnable()
        {
            if (_playerControls == null)
            {
                _playerControls = new PlayerControls();
                _playerControls.MainMenu.Select.performed += i => _continuePressed();
                //_playerControls.MainMenu.AutoRoll.performed += i => _rollAllStatsPressed();
            }
            _playerControls.Enable();

            if (_continuePressed == null)
            {
                _continuePressed += SelectStat;
            }
            if(_rollAllStatsPressed == null)
            {
                _rollAllStatsPressed += AutoRollAllStats;
            }

            if (_allStatRolled == null)
            {
                _allStatRolled += OnAllStatsRolled;
            }
            UpdateCurrentStatText();
        }
        private void OnDisable()
        {
            _playerControls.Disable();
        }
        void Update()
        {
            for (int i = _currentStatIndex; i < _statsText.Length; i++)
            {
                RandomizeStatValue(i);
            }
        }
        void InitializeStats()
        {
            _statLevels = new List<int>();

            for (int i = 0; i < _statsText.Length; i++)
            {
                _statLevels.Add(_minStatValue);
            }
            _statLevels.Add(_baseLuckValue);
        }
        void RandomizeStatValue(int currentIdx)
        {
            _statLevels[currentIdx] = Random.Range(_minStatValue, _maxStatValue);
            _statsLevelText[currentIdx].text = _statLevels[currentIdx].ToString();
        }

        private void UpdateLuckStat(int currentIdx)
        {
            //int rolledStat = _statLevels[currentIdx];

            //int bonus = rolledStat == _minStatValue ? 1 : 0;
            //bonus = rolledStat == _maxStatValue ? -1 : bonus;

            //int valueToAdd = -(rolledStat - 10) + bonus;

            //_statLevels[^1] += valueToAdd;
            //_statsLevelText[^1].text = _statLevels[^1].ToString();
        }

        void SelectStat()
        {
            if (_autoRoll) return;
            RollStat();
        }
        public void RollStat()
        {
            if (_currentStatIndex >= _statsText.Length) return;

            RandomizeStatValue(_currentStatIndex);
            StartCoroutine(UpdateColor(_currentStatIndex));
            UpdateLuckStat(_currentStatIndex);
            PlaySound(_currentStatIndex);
            _currentStatIndex++;
            UpdateCurrentStatText();

            if (_currentStatIndex >= _statsText.Length)
            {
                _continuePressed -= SelectStat;

                if (_allStatRolled != null)
                {
                    _allStatRolled();
                    _allStatRolled = null;
                }
            }
        }
        public void AutoRollAllStats()
        {
            if (_autoRoll) return;
            _rollAllStatsPressed -= AutoRollAllStats;
            _currentStatButton.gameObject.SetActive(false);
            _autoRollButton.interactable = false;
            StartCoroutine(AutoRoll());
        }
        IEnumerator AutoRoll()
        {
            _autoRoll = true;
            while (_currentStatIndex < _statsLevelText.Length)
            {
                yield return new WaitForSeconds(_autoRollInterval);
                RollStat();
            }
        }
        void UpdateCurrentStatText()
        {
            if (_currentStatIndex >= _statsText.Length) return;
            string statName = _statsText[_currentStatIndex].text.Replace(":", "");

            _currentStatButton.ButtonText.text = "Roll " + statName ;
        }
        void DisableControlText()
        {
            _currentStatButton.gameObject.SetActive(false);
            _autoRollButton.gameObject.SetActive(false);
        }
        IEnumerator UpdateColor(int currentStatIndex)
        {
            Color initialColor = _statsLevelText[currentStatIndex].color;
            Color finalColor = new Color(1f - (float)_statLevels[currentStatIndex] / (float)_maxStatValue, (float)_statLevels[currentStatIndex] / (float)_maxStatValue, 0f);
            float currentDuration = 0;
            while (currentDuration < _colorLerpDuration)
            {
                yield return null;
                currentDuration += Time.deltaTime;
                float ratio = currentDuration / _colorLerpDuration;
                Color color =
                _statsLevelText[currentStatIndex].color = Color.Lerp(initialColor, finalColor, ratio);
            }
        }
        void OnAllStatsRolled()
        {
            DisableControlText();
            _startButton.gameObject.SetActive(true);    
        }
        void PlaySound(int currentStatIndex)
        {
            if (_audioSource == null) return;
            AudioSettings.RandomizePitch(_audioSource);

            AudioClip audioClip = SelectAudioClip(currentStatIndex);

            if (audioClip == null) return;

            _audioSource.clip = audioClip;
            _audioSource.Play();
        }
        AudioClip SelectAudioClip(int currentStatIndex)
        {
            if (_statLevels[currentStatIndex] <= _minStatValue) return _lowStatSound;
            
            if (_statLevels[currentStatIndex] < _maxStatValue) return _statSound;

            return _highStatSound;
        }
        public CharacterStatsData SaveStats()
        {
            return new CharacterStatsData(_statLevels);
        }
    }
}