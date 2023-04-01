using MalbersAnimations.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;


namespace MalbersAnimations
{
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/annex/integrations/unity-input-system-new#input-link-ui")]
    [AddComponentMenu("Malbers/Input/MInput UI")]
    public class MInputLinkUI : MonoBehaviour
    {
        public InputActionReference input;
        public StringEvent UpdateInput = new StringEvent();

        private void OnEnable() => InputUser.onChange += OnUserChange;

        private void OnDisable() => InputUser.onChange -= OnUserChange;

        private void OnUserChange(InputUser user, InputUserChange change, InputDevice device)
        {
            if (change == InputUserChange.ControlsChanged) UpdateUIInput();
        }
        public void UpdateUIInput() => UpdateInput.Invoke(input.action.GetBindingDisplayString());
    }
}
