using LostRunes.Menu;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LostRunes
{
    public enum NameStatus { Valid, AlreadyExist, TooShort, TooLong }
    public class NameInputHandler : MonoBehaviour
    {
        NameStatus _nameStatus = NameStatus.TooShort;
        [SerializeField] CustomInputField _inputField;
        [SerializeField] CustomButton _validateButton;
        [SerializeField] TextMeshProUGUI _errorMessage;
        private void Awake()
        {
            SetValidateNameButtonInteractable(false);
        }
        public void IsNameValid(string value, List<string> atlas)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= 1)
            {
                _nameStatus = NameStatus.TooShort;
                UpdateErrorMessage("The name of the world is too short.");
                SetValidateNameButtonInteractable(false);
                return;
            }
            if (value.Length > 20)
            {
                _nameStatus = NameStatus.TooLong;
                UpdateErrorMessage("The name of the world is too long.");
                SetValidateNameButtonInteractable(false);
                return;
            }

            foreach (string name in atlas)
            {
                if (name == value)
                {
                    _nameStatus = NameStatus.AlreadyExist;
                    UpdateErrorMessage("Another world already has that name.");
                    SetValidateNameButtonInteractable(false);
                    return;
                }
            }

            _nameStatus = NameStatus.Valid;
            SetValidateNameButtonInteractable(true);
            UpdateErrorMessage("");
        }
        public void ResetErrorMessage()
        {
            if (_errorMessage != null)
            {
                UpdateErrorMessage("");
            }
        }
        void UpdateErrorMessage(string message)
        {
            if (_errorMessage == null) return;

            if (string.IsNullOrEmpty(message) || message.Length == 0)
            {
                _errorMessage.text = "";
                return;
            }

            _errorMessage.text = "Error : " + message;
        }
        void SetValidateNameButtonInteractable(bool interactable)
        {
            if (_validateButton == null) return;

            _validateButton.interactable = interactable;
        }
        public void ResetInputText()
        {
            if (_inputField == null) return;

            _inputField.text = "";
            _validateButton.interactable = false;
            ResetErrorMessage();
        }
        public string GetName()
        {
            if (_inputField == null) return string.Empty;

            return _inputField.text;
        }
    }
}