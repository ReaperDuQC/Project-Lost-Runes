using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostRunes.Menu;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Rendering;

namespace LostRunes.SaveSystem
{
    public static class SaveSystem
    {
        #region Option Settings

        #region Audio Settings
        public static void SaveAudioSettings(Menu.AudioSettings audioSettings)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/Option.Settings.Audio";
            FileStream stream = new FileStream(path, FileMode.Create);

            AudioSettingsData data = new AudioSettingsData(audioSettings);

            formatter.Serialize(stream, data);
            stream.Close();
        }
        public static AudioSettingsData LoadAudioSettings()
        {
            string path = Application.persistentDataPath + "/Option.Settings.Audio";

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                AudioSettingsData data = formatter.Deserialize(stream) as AudioSettingsData;
                stream.Close();

                return data;
            }
            else
            {
                //Debug.Log("Save file not found in " + path);

                return new AudioSettingsData();
            }
        }
        #endregion

        #region Gameplay Settings
        public static void SaveGameplaySettings(GameplaySettings gameplaySettings)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/Option.Settings.Gameplay";
            FileStream stream = new FileStream(path, FileMode.Create);

            GameplaySettingsData data = new GameplaySettingsData(gameplaySettings);

            formatter.Serialize(stream, data);
            stream.Close();
        }
        public static GameplaySettingsData LoadGameplaySettings()
        {
            string path = Application.persistentDataPath + "/Option.Settings.Gameplay";

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                GameplaySettingsData data = formatter.Deserialize(stream) as GameplaySettingsData;
                stream.Close();

                return data;
            }
            else
            {
                //Debug.Log("Save file not found in " + path);
                return new GameplaySettingsData();
            }
        }
        #endregion

        #region Graphic Settings
        public static void SaveGraphicSettings(GraphicSettings graphicSettings)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/Option.Settings.Graphic";
            FileStream stream = new FileStream(path, FileMode.Create);

            GraphicSettingsData data = new GraphicSettingsData(graphicSettings);

            formatter.Serialize(stream, data);
            stream.Close();
        }
        public static GraphicSettingsData LoadGraphicSettings()
        {
            string path = Application.persistentDataPath + "/Option.Settings.Graphic";

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                GraphicSettingsData data = formatter.Deserialize(stream) as GraphicSettingsData;
                stream.Close();

                return data;
            }
            else
            {
                //Debug.Log("Save file not found in " + path);
                return new GraphicSettingsData();
            }
        }
        #endregion

        #endregion

        #region Character
        public static void SaveCharacter(CharacterStatsData stats)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/Player.Stats";
            FileStream stream = new FileStream(path, FileMode.Create);

            CharacterStatsData data = stats;

            formatter.Serialize(stream, data);
            stream.Close();
        }
        public static CharacterStatsData LoadCharacter()
        {
            string path = Application.persistentDataPath + "/Player.Stats";

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                CharacterStatsData data = formatter.Deserialize(stream) as CharacterStatsData;
                stream.Close();

                return data;
            }
            else
            {
                //Debug.Log("Save file not found in " + path);
                return null;
            }
        }
        #endregion

        //public static void SaveContinueData(bool isContinue)
        //{
        //    BinaryFormatter formatter = new BinaryFormatter();
        //    string path = Application.persistentDataPath + "/Game.Continue";
        //    FileStream stream = new FileStream(path, FileMode.Create);

        //    bool data = isContinue;

        //    formatter.Serialize(stream, data);
        //    stream.Close();
        //}
        //public static bool LoadContinueData()
        //{
        //    string path = Application.persistentDataPath + "/Game.Continue";

        //    if (File.Exists(path))
        //    {
        //        BinaryFormatter formatter = new BinaryFormatter();
        //        FileStream stream = new FileStream(path, FileMode.Open);

        //        bool data = (bool)formatter.Deserialize(stream);
        //        stream.Close();

        //        return data;
        //    }
        //    else
        //    {
        //        //Debug.Log("Save file not found in " + path);
        //        return false;
        //    }
        //}
    }
}