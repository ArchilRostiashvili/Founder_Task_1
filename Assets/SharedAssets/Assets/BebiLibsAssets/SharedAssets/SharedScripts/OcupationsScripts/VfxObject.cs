using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT_Occupations.General
{
    public class VfxObject : MonoBehaviour
    {
        public Vector3 initLocalPos = Vector3.zero;
        public Vector3 initEulaerAngles = Vector3.zero;
        public Vector3 initLocalScale = Vector3.one;
        private Transform _transform;
        private int _deltaOreder = 0;
        private bool _isCurrentlyActive;
        [SerializeField] private List<ParticleSystemRenderer> _particleSystemRenderers;
        [SerializeField] private List<ParticleSystem> _paticles;

        public bool GetIsCurrentlyActive => _isCurrentlyActive;

        public void Initialize(int deltaOrder = 0, bool isLooping = false, float period = 2.0f, string sortingOrderName = "Default")
        {
            StopAllCoroutines();
            SetValues();
            _deltaOreder = deltaOrder;
            ChangeOrders(_deltaOreder, sortingOrderName);
            gameObject.SetActive(true);
            StartCoroutine(PlayRoutine(isLooping));
            if (!isLooping)
                StartCoroutine(DisableAfterLifeTime(period));
        }

        IEnumerator PlayRoutine(bool isLooping)
        {
            _isCurrentlyActive = true;
            yield return null;
            PlayParticles(isLooping);
        }

        void SetValues()
        {
            if (_transform == default)
                _transform = GetComponent<Transform>();
            _transform.localPosition = initLocalPos;
            _transform.localScale = initLocalScale;
            _transform.eulerAngles = initEulaerAngles;
        }

        void PlayParticles(bool loop)
        {
            foreach (var particle in _paticles)
            {
                var main = particle.main;
                main.loop = loop;
                particle.Play();
            }
        }

        void ChangeOrders(int deltaOrder, string orderName = "Default")
        {
            CheckForParticles();
            foreach (var particleSystemRenderer in _particleSystemRenderers)
            {
                particleSystemRenderer.sortingLayerName = orderName;
                particleSystemRenderer.sortingOrder += deltaOrder;
            }
        }

        void CheckForParticles()
        {
            if (_particleSystemRenderers != default)
                if (_particleSystemRenderers.Count > 0)
                    return;
            _paticles = GetComponentsInChildren<ParticleSystem>().ToList();
            foreach (var particle in _paticles)
            {
                var main = particle.main;
                main.playOnAwake = false;
                main.loop = false;
            }

            _particleSystemRenderers = _paticles.Select
            (
                i =>
                    i.GetComponent<ParticleSystemRenderer>()
            ).ToList();
        }


        public void Disable()
        {
            _isCurrentlyActive = false;
            ResetValues();
            transform.localScale = Vector3.one;
            transform.SetParent(null);
            Activate(false);
        }

        public void ResetValues()
        {
            initLocalPos = Vector3.zero;
            initEulaerAngles = Vector3.zero;
            initLocalScale = Vector3.one;
        }

        private void OnDisable()
        {
            ChangeOrders(-_deltaOreder);
        }

        public void Activate(bool activate) => gameObject.SetActive(activate);

        IEnumerator DisableAfterLifeTime(float period)
        {
            yield return new WaitForSeconds(period);
            Disable();
        }
    }
}