using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;
using System;
using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SDateTimeTester : MonoBehaviour
{

    public void TestSDateTime()
    {
        //DateTime dateTime = new DateTime(2022, 10, 1, 4, 30, 35);
        DateTime dateTime = DateTime.Now;

        Debug.Log(dateTime + " " + dateTime.Kind);
        SDateTime date = dateTime;
        Debug.Log(date + " " + dateTime.Kind);
        //long epoch = date.GetEpochTime();
        //SDateTime newDate = SDateTime.FromString(date.ToString(), DateTimeKind.Utc);
        SDateTime newDate = SDateTime.FromString(date.ToString(), DateTimeKind.Local);
        Debug.Log(newDate + " " + newDate.Kind + " " + newDate.DateTime.Kind);

        DateTime nextDateTime = (DateTime)newDate;
        Debug.Log(nextDateTime + " " + nextDateTime.Kind);

        Debug.Log(nextDateTime == dateTime);

        string json = JsonConvert.SerializeObject(newDate);
        Debug.Log(json);

        SDateTime dateFromJson = JsonConvert.DeserializeObject<SDateTime>(json);
        Debug.Log(dateFromJson + " " + dateFromJson.Kind);
    }


}


#if UNITY_EDITOR
[CustomEditor(typeof(SDateTimeTester))]
public class SDateTimeTesterEditor : Editor
{
    private SDateTimeTester _sDateTimeTester;

    private void OnEnable()
    {
        _sDateTimeTester = (SDateTimeTester)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Test"))
        {
            _sDateTimeTester.TestSDateTime();
        }
    }
}
#endif