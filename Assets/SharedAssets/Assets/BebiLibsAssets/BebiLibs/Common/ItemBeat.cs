using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class ItemBeat : MonoBehaviour
    {
        // Start is called before the first frame update

        private void OnEnable()
        {
            this.Activate();
        }

        private void OnDisable()
        {
            this.Stop();
        }

        public void Activate()
        {
            this.transform.DOScale(1.1f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        }

        public void Stop()
        {
            this.transform.DOKill();
            this.transform.localScale = Vector3.one;
        }
    }
}
