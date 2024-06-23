using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BalloonAdventure
{    
    public class ItemCandy : MonoBehaviour
    {
        public SpriteRenderer SR;
        private Vector3 _rotation;
        private Vector3 _position;
    
        public float powerRotation;
        public float speedRotation;
    
        public float gravityY;
        private float _accelerationY;
    
        private float _timeValue;
    
        public bool active;
    
        public void Activate(Sprite sp, Vector3 p)
        {
            this.gameObject.SetActive(true);
            this.SR.sprite = sp;
            _rotation = Vector3.zero;
            _timeValue = UnityEngine.Random.Range(0.0f, 4.5f);
            _rotation.z = Mathf.Sin(_timeValue) * this.powerRotation;
    
            _accelerationY = 0.0f;
            _position = p;
            this.transform.position = p;
            this.active = true;
            this.transform.localScale = Vector3.one * 1.2f;
        }
    
        public void Hide()
        {
            this.gameObject.SetActive(false);
            this.active = false;
        }
    
        public void EnterFrame()
        {
            if (!this.active)
            {
                return;
            }
    
            _timeValue += Time.deltaTime * this.speedRotation;
            _rotation.z = Mathf.Sin(_timeValue) * this.powerRotation;
            this.SR.transform.eulerAngles = _rotation;
    
            _accelerationY += this.gravityY * Time.deltaTime;
            _position.y += _accelerationY;
            this.transform.position = _position;
        }
    }
}