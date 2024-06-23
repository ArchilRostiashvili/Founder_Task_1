using UnityEngine;

namespace FarmLife
{
    public class ItemCloud : MonoBehaviour
    {
        private Transform _startTR;
        private Transform _endTR;

        private float _movementSpeed;

        private bool _isActive = false;

        public void SetData(Transform pointA, Transform pointB, float speed)
        {
            _startTR = pointA;
            _endTR = pointB;
            _movementSpeed = speed;
        }

        public void Move()
        {
            if (!_isActive)
                return;
            transform.localPosition = new Vector3(transform.localPosition.x - _movementSpeed * Time.deltaTime, transform.localPosition.y, transform.localPosition.z);

            if (transform.localPosition.x < _endTR.localPosition.x)
            {
                transform.localPosition = new Vector3(_startTR.localPosition.x, transform.localPosition.y, _startTR.localPosition.z);
            }

            transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -9999f, _startTR.localPosition.x), transform.localPosition.y, transform.localPosition.z);
        }

        public void Activate()
        {
            _isActive = true;
        }

        public void Stop()
        {
            _isActive = false;
        }
    }
}