using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LostRunes
{
    public class DebugMenu : NetworkBehaviour
    {
        [Header("Display Options")]
        [SerializeField] bool _diplayVersion;
        [SerializeField] bool _diplayFPS;
        [SerializeField] bool _diplayControlScheme;
        [SerializeField] bool _diplayConnectedPlayers;

        [Header("Text References")]  
        [SerializeField] TextMeshProUGUI _versionText;
        [SerializeField] TextMeshProUGUI _fpsText;
        [SerializeField] TextMeshProUGUI _controlSchemeText;
        [SerializeField] TextMeshProUGUI _connectedPLayersText;
        [Header("External References")]

       
        [SerializeField] string _version;
        [SerializeField] ControlSchemeTracker _controlScheme;
        [SerializeField] FPSDisplay _fpsDisplay;

        [SerializeField] GameObject _debugInterface;

        PlayerControls _playerControls;

        private void Awake()
        {
            if (_fpsDisplay != null)
            {
                _fpsDisplay._fpsUpdated += UpdateFPSText;
            }

            if (_controlScheme != null)
            {
                _controlScheme._controlSchemeChanged += UpdateControlSchemeText;
            }

            if (_versionText != null)
            {
                _versionText.gameObject.SetActive(_diplayVersion);
            }

            if (_controlSchemeText != null)
            {
                _controlSchemeText.gameObject.SetActive(_diplayControlScheme);
            }

            if (_fpsText != null)
            {
                _fpsText.gameObject.SetActive(_diplayFPS);
            }

            _fpsDisplay.enabled = _diplayFPS;

            if (_connectedPLayersText != null)
            {
                _connectedPLayersText.gameObject.SetActive(_diplayConnectedPlayers);
            }

            UpdateFPSText(0);
            UpdateVersionText();
        }
        private void OnEnable()
        {
            if (_playerControls == null)
            {
                _playerControls = new();

                _playerControls.Debug.ToggleDebug.performed += ToggleDebugCallback;
            }
            _playerControls.Enable();
            if (NetworkManager.Singleton != null && IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += UpdateConnectedPlayersText;
                NetworkManager.Singleton.OnClientDisconnectCallback += UpdateConnectedPlayersText;
            }
        }

        private void ToggleDebugCallback(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ToggleDebugInterface();
            }
        }

        private void OnDisable()
        {
            _playerControls.Disable();
            if (NetworkManager.Singleton != null && IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= UpdateConnectedPlayersText;
                NetworkManager.Singleton.OnClientDisconnectCallback -= UpdateConnectedPlayersText;
            }
        }
        public void ToggleDebugInterface()
        {
            if (_debugInterface == null) return;

            _debugInterface.SetActive(!_debugInterface.activeSelf);
        }
        void UpdateConnectedPlayersText(ulong id)
        {
            int numberOfPlayers = NetworkManager.Singleton.ConnectedClients.Count;

            // Call the UpdateConnectedPlayersTextRpc() function on all clients
            UpdateConnectedPlayersTextClientRPC(numberOfPlayers);
            UpdateConnectedPlayersTextClientRPC(numberOfPlayers);
        }

        [ClientRpc]
        private void UpdateConnectedPlayersTextClientRPC(int numberOfPlayers)
        {
            if (_connectedPLayersText != null)
            {
                _connectedPLayersText.text = "Connected : " + numberOfPlayers.ToString()/* +  "/" + maxNumberOfPlayers.ToString()*/;
            }
        }

        void UpdateVersionText()
        {
            if (_versionText != null)
            {
                _versionText.text = "Version : " + _version;
            }
        }

        void UpdateFPSText(int fps)
        {
            if (_fpsText != null)
            {
                _fpsText.text = "FPS : " + fps.ToString();
            }
        }

        void UpdateControlSchemeText(string newControlScheme)
        {
            if (_controlSchemeText != null)
            {
                _controlSchemeText.text = "Control Scheme : " + newControlScheme;
            }
        }
    }
}