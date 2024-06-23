using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleElement : MonoBehaviour
{
    public Animator anim;

    public float time;

    public void Play()
    {
        this.gameObject.SetActive(true);
        this.anim.SetTrigger("Boom");

        this.StartCoroutine(this.DelayHide());
    }

    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(this.time);
        this.Stop();
    }

    public void Stop()
    {
        this.StopAllCoroutines();
        this.gameObject.SetActive(false);
    }

}
