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
        #region Settings

        #region Audio Settings
        public static void SaveAudioSettings(Menu.AudioSettings audioSettings)
        {
            string folderName = "Settings";
            string path = Application.persistentDataPath + "/" + folderName + "/" + "audio.set";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }


            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            AudioSettingsData data = new AudioSettingsData(audioSettings);

            formatter.Serialize(stream, data);
            stream.Close();
        }
        public static AudioSettingsData LoadAudioSettings()
        {
            string folderName = "Settings";
            string path = Application.persistentDataPath + "/" + folderName + "/" + "audio.set";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }

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
            string folderName = "Settings";
            string path = Application.persistentDataPath + "/" + folderName + "/" + "gameplay.set";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            GameplaySettingsData data = new GameplaySettingsData(gameplaySettings);

            formatter.Serialize(stream, data);
            stream.Close();
        }
        public static GameplaySettingsData LoadGameplaySettings()
        {
            string folderName = "Settings";
            string path = Application.persistentDataPath + "/" + folderName + "/" + "gameplay.set";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }

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
            string folderName = "Settings";
            string path = Application.persistentDataPath + "/" + folderName + "/" + "graphic.set";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            GraphicSettingsData data = new GraphicSettingsData(graphicSettings);

            formatter.Serialize(stream, data);
            stream.Close();
        }
        public static GraphicSettingsData LoadGraphicSettings()
        {
            string folderName = "Settings";
            string path = Application.persistentDataPath + "/" + folderName + "/" + "graphic.set";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }

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

        #region Player

        public static void SavePlayerAtlas() // need player atlas data
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/Data/graphic";
            FileStream stream = new FileStream(path, FileMode.Create);

            //GraphicSettingsData data = new GraphicSettingsData(graphicSettings);

            //formatter.Serialize(stream, data);
            stream.Close();
        }
        public static GraphicSettingsData LoadPlayerAtlas()
        {
            string path = Application.persistentDataPath + "/Settings/graphic";

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

        public static void SavePlayerData()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/Settings/graphic";
            FileStream stream = new FileStream(path, FileMode.Create);

            //GraphicSettingsData data = new GraphicSettingsData(graphicSettings);

            //formatter.Serialize(stream, data);
            stream.Close();
        }
        public static GraphicSettingsData LoadPlayerData()
        {
            string path = Application.persistentDataPath + "/Settings/graphic";

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

        #region World

        public static void SaveWorldAtlas(WorldAtlas worldAtlas)
        {
            string folderName = "Data";
            string path = Application.persistentDataPath + "/" + folderName + "/" + "WorldAtlas.wld";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            WorldAtlas data = worldAtlas;

            formatter.Serialize(stream, data);
            stream.Close();
        }
        public static WorldAtlas LoadWorldAtlas()
        {
            string folderName = "Data";
            string path = Application.persistentDataPath + "/" + folderName + "/" + "WorldAtlas.wld";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }


            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                WorldAtlas data = formatter.Deserialize(stream) as WorldAtlas;
                stream.Close();

                return data;
            }
            else
            {
                //Debug.Log("Save file not found in " + path);
                return new WorldAtlas();
            }
        }
        public static void SaveWorldData(GraphicSettings graphicSettings)
        {
            string folderName = "Data";
            string path = Application.persistentDataPath + "/" + folderName + "/" + " " +".wld";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }


            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            GraphicSettingsData data = new GraphicSettingsData(graphicSettings);

            formatter.Serialize(stream, data);
            stream.Close();
        }
        public static GraphicSettingsData LoadWorldData()
        {
            string path = Application.persistentDataPath + "/Settings/graphic";

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
    }
}