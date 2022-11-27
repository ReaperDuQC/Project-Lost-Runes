using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace RPG
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField] Button _hostButton;
        [SerializeField] Button _clientButton;
        [SerializeField] Button _serverButton;

        private void Awake()
        {
            _hostButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
            });
            _clientButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
            });
            _serverButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartServer();
            });
        }
    }
}