using System.IO;
using UnityEngine;

namespace Core.Persistence
{
    public class SaveSystem
    {
        private const string FileName = "game_save.json";

        private string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        public void Save(GameSaveData data)
        {
            if (data == null)
            {
                Debug.LogWarning("SaveSystem.Save called with null data");
                return;
            }

            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SavePath, json);
#if UNITY_EDITOR
                Debug.Log($"Game saved to: {SavePath}");
#endif
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Save failed: {ex}");
            }
        }

        public GameSaveData Load()
        {
            if (!File.Exists(SavePath))
            {
#if UNITY_EDITOR
                Debug.Log("No save file found");
#endif
                return null;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                return JsonUtility.FromJson<GameSaveData>(json);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Load failed: {ex}");
                return null;
            }
        }

        public void Clear()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
#if UNITY_EDITOR
                Debug.Log("Save file deleted");
#endif
            }
        }
    }
}