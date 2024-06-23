using BebiLibs.Analytics;
using BebiLibs.PlayerPreferencesSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


#if ACTIVATE_FIREBASE
using Firebase.Analytics;
using Firebase;
#endif


namespace BebiLibs.PlayerPreferencesSystem
{
    public class SerializationTester : MonoBehaviour
    {

        //adb shell setprop debug.firebase.analytics.app com.happytools.learning.kids.games
        public const string PLAYER_PREF_KEY = "pp_pref_test";

        private void Start()
        {
            FirebaseDependencyResolver.AddInitializationListener(OnFirebaseInitialize, true);
        }

        private void OnFirebaseInitialize(bool isFirebaseAvailable)
        {
            if (!isFirebaseAvailable) return;

            TestUnityPlayerPrefs("_one");
            //TestCustomPlayerPrefs("_one");

            StartCoroutine(RepeatTestAfter());
        }

        private IEnumerator RepeatTestAfter(float time = 10f)
        {
            yield return new WaitForSeconds(time);

            TestUnityPlayerPrefs("_two");

            // PlayerPreferences.SetString("test1", RandomString(100, 10));
            // PlayerPreferences.SetString("test2", RandomString(100, 10));
            // PlayerPreferences.SaveAsync();
            // PlayerPreferences.SetString("test3", RandomString(100, 10));
            // PlayerPreferences.SaveAsync();

            // TestCustomPlayerPrefs("_two");
        }

        private void TestUnityPlayerPrefs(string testID)
        {
            string key = PLAYER_PREF_KEY + testID; //pp_pref_test_one | pp_pref_test_two
            int value = PlayerPrefs.GetInt(key, 0);

            if (value == 0)
            {
                if (TrySaveData(key, 1, out string error))
                {
                    SenFirebaseEvent("unity_pp_f_test" + testID, "value", GetCurrentTime());
                    AnalyticsManager.LogEvent("unity_pp_test" + testID, "value", GetCurrentTime());
                }
                else
                {
                    SenFirebaseEvent("unity_pref_f_error" + testID, "value", error.GetFirstNCharacters(100));
                    AnalyticsManager.LogEvent("unity_pref_error" + testID, "value", error.GetFirstNCharacters(100));
                }
            }
        }

        // private void TestCustomPlayerPrefs(string testTimeString)
        // {
        //     string key = PLAYER_PREF_KEY + testTimeString; //pp_pref_test_one | pp_pref_test_two
        //     long value = PlayerPreferences.GetLong(key, 0);
        //     if (value == 0)
        //     {
        //         PlayerPreferences.SetLong(key, 1);
        //         PlayerPreferences.SaveAsync((string error) =>
        //         {
        //             if (string.IsNullOrEmpty(error))
        //             {
        //                 SenFirebaseEvent("custom_pp_f_test" + testTimeString, "value", GetCurrentTime());
        //                 AnalyticsManager.LogEvent("custom_pp_test" + testTimeString, "value", GetCurrentTime());
        //             }
        //             else
        //             {
        //                 SenFirebaseEvent("custom_pref_f_error" + testTimeString, "value", error.GetFirstNCharacters(100));
        //                 AnalyticsManager.LogEvent("custom_pref_error" + testTimeString, "value", error.GetFirstNCharacters(100));
        //             }
        //         });
        //     }
        // }

        private string GetCurrentTime()
        {
            DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
            return now.ToUnixTimeMilliseconds().ToString();
        }

        private void SenFirebaseEvent(string eventName, string parameterName, string value)
        {
#if ACTIVATE_FIREBASE
            try
            {
                FirebaseAnalytics.LogEvent(eventName, parameterName, value);
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable To Write Data, Error {e}");
            }
#endif
        }

