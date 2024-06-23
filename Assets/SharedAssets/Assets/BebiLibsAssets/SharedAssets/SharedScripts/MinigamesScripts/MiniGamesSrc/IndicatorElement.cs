using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorElement : MonoBehaviour
{
    public GameObject GO_Active;
    public GameObject GO_Passive;
    
    public void Activate(bool bl)
    {
        this.GO_Active.SetActive(bl);
        this.GO_Passive.SetActive(!bl);
    }
}
