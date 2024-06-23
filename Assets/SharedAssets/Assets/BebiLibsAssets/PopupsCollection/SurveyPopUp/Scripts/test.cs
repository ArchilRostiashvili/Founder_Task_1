using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;
using Survey;
public class test : MonoBehaviour
{
    public SurveyPopUp survey;
    private void Start()
    {
        survey.Init();
        SurveyPopUp.Activate(string.Empty, string.Empty);
    }
}
