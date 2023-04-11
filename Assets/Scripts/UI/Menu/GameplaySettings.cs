using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes.Menu
{
    [System.Serializable]
    public class GameplaySettingsData
    {
        public GameplaySettingsData()
        {

        }
        public GameplaySettingsData(GameplaySettings gameplaySettings)
        {

        }
    }
    public class GameplaySettings : MonoBehaviour
    {
        public void LoadGameplaySettingsData()
        {
            GameplaySettingsData data = SaveSystem.SaveSystem.LoadGameplaySettings();

        }
        public void SaveGameplaySettings()
        {
            SaveSystem.SaveSystem.SaveGameplaySettings(this);
        }
        public void ResetToDefault()
        {

        }
    }
}