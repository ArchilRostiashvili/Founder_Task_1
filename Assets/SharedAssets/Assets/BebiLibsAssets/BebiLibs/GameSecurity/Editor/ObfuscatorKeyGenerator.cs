using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Purchasing;


namespace BebiLibs.GameSecurity
{
    using UnityEngine;
    using UnityEditor;

    internal class ObfuscatorKeyGenerator : EditorWindow
    {
        [MenuItem("BebiLibs/Helper/Obfuscator Key Generator")]
        private static void ShowWindow()
        {
            var window = GetWindow<ObfuscatorKeyGenerator>();
            window.titleContent = new GUIContent("ObfuscatorKeyGenerator");
            window.Show();
        }

        private static string obfuscateKey = "EAAAADAQABAAABgQDhwGQ+ecH53zxRkgsOtsdpSBJejSYrJIF5tkglOWzdxHHuhntxXOqFUS9nDZT6VwqYwzXorgzp32uxu2RjXO/xuoKcEzJqeXWMtJcJ67bniaO1SwUMidLGvoAPVh2DR4tIa+g/vqdc1r3VFZwH275KGPHJrIKYdt8SgSnFA3gqO";
        private static bool enableBase64Cast = false;

        private void OnGUI()
        {
            GUILayout.Label("Key To Obfuscate");
            obfuscateKey = EditorGUILayout.TextArea(obfuscateKey, GUILayout.MinHeight(20),
              GUILayout.MaxHeight(50));

            enableBase64Cast = EditorGUILayout.Toggle("Enable Base64Encoding", enableBase64Cast);

            if (GUILayout.Button("Generate Obfuscate Data"))
            {
                this.GenerateObfuscatorKey();
            }


            if (GUILayout.Button("Test Obfuscator"))
            {
#if UNITY_PURCHASING
                SafePlayerPrefs.SetString("SafeStringTest", "Hello World 2");
                string value = SafePlayerPrefs.GetString("SafeStringTest", "This Is Great");
                Debug.Log(value);

                SafePlayerPrefs.SetFloat("SafeStringTestFloat", 0.3124f);
                float floatValue = SafePlayerPrefs.GetFloat("SafeStringTestFloat", 0.2123f);
                Debug.Log(floatValue);

                SafePlayerPrefs.SetInt("SafeStringTestInt", 5);
                float intValue = SafePlayerPrefs.GetFloat("SafeStringTestInt", 4);
                Debug.Log(intValue);

                SafePlayerPrefs.SetBool("SafeStringTestBoolean", true);
                bool boolValue = SafePlayerPrefs.GetBool("SafeStringTestBoolean", false);
                Debug.Log(boolValue);
#endif
            }

        }

        public void GenerateObfuscatorKey()
        {
            string byte64String;
            if (enableBase64Cast)
            {
                byte64String = Base64Encode(obfuscateKey);
                Debug.Log("Original Base64 Key: " + byte64String);
            }
            else
            {
                byte64String = obfuscateKey;
            }

#if UNITY_PURCHASING
            var bytes = Convert.FromBase64String(byte64String);
            int[] order = new int[0];
            order = new int[bytes.Length / 20 + 1];
            byte[] tangled = new byte[0]; tangled = TangleObfuscator.Obfuscate(bytes, order, out int key);

            WriteObfuscatedClassAsAsset(key, order, tangled, tangled.Length != 0);
#endif
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private static void WriteObfuscatedClassAsAsset(int key, int[] order, byte[] data, bool populated)
        {
            Dictionary<string, string> substitutionDictionary = new Dictionary<string, string>()
            {
                {"{KEY}", key.ToString()},
                {"{ORDER}", String.Format("{0}",String.Join(",", Array.ConvertAll(order, i => i.ToString())))},
                {"{DATA}", Convert.ToBase64String(data)},
                {"{POPULATED}", populated.ToString().ToLowerInvariant()} // Defaults to XML-friendly values
            };

            Debug.Log("Key: " + substitutionDictionary["{KEY}"]);
            Debug.Log("Order: " + substitutionDictionary["{ORDER}"]);
            Debug.Log("OriginaKeyTangle: " + substitutionDictionary["{DATA}"]);
            //Debug.Log(substitutionDictionary["{POPULATED}"]);

            if (populated)
            {
                Debug.Log("Key Generation Successfull....");
            }
            else
            {
                Debug.Log("Key Generation Failed....");
            }
        }

    }
}

#endif

