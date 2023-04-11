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

        HealthBar _healthBar;
        ManaBar _manaBar;
        StaminaBar _staminaBar;

        string _player = "Player ";
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        public void Initialize(CharacterStats stats)
        {
            bool isOwner = true;// IsOwner;
            gameObject.name = _player + GetComponent<NetworkObject>().OwnerClientId.ToString();

            _inputHandler = GetComponent<InputHandler>();
            _characterStats = stats;
            _playerLocomotion = GetComponent<PlayerLocomotion>();
            _animator = GetComponentInChildren<Animator>();
            _playerManager = GetComponent<PlayerManager>();

            _cameraHandler = FindObjectOfType<CameraHandler>();

            GameManager gm = GameManager.Instance;
            gm?.HealthBar?.Initialize(stats);
            gm?.ManaBar?.Initialize(stats);
            gm?.StaminaBar?.Initialize(stats);

            gm?.CharacterStatUI?.Initialize(stats);

            if (isOwner)
            {
                _cameraHandler.Initialize(_playerManager, _inputHandler);
            }
            this.enabled = true;

            _playerManager.Initialize(isOwner, _cameraHandler, _inputHandler, _characterStats, _playerLocomotion, _animator);
            _playerLocomotion.Initialize(isOwner, _cameraHandler, _inputHandler, _playerManager);
            _inputHandler.Initialize(isOwner);

            Destroy(this);
        }
    }
}