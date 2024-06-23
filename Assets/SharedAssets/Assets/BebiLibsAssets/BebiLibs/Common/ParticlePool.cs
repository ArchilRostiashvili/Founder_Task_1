using BebiLibs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class ParticlePool : MonoBehaviour
    {
        [SerializeField] GameObject ParticleEffect;
        [SerializeField] float decrementOnIdleTime = 10f;
        [SerializeField] int prefillCount;

        private List<ItemParticleSystem> _arrayParticlePool = new List<ItemParticleSystem>();
        private float _currentTimer;

        private void Start()
        {
            this.PrefillParticles();
        }

        private void PrefillParticles()
        {
            for (int i = 0; i < this.prefillCount; i++)
            {
                this.AddParticleToPool();
            }
        }

        private ItemParticleSystem AddParticleToPool()
        {
            ItemParticleSystem particleSystem = Instantiate(this.ParticleEffect, Vector3.zero, Quaternion.identity).GetComponent<ItemParticleSystem>();
            particleSystem.transform.SetParent(this.transform);
            _arrayParticlePool.Add(particleSystem);
            particleSystem.gameObject.SetActive(false);

            return particleSystem;
        }

        public ItemParticleSystem GetParticle()
        {
            _currentTimer = this.decrementOnIdleTime;
            ItemParticleSystem result = null;

            if (_arrayParticlePool.Count == 0 || _arrayParticlePool.FindAll(x => !x.gameObject.activeSelf).Count == 0)
            {
                result = this.AddParticleToPool();
            }
            else
            {
                result = _arrayParticlePool.Find(x => !x.gameObject.activeSelf);
            }

            return result;
        }

        private void DecrementPool()
        {
            if (_arrayParticlePool.Count == 0) return;

            GameObject particleToRemove = _arrayParticlePool[_arrayParticlePool.Count - 1].gameObject;
            _arrayParticlePool.RemoveAt(_arrayParticlePool.Count - 1);
            Destroy(particleToRemove);
        }

        private void Update()
        {
            if (_currentTimer > 0f)
            {
                _currentTimer -= Time.deltaTime;
                if (_currentTimer <= 0)
                {
                    this.DecrementPool();
                    _currentTimer = this.decrementOnIdleTime;
                }
            }
        }
    }
}

