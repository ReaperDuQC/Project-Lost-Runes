using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LostRunes
{
    public enum Rarity { Common, Uncommon, Rare, Epic, Legendary}
    public class Item : ScriptableObject
    {
        public int _id;
        public string _itemName;
        public string _itemDescription;
        public Rarity _rarity;
        public Sprite _itemIcon;
        public GameObject _itemPrefab;
        public int _stackAmount;
    }
}