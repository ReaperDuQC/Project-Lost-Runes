using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetcodePlus;

namespace NetcodePlus.Demo
{
    public class PlayerLine : MonoBehaviour
    {
        public Text title;

        private LobbyPlayer player;

        void Awake()
        {

        }

        public void SetLine(LobbyPlayer player)
        {
            this.player = player;
            title.text = player.username;
        }

        public void Hide()
        {
            this.player = null;
            title.text = "";
        }

        public LobbyPlayer GetPlayer()
        {
            return player;
        }
    }
}