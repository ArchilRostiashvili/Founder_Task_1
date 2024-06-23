using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BebiLibs
{
    public class JsonFileLoader
    {
        public static string GetFilePathInDisk(string fileName) => Path.Combine(Application.persistentDataPath, fileName + ".json");

        public static bool TryLoadJsonFile<T>(string filename, out T result)
        {
            try
            {
                string jr = PlayerPrefs.GetString(filename, string.Empty);
                //using var s = new StreamReader(GetFilePathInDisk(filename));
                //using var jr = new JsonTextReader(s);
                var js = new JsonSerializer();
                result = JsonConvert.DeserializeObject<T>(jr);
                return result != null;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Unable To Load Json File " + ex);
                result = default;
                return false;
            }
        }


        public static bool TryLoadJsonFile<T>(string filename, out string jsonText)
        {
            try
            {
                string jr = PlayerPrefs.GetString(filename, string.Empty);
                //using var s = new StreamReader(GetFilePathInDisk(filename));
                //using var jr = new JsonTextReader(s);
                //jsonText = jr.ReadAsString();
                jsonText = jr;
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Unable To Load Json File: " + ex);
                jsonText = string.Empty;
                return false;
            }
        }


        public static bool TrySaveJsonFile<T>(string filename, string json)
        {
            try
            {
                //using var s = new StreamWriter(GetFilePathInDisk(filename));
                //using var jr = new JsonTextWriter(s);
                //jr.WriteRaw(json);
                PlayerPrefs.SetString(filename, json);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Unable To Save Json File: " + ex);
                return false;
            }

        }


        public static bool TrySaveJsonFile<T>(string filename, T obj)
        {
            try
            {
                // using var s = new StreamWriter(GetFilePathInDisk(filename));
                // using var jr = new JsonTextWriter(s);
                //var js = new JsonSerializer();
                string jsonString = JsonConvert.SerializeObject(obj);
                PlayerPrefs.SetString(filename, jsonString);
                //js.Serialize(jr, obj, typeof(T));
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Unable To Save Json File: " + ex);
                return false;
            }
        }
    }
}
