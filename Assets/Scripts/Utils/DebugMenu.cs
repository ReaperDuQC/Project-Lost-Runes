using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    [SerializeField] Color _debugColor = Color.white;

    [SerializeField] bool _diplayVersion;
    [SerializeField] TextMeshProUGUI _versionText;

    [SerializeField] bool _diplayFPS;
    [SerializeField] TextMeshProUGUI _fpsText;

    [SerializeField] bool _diplayControlScheme;
    [SerializeField] TextMeshProUGUI _controlSchemeText;

    [SerializeField] bool _diplayConnectedPlayers;
    [SerializeField] TextMeshProUGUI _connectedPLayersText;
    [Header("External References")]

    [SerializeField] string _version;
    [SerializeField] ControlSchemeTracker _controlScheme;
    [SerializeField] FPSDisplay _fpsDisplay;

    private void Awake()
    {
        if(_fpsDisplay != null)
        {
            _fpsDisplay._fpsUpdated += UpdateFPSText;
        }

        if(_controlScheme != null)
        {
            _controlScheme._controlSchemeChanged += UpdateControlSchemeText;
        }
        // Get current Version and display
        // _version = 
        if (_diplayVersion)
        {
            if(_versionText != null)
            {
                _versionText.gameObject.SetActive(_diplayVersion);
            }
        }

        if (_diplayControlScheme)
        {
            if (_controlSchemeText != null)
            {
                _controlSchemeText.gameObject.SetActive(_diplayControlScheme);
            }
        }

        if (_diplayFPS)
        {
            if (_fpsText != null)
            {
                _fpsText.gameObject.SetActive(_diplayFPS);
                _fpsDisplay.enabled = _diplayFPS;
            }

            _fpsDisplay.enabled = _diplayFPS;
        }

        if (_diplayConnectedPlayers)
        {
            if (_connectedPLayersText != null)
            {
                _connectedPLayersText.gameObject.SetActive(_diplayConnectedPlayers);
            }
        }

        UpdateFPSText(0);
        UpdateVersionText();
        UpdateConnectedPlayersText(0);
    }
    private void OnEnable()
    {
        if(NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += UpdateConnectedPlayersText;
            NetworkManager.Singleton.OnClientDisconnectCallback += UpdateConnectedPlayersText;
        }
    }
    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= UpdateConnectedPlayersText;
            NetworkManager.Singleton.OnClientDisconnectCallback -= UpdateConnectedPlayersText;
        }
    }
    void UpdateConnectedPlayersText(ulong id)
    {
        if (_connectedPLayersText != null)
        {
            if (NetworkManager.Singleton != null)
            {
                int numberOfPlayers = NetworkManager.Singleton.ConnectedClients.Count;
                _connectedPLayersText.text = "Connected : " + numberOfPlayers.ToString()/* +  "/" + maxNumberOfPlayers.ToString()*/;
            }
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
