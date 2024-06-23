using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class LoadingDots : MonoBehaviour
    {
        public GameObject[] arrayDots;
        public float speed;
        private Sequence _s;

        void Awake()
        {
            for (int i = 0; i < this.arrayDots.Length; i++)
            {
                this.arrayDots[i].SetActive(false);
            }
        }

        public void Stop()
        {
            if (_s != null)
            {
                _s.Kill();
                _s = null;
            }

            for (int i = 0; i < this.arrayDots.Length; i++)
            {
                this.arrayDots[i].SetActive(false);
            }
        }


        public void Play()
        {
            this.Stop();

            _s = DOTween.Sequence();
            _s.AppendCallback(() =>
            {
                this.arrayDots[0].SetActive(true);
                this.arrayDots[1].SetActive(false);
                this.arrayDots[2].SetActive(false);
            });
            _s.AppendInterval(this.speed);
            _s.AppendCallback(() =>
            {
                this.arrayDots[0].SetActive(true);
                this.arrayDots[1].SetActive(true);
                this.arrayDots[2].SetActive(false);
            });
            _s.AppendInterval(this.speed);
            _s.AppendCallback(() =>
            {
                this.arrayDots[0].SetActive(true);
                this.arrayDots[1].SetActive(true);
                this.arrayDots[2].SetActive(true);
            });
            _s.AppendInterval(this.speed);
            _s.AppendCallback(() =>
            {
                this.arrayDots[0].SetActive(false);
                this.arrayDots[1].SetActive(false);
                this.arrayDots[2].SetActive(false);
            });
            _s.AppendInterval(this.speed);
            _s.SetLoops(-1);
        }
    }
}