        public bool TrySaveData(string key, int value, out string error)
        {
            try
            {
                PlayerPrefs.SetInt(key, value);
                PlayerPrefs.Save();
                error = null;
                return true;
            }
            catch (PlayerPrefsException e)
            {
                Debug.Log($"Unable To Write Data, Allotted storage space, Error {e}");
                error = "Allotted storage space: Error " + e.Message;
                return false;
            }
            catch (Exception e)
            {
                Debug.Log($"Unable To Write Data, Error {e}");
                error = "Unknown Error: " + e.Message;
                return false;
            }
        }


        public void TestPreferencesInEditor()
        {
            PlayerPreferences.Initialize();
            PlayerPreferences.LogState();


            PlayerPreferences.SaveAsync();

            PlayerPreferences.SetBool("IsFirstLaunch", false);
            PlayerPreferences.SetLong("Value", 173);
            PlayerPreferences.SetDouble("Float_Value", 2.323f);
            PlayerPreferences.SetString("name", "Bebi");
            PlayerPreferences.SetString("test", RandomString(100, 10));

            PlayerPreferences.SetString("game", "sadfasdddddddddd  asd asdma slkdaksdmopaksmdpalkmspdmas;dl,a; lsmdpasklmd al,smdpalms,d; aspdma psld, ;als,dmpa msd;aspdlma, ps l d,a;lsd, p[aswd, ;als,d");

            PlayerPreferences.LogState();

            PlayerPreferences.SaveAsync();
            PlayerPreferences.SaveAsync();
            PlayerPreferences.SaveAsync();
        }


        public static string RandomString(int length, int repeat)
        {
            try
            {
                StringBuilder str_build = new StringBuilder(length * repeat);
                System.Random random = new System.Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                char[] char_array = chars.ToCharArray();
                int charLength = char_array.Length;

                for (int i = 0; i < length; i++)
                {
                    str_build.Append(char_array[random.Next(charLength)], repeat);
                }
                return str_build.ToString();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return "lorem ipsum dolor sit amet";
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SerializationTester))]
    public class SerializationTesterEditor : Editor
    {
        private SerializationTester _serializationTester;
        private bool _foldout = false;
        private bool _saveOnChange = false;
        private Vector2 _scrollPosition = Vector2.zero;

        private void OnEnable()
        {
            _serializationTester = (SerializationTester)target;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Test Custom Preferences"))
            {
                _serializationTester.TestPreferencesInEditor();
            }

            if (GUILayout.Button("Clear Custom Preferences"))
            {
                PlayerPreferences.DeleteAll();
                PlayerPreferences.SaveAsync();
            }

            _foldout = EditorGUILayout.Foldout(_foldout, "Game Preferences");
            if (!_foldout) return;

            _saveOnChange = EditorGUILayout.Toggle("Save On Change", _saveOnChange);

            EditorGUI.indentLevel++;
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUI.BeginChangeCheck();
            DrawPreferences();
            if (EditorGUI.EndChangeCheck() && _saveOnChange)
            {
                PlayerPreferences.SaveAsync();
            }

            EditorGUILayout.EndScrollView();
            EditorGUI.indentLevel--;
        }

        private static void DrawPreferences()
        {
            foreach (KeyValuePair<string, Preference> item in PlayerPreferences.GamePreferences)
            {
                if (item.Value.PreferenceType == PreferenceDataType.Bool)
                {
                    item.Value.BoolValue = EditorGUILayout.Toggle(item.Key, (bool)item.Value.BoolValue);
                }
                else if (item.Value.PreferenceType == PreferenceDataType.Long)
                {
                    item.Value.LongValue = EditorGUILayout.LongField(item.Key, item.Value.LongValue);
                }
                else if (item.Value.PreferenceType == PreferenceDataType.Double)
                {
                    item.Value.DoubleValue = EditorGUILayout.DoubleField(item.Key, item.Value.DoubleValue);
                }
                else if (item.Value.PreferenceType == PreferenceDataType.String)
                {
                    item.Value.StringValue = EditorGUILayout.TextField(item.Key, (string)item.Value.StringValue);
                }
            }
        }
    }
#endif
}
