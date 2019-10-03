using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace WinterFeather
{
    public static class DataTools
    {
        public static string simpleKey = "winter";
        //Application.persistentDataPath +@"\SaveData\LevelsData.txt"
        public static void SaveToText(string data, string path)
        {
            //string path = Application.persistentDataPath + subPath;
            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine(data);
            sw.Dispose();
        }

        public static void AppendToText(string data, string path)
        {
            StreamWriter sw = new StreamWriter(path, true);
            sw.Write(data);
            sw.Dispose();
        }


        public static string LoadFormText(string path)
        {
            //string path = Application.persistentDataPath + subPath;
            string data = "";
            try
            {
                StreamReader sr = new StreamReader(path);
                data = sr.ReadToEnd();
                sr.Dispose();
            }
            catch (Exception)
            {

            }
            return data;
        }

        public static void SaveToByte(string data, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            byte[] buffs = System.Text.Encoding.Default.GetBytes(data);
            //TODO:For Thread -> While -> SetLengthLimmit

            SimpleCode(buffs, simpleKey);

            fs.Write(buffs, 0, buffs.Length);

            fs.Dispose();
        }

        public static void AppendToByte(string data, string path)
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            fs.Seek(0, SeekOrigin.End);
            byte[] buffs = System.Text.Encoding.Default.GetBytes(data);
            fs.Write(buffs, 0, buffs.Length);

            fs.Dispose();
        }

        public static byte[] LoadFormByte(string path)
        {
            List<byte> dataList = new List<byte>();
            byte[] data;
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] buffs = new byte[1024];
                while (true)
                {
                    int readCount = fs.Read(buffs, 0, buffs.Length);
                    if (readCount <= 0) break;
                    if (readCount == buffs.Length)
                    {
                        dataList.AddRange(buffs);
                    }
                    else
                    {
                        byte[] endBuffs = new byte[readCount];
                        Array.Copy(buffs, endBuffs, endBuffs.Length);
                        dataList.AddRange(endBuffs);
                    }
                }
                dataList.Capacity = dataList.Count;
                data = dataList.ToArray();
                SimpleCode(data, simpleKey);
                fs.Dispose();
            }
            catch (Exception)
            {
                throw;
            }

            return data;
        }

        public static void SaveToJason<T>(List<T> data, string path)
        {
            Serialization<T> s = new Serialization<T>(data);
            SaveToJason(s, path);
        }

        public static void SaveToJason<T, U>(Dictionary<T, U> data, string path)
        {
            Serialization<T, U> s = new Serialization<T, U>(data);
            SaveToJason(s, path);
        }

        public static void SaveToJason(object data, string path)
        {
            string jsonData = JsonUtility.ToJson(data);
            SaveToByte(jsonData, path);
        }

        public static List<T> LoadFormJsonToList<T>(string path)
        {
            string data = System.Text.Encoding.UTF8.GetString(LoadFormByte(path));
            Serialization<T> result = JsonUtility.FromJson<Serialization<T>>(data);
            return result.ToList();
        }

        public static Dictionary<T, U> LoadFormJsonToDictionary<T, U>(string path)
        {
            string data = System.Text.Encoding.UTF8.GetString(LoadFormByte(path));
            Serialization<T, U> r = JsonUtility.FromJson<Serialization<T, U>>(data);
            return r.ToDictionary();
        }

        public static T LoadFormJson<T>(string path)
        {
            string data = System.Text.Encoding.UTF8.GetString(LoadFormByte(path));
            return JsonUtility.FromJson<T>(data);
        }

        static void SimpleCode(byte[] data, string key)
        {
            SimpleCode(data, string.IsNullOrEmpty(key) ? null : System.Text.Encoding.Default.GetBytes(key));
        }

        static void SimpleCode(byte[] data, byte[] key)
        {
            if (key == null || key.Length <= 0) return;
            for (int i = 0; i < data.Length; i += key.Length)
            {
                for (int j = i; j < i + key.Length; j++)
                {
                    if (j >= data.Length) break;
                    data[j] ^= key[j - i];
                }
            }
        }


        [Serializable]
        class Serialization<T>
        {
            [SerializeField]
            List<T> target;
            public List<T> ToList() { return target; }

            public Serialization(List<T> target)
            {
                this.target = target;
            }
        }

        [Serializable]
        class Serialization<TKey, TValue> : ISerializationCallbackReceiver
        {
            [SerializeField]
            List<TKey> keys;
            [SerializeField]
            List<TValue> values;

            Dictionary<TKey, TValue> target;
            public Dictionary<TKey, TValue> ToDictionary() { return target; }

            public Serialization(Dictionary<TKey, TValue> target)
            {
                this.target = target;
            }

            public void OnBeforeSerialize()
            {
                keys = new List<TKey>(target.Keys);
                values = new List<TValue>(target.Values);
            }

            public void OnAfterDeserialize()
            {
                var count = Math.Min(keys.Count, values.Count);
                target = new Dictionary<TKey, TValue>(count);
                for (var i = 0; i < count; ++i)
                {
                    target.Add(keys[i], values[i]);
                }
            }
        }
    }


}