using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace LostRunes.Multiplayer
{
    public class PlayerInitializer : NetworkBehaviour
    {
        PlayerLocomotion _playerLocomotion;
        PlayerManager _playerManager;
        InputHandler _inputHandler;
        Animator _animator;
        CharacterStats _characterStats;
        
        CameraHandler _cameraHandler;

        string _player = "Player ";
        public override void OnNetworkSpawn()
        {
            gameObject.name = _player + GetComponent<NetworkObject>().OwnerClientId.ToString();

            _inputHandler = GetComponent<InputHandler>();
            _characterStats = GetComponent<CharacterStats>();
            _playerLocomotion = GetComponent<PlayerLocomotion>();
            _animator = GetComponentInChildren<Animator>();
            _playerManager = GetComponent<PlayerManager>();

            _cameraHandler = FindObjectOfType<CameraHandler>();

            if (IsOwner)
            {
                _cameraHandler.Initialize(_playerManager, _inputHandler);
            }

            _playerManager.Initialize(IsOwner, _cameraHandler, _inputHandler, _characterStats, _playerLocomotion, _animator);
            _playerLocomotion.Initialize(IsOwner, _cameraHandler, _inputHandler, _playerManager);
            _inputHandler.Initialize(IsOwner);

            Destroy(this);
        }
    }
}