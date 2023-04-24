using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetcodePlus;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace NetcodePlus.Demo
{
    public class MenuLobby : MonoBehaviour
    {
        private static MenuLobby instance;

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            LobbyConnectPanel.Get().Show();
			Menu.last_menu = SceneManager.GetActiveScene().name;
        }

        public void HideAllPanels()
        {
            LobbyPanel.Get().Hide();
            LobbyConnectPanel.Get().Hide();
            LobbyCreatePanel.Get().Hide();
            LobbyRoomPanel.Get().Hide();
            ConnectingPanel.Get().Hide();
        }

        public async void CreateGame(CreateGameData cdata)
        {
            await ClientLobby.Get().CreateGame(cdata);
        }

        public async void ConnectToGame(LobbyGame game)
        {
            if (game == null)
                return;

            HideAllPanels();
            ConnectingPanel.Get().Show();

            await Task.Delay(500);

            GameMode mode = (GameMode)NetworkTool.DeserializeInt32(game.extra);
            DemoConnectData ddata = new DemoConnectData(mode);
            ddata.character = LobbyConnectPanel.Get().GetCharacter();
            ClientLobby.Get().SetConnectionExtraData(ddata);

            bool host = game.IsHost(ClientLobby.Get().ClientID);
            if (host)
                await ConnectToGameHost(game);
            else
                await ConnectToGameJoin(game);
        }

        private async Task ConnectToGameHost(LobbyGame game)
        {
            await Task.Delay(1000); //Wait for server to start

            await ClientLobby.Get().ConnectToGame(game);

            await Task.Delay(1000);

            if (!TheNetwork.Get().IsConnected())
                await ClientLobby.Get().ConnectToGame(game); //Try again
        }

        private async Task ConnectToGameJoin(LobbyGame game)
        {
            await Task.Delay(3000); //Wait for host to connect

            await ClientLobby.Get().ConnectToGame(game);

            await Task.Delay(1000); 

            if(!TheNetwork.Get().IsConnected())
                await ClientLobby.Get().ConnectToGame(game); //Try again

            await Task.Delay(1000);

            if (!TheNetwork.Get().IsConnected())
                await ClientLobby.Get().ConnectToGame(game); //Try again
        }

        public void OnClickSwitch()
        {
            Menu.GoToSimpleMenu();
        }

        public static MenuLobby Get()
        {
            return instance;
        }
    }
}