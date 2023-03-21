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


            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Create);

            AudioSettingsData data = new(audioSettings);

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
                BinaryFormatter formatter = new();
                FileStream stream = new(path, FileMode.Open);

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

            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Create);

            GameplaySettingsData data = new(gameplaySettings);

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
                BinaryFormatter formatter = new();
                FileStream stream = new(path, FileMode.Open);

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

            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Create);

            GraphicSettingsData data = new(graphicSettings);

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
                BinaryFormatter formatter = new();
                FileStream stream = new(path, FileMode.Open);

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

        public static void SavePlayerAtlas(PlayerAtlas atlas) // need player atlas data
        {
            string folderName = "Data";
            string path = Application.persistentDataPath + "/" + folderName + "/" + "PlayerAtlas.plr";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }

            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Create);

            PlayerAtlas data = atlas;

            formatter.Serialize(stream, data);
            stream.Close();
        }
        public static PlayerAtlas LoadPlayerAtlas()
        {
            string folderName = "Data";
            string path = Application.persistentDataPath + "/" + folderName + "/" + "PlayerAtlas.plr";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }


            if (File.Exists(path))
            {
                BinaryFormatter formatter = new();
                FileStream stream = new(path, FileMode.Open);

                PlayerAtlas data = formatter.Deserialize(stream) as PlayerAtlas;
                stream.Close();

                return data;
            }
            else
            {
                //Debug.Log("Save file not found in " + path);
                return new PlayerAtlas();
            }
        }

        public static void SavePlayerData(PlayerData playerData)
        {
            string folderName = "Data";
            string path = Application.persistentDataPath + "/" + folderName + "/" + playerData._name + ".plr";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }

            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Create);

            PlayerData data = playerData;

            formatter.Serialize(stream, data);
            stream.Close();
        }
        public static PlayerData LoadPlayerData(string playerName)
        {
            string folderName = "Data";
            string path = Application.persistentDataPath + "/" + folderName + "/" + playerName + ".plr";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new();
                FileStream stream = new(path, FileMode.Open);

                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                stream.Close();

                return data;
            }
            else
            {
                //Debug.Log("Save file not found in " + path);
                return new PlayerData();
            }
        }

        public static void DeleteExistingPlayer(string fileName)
        {
            PlayerAtlas atlas = LoadPlayerAtlas();

            if (!atlas.Players.Contains(fileName)) return;

            atlas.Players.Remove(fileName);

            SavePlayerAtlas(atlas);


            string folderName = "Data";
            string path = Application.persistentDataPath + "/" + folderName + "/" + fileName + ".plr";

            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("Successfully deleted file: " + fileName);
            }
            else
            {
                Debug.LogError("File not found: " + fileName);
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

            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Create);

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
                BinaryFormatter formatter = new();
                FileStream stream = new(path, FileMode.Open);

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
        public static void SaveWorldData(WorldData worldData)
        {
            string folderName = "Data";
            string path = Application.persistentDataPath + "/" + folderName + "/" + worldData._worldName + ".wld";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }

            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Create);

            WorldData data = worldData;

            formatter.Serialize(stream, data);
            stream.Close();
        }
        public static WorldData LoadWorldData(string worldName)
        {
            string folderName = "Data";
            string path = Application.persistentDataPath + "/" + folderName + "/" + worldName + ".wld";

            if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);
            }


            if (File.Exists(path))
            {
                BinaryFormatter formatter = new();
                FileStream stream = new(path, FileMode.Open);

                WorldData data = formatter.Deserialize(stream) as WorldData;
                stream.Close();

                return data;
            }
            else
            {
                //Debug.Log("Save file not found in " + path);
                return null;
            }
        }
        public static void DeleteExistingWorld(string fileName)
        {
            WorldAtlas atlas = LoadWorldAtlas();

            if (!atlas.Worlds.Contains(fileName)) return;

            atlas.Worlds.Remove(fileName);

            SaveWorldAtlas(atlas);


            string folderName = "Data";
            string path = Application.persistentDataPath + "/" + folderName + "/" + fileName + ".wld";

            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("Successfully deleted file: " + fileName);
            }
            else
            {
                Debug.LogError("File not found: " + fileName);
            }
        }
        #endregion
    }
}