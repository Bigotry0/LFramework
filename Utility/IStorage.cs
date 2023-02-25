#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using UnityEngine;

namespace LFramework.AI.Utility
{
    public interface IStorage : IUtility
    {
        void SaveInt(string key, int value);
        int LoadInt(string key, int defaultValue = 0);
    }

    public interface IJsonStorage : IUtility
    {
        void SaveByJson(string fileName, object data);
        T LoadFromJson<T>(string fileName);
        void DeleteSaveFile(string fileName);
    }

    /// <summary>
    /// PlayerPrefs存档
    /// </summary>
    public class PlayerPrefsStorage : IStorage
    {
        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public int LoadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }

    /// <summary>
    /// EditorPrefs存档
    /// </summary>
    public class EditorPrefsStorage : IStorage
    {
        public void SaveInt(string key, int value)
        {
#if UNITY_EDITOR
            EditorPrefs.SetInt(key, value);
#endif
        }

        public int LoadInt(string key, int defaultValue = 0)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetInt(key, defaultValue);
#else
            return 0;
#endif
        }
    }

    /// <summary>
    /// Json存档
    /// </summary>
    public class JsonStorage : IJsonStorage
    {
        public void SaveByJson(string fileName, object data)
        {
            var path = Path.Combine(Application.persistentDataPath, fileName);

            try
            {
                var json = JsonUtility.ToJson(data);
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"保存Json数据到{path}失败，错误：{e}");
            }
        }

        public T LoadFromJson<T>(string fileName)
        {
            var path = Path.Combine(Application.persistentDataPath, fileName);

            try
            {
                var json = File.ReadAllText(path);

                var data = JsonUtility.FromJson<T>(json);

                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"从{path}中读取数据失败。错误：{e}");

                return default;
            }
        }

        public void DeleteSaveFile(string fileName)
        {
            var path = Path.Combine(Application.persistentDataPath, fileName);

            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                Debug.LogError($"从{path}中删除存档失败。错误：{e}");
            }
        }
    }
}