using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Steamworks;
using Steamworks.Data;
using UnityEngine.UI;

namespace LostRunes
{
    public class ChatManager : MonoBehaviour
    {
        [SerializeField] TMP_InputField _messageInputField;
        [SerializeField] TextMeshProUGUI _messageTemplate;
        [SerializeField] GameObject _messagesContainer;
        [SerializeField] ScrollRect _scrollRect;

        PlayerControls _input;

        private void Start()
        {
            _messageTemplate.text = "";
        }
        private void OnEnable()
        {
            SteamMatchmaking.OnChatMessage += ChatSent;
            SteamMatchmaking.OnLobbyEntered += LobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += LobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave += LobbyMemberLeave;

            if(_input == null)
            {
                _input = new PlayerControls();
                _input.MainMenu.ToggleChat.performed += i => ToggleChatBox();
            }
            _input.Enable();
        }
        private void OnDisable()
        {
            SteamMatchmaking.OnChatMessage -= ChatSent;
            SteamMatchmaking.OnLobbyEntered -= LobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined -= LobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave -= LobbyMemberLeave;


            _input.Disable();
        }
        private void LobbyEntered(Lobby obj) => AddMessageBox("You entered the lobby");
        private void LobbyMemberJoined(Lobby lobby, Friend friend) => AddMessageBox(friend.Name + " Joined the lobby");
        private void LobbyMemberLeave(Lobby lobby, Friend friend) => AddMessageBox(friend.Name + " Left the lobby");

        private void ChatSent(Lobby lobby, Friend friend, string msg)
        {
            AddMessageBox(friend.Name + ": " + msg);
        }

        private void AddMessageBox(string msg)
        {
            GameObject message = Instantiate(_messageTemplate.gameObject, _messagesContainer.transform, false);
            message.GetComponent<TextMeshProUGUI>().text = msg;

            ScrollToBotom();
        }

        private void ScrollToBotom()
        {
            if (_scrollRect == null) return;
            _scrollRect.verticalNormalizedPosition = 0f;
        }

        void ToggleChatBox()
        {
            if (_messageInputField == null)  return;

            if (_messageInputField.gameObject.activeSelf)
            {
                if (!string.IsNullOrEmpty(_messageInputField.text))
                {
                    //LobbySaver.Instance._currentLobby?.SendChatString(_messageInputField.text);
                }
                _messageInputField.gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                _messageInputField.gameObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_messageInputField.gameObject);
            }
        }
    }
}