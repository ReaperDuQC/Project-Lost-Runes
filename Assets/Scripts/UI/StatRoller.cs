using LostRunes.Menu;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] TextMeshProUGUI _currentStatText;
        [SerializeField] TextMeshProUGUI _autoRollText;
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
        bool _autoRoll = false;
        List<int> _statLevels;

        bool _rollEnabled;

        [SerializeField] CharacterCreator _characterCreator;
        SceneTransition _sceneTransition;
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _sceneTransition = GetComponent<SceneTransition>();
            InitializeStats();
        }
        private void Start()
        {
            MainMenuUI.Instance.OptionMenuUI.AudioSettings.SubscribeToSfxAudioSource(_audioSource);
        }
        private void OnEnable()
        {
            if (_playerControls == null)
            {
                _playerControls = new PlayerControls();
                _playerControls.MainMenu.Select.performed += i => _continuePressed();
               // _playerControls.MainMenu.AutoRoll.performed += i => _rollAllStatsPressed();
            }
            _playerControls.Enable();

            _rollEnabled = false;

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
            for (int i = _currentStatIndex; i < _statsLevelText.Length; i++)
            {
                RandomizeStatValue(i);
            }
        }
        public void EnableStatRoll(bool enable)
        {
            _rollEnabled = enable;
        }
        void InitializeStats()
        {
            _statLevels = new List<int>();

            for (int i = 0; i < _statsLevelText.Length; i++)
            {
                _statLevels.Add(_minStatValue);
            }
        }
        void RandomizeStatValue(int currentIdx)
        {
            _statLevels[currentIdx] = Random.Range(_minStatValue, _maxStatValue);
            _statsLevelText[currentIdx].text = _statLevels[currentIdx].ToString();
        }
        void SelectStat()
        {
            if (!_rollEnabled) return;
            if (_autoRoll) return;
            RollStat();
        }
        public void RollStat()
        {
            if (_currentStatIndex >= _statsText.Length) return;

            RandomizeStatValue(_currentStatIndex);
            StartCoroutine(UpdateColor(_currentStatIndex));
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
            if (!_rollEnabled) return;

            if (_autoRoll) return;
            _rollAllStatsPressed -= AutoRollAllStats;
            _currentStatText.gameObject.SetActive(false);
            _autoRollText.GetComponent<Button>().interactable = false;
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

            _currentStatText.text = "Roll " + statName + "Level";
        }
        void DisableControlText()
        {
            _currentStatText.gameObject.SetActive(false);
            _autoRollText.gameObject.SetActive(false);
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
        public void EnableContinue()
        {
            _continuePressed = null;
            //_sceneTransition.GetPromptButton().GetComponent<Button>().onClick.AddListener(_sceneTransition.StartOutTransition);
            //_continuePressed += _sceneTransition.StartOutTransition;
            //_sceneTransition.GetPromptButton().GetComponent<Button>().onClick.AddListener(_sceneTransition.HidePrompt);
            //_continuePressed += _sceneTransition.HidePrompt;
            //_sceneTransition.GetPromptButton().GetComponent<Button>().onClick.AddListener(DisableContinue);
            //_continuePressed += DisableContinue;
        }
        public void DisableContinue()
        {
            _continuePressed = null;
            //_continuePressed += _sceneTransition.ResumeAsyncLoad;
        }
        void OnAllStatsRolled()
        {
            DisableControlText();
            //_sceneTransition.DisplayPrompt();
            //_sceneTransition.GetPromptButton().GetComponent<Button>().onClick.AddListener(DisableContinue);
            EnableContinue();
            SaveRolledStats();
        }
        void PlaySound(int currentStatIndex)
        {
            if (_audioSource == null) return;
            MainMenuUI.Instance.OptionMenuUI.AudioSettings.RandomizePitch(_audioSource);

            AudioClip audioClip = SelectAudioClip(currentStatIndex);

            if (audioClip == null) return;

            _audioSource.clip = audioClip;
            _audioSource.Play();
        }
        AudioClip SelectAudioClip(int currentStatIndex)
        {
            AudioClip audioClip = null;
            if (_statLevels[currentStatIndex] == _minStatValue)
            {
                audioClip = _lowStatSound;
            }
            else if (_statLevels[currentStatIndex] == _maxStatValue)
            {
                audioClip = _highStatSound;
            }
            else
            {
                audioClip = _statSound;
            }
            return audioClip;
        }
        void SaveRolledStats()
        {
            string characterName = _characterCreator != null ? _characterCreator.CharacterName : "";
            bool isMale = _characterCreator != null ? _characterCreator.IsMale :true;
            SaveSystem.SaveSystem.SaveCharacter(new CharacterStatsData(characterName, isMale, _statLevels));
            SaveSystem.SaveSystem.SaveContinueData(false);
        }
    }
}