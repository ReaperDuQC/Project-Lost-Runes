using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LostRunes.Tools
{
    public class SoundPicker : MonoBehaviour
    {
        [SerializeField] AudioClip _onCharacterHit1;
        [SerializeField] AudioClip _onCharacterHit2;
        AudioSource _source;
        // Start is called before the first frame update
        void Start()
        {
            _source = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Mouse.current.leftButton.IsActuated())
            {
                if (_onCharacterHit1 != null)
                {

                    _source.clip = _onCharacterHit1;
                    _source.PlayOneShot(_onCharacterHit1);
                }
            }
            if (Mouse.current.rightButton.IsActuated())
            {
                if (_onCharacterHit2 != null)
                {
                    _source.clip = _onCharacterHit2;
                    _source.PlayOneShot(_onCharacterHit2);
                }
            }
        }
    }
}