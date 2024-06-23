using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class ItemSwing : MonoBehaviour
    {
        public bool active;
        public bool horizontal;
        public float power = 0.2f;
        public float speed = 1.0f;

        private Vector2 _v;
        private float _time = 0.0f;
        private Vector2 _defaultPosition;

        private void Awake()
        {

            _defaultPosition = this.transform.position;
            _v = _defaultPosition;
        }

        void Update()
        {
            if (!this.active)
            {
                return;
            }

            if (this.horizontal)
            {
                _v.x = _defaultPosition.x + Mathf.Sin(_time) * this.power;
            }
            else
            {
                _v.y = _defaultPosition.y + Mathf.Sin(_time) * this.power;
            }
            _time += (Time.deltaTime * this.speed);

            this.transform.position = _v;
        }
    }
}
