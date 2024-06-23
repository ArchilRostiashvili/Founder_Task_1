using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemFire : MonoBehaviour
{
    public Collider2D colliderFire;
    public SpriteRenderer SR;
    public int state = 0;
    [HideInInspector]
    public bool clicked;
    public void ResetContent()
    {
        this.SR.color = Color.white;
        this.gameObject.SetActive(true);
        this.clicked = false;
    }

    public void PutOut()
    {
        this.state = 0;
        this.gameObject.SetActive(false);
    }
}
