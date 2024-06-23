using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeCoverSizer : MonoBehaviour
{
    public RectTransform RT_Line1;
    public RectTransform RT_Line2;
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        Vector2 size = this.RT_Line2.sizeDelta;
        size.x = (Screen.width * 1536.0f / Screen.height) - 520.0f;
        //size.x = Screen.width - 520.0f;
        this.RT_Line2.sizeDelta = size;
        this.RT_Line2.anchoredPosition = new Vector2(520f, 0.0f);
    }
}
