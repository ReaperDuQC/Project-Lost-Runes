using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostRunes.SaveSystem;
using LostRunes.Menu;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace LostRunes
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] GameObject _characterPrefab;
        [SerializeField] Transform _spawnPosition;
        [SerializeField] GameObject _player;

        [SerializeField] SceneTransition _transition;

        [SerializeField] GameMenu _gameMenu;
        public GameMenu GameMenu { get { return _gameMenu; } }

        static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }
        private void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
            HideCursor(SceneManager.GetActiveScene().name == "Menu");
        }
        private void Start()
        {
            LoadPlayer();
        }

        private void LoadPlayer()
        {
            if (_characterPrefab == null) return;
            if (_player == null) return;

            GameObject player = Instantiate(_characterPrefab);
            player.transform.parent = _player.transform;
            player.transform.position = _spawnPosition != null ? _spawnPosition.position : Vector3.zero;
        }
        public void SaveGame()
        {

        }
        public void ReturnToMainMenu()
        {
            if (_transition == null) return;

            _transition.SetSceneToLoad("Menu");
            _transition.StartOutTransition();
        }
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        public void HideCursor(bool visible)
        {
           // Cursor.visible = visible;
           // Cursor.lockState = visible ? CursorLockMode.Confined : CursorLockMode.Locked;
        }
    }
}