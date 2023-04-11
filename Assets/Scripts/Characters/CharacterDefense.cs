using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    [System.Serializable]
    public class CharacterDefense
    {
        public float _basePhysicalDefense = 50f;
        public List<float> _bonusPhysicalDefense = new();

        public float _baseFireDefense = 10f;
        public List<float> _bonusFireDefense = new();

        public float _baseThunderDefense = 10f;
        public List<float> _bonusThunderDefense = new();

        public float _baseIceDefense = 10f;
        public List<float> _bonusIceDefense = new();

        public float _baseMagicDefense = 10f;
        public List<float> _bonusMagicDefense = new();

        public float _baseHolyDefense = 10f;
        public List<float> _bonusHolyDefense = new();

        public float _baseDarkDefense = 10f;
        public List<float> _bonusDarkDefense = new();

        public float _baseBleedDefense = 10f;
        public List<float> _bonusBleedDefense = new();

        public float _basePoisonDefense = 10f;
        public List<float> _bonusPoisonDefense = new();

        public float TotalPhysicalDefense { get { return _basePhysicalDefense + GetAmount(_bonusPhysicalDefense); } }
        public float TotalFireDefense { get { return _baseFireDefense + GetAmount(_bonusFireDefense); } }
        public float TotalThunderDefense { get { return _baseThunderDefense + GetAmount(_bonusThunderDefense); } }
        public float TotalIceDefense { get { return _baseIceDefense + GetAmount(_bonusIceDefense); } }
        public float TotalMagiclDefense { get { return _baseMagicDefense + GetAmount(_bonusMagicDefense); } }
        public float TotalHolyDefense { get { return _baseHolyDefense + GetAmount(_bonusHolyDefense); } }
        public float TotalDarklDefense { get { return _baseDarkDefense + GetAmount(_bonusDarkDefense); } }
        public float TotalBleedDefense { get { return _baseBleedDefense + GetAmount(_bonusBleedDefense); } }
        public float TotalPoisonDefense { get { return _basePoisonDefense + GetAmount(_bonusPoisonDefense); } }

        public CharacterDefense()
        {

        }

        public void Initialize(CharacterStats stats)
        {

        }

        float GetAmount(List<float> list)
        {
            if (list == null || list.Count == 0) { return 0f; }

            float amount = 0f;

            foreach (var item in list)
            {
                amount += item;
            }
            return amount;
        }
    }
}