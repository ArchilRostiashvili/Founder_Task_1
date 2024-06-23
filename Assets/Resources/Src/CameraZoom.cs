using System;
using Cinemachine.Utility;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private float _zoomSpeed;

    private Vector3 _targetPosition;
    private Vector3 _initialCameraPosition;
    private float _initialCameraOrthoSize;

    private bool _isZooming = false;

    public void ZoomCamera(Vector3 zoomPosition)
    {
        _initialCameraPosition = _cam.transform.position;
        _initialCameraOrthoSize = _cam.orthographicSize;
        _targetPosition = new Vector3(zoomPosition.x, zoomPosition.y, -10f);

        _isZooming = true;
    }

    public void ResetCamera()
    {
        _isZooming = false;
        _cam.transform.position = _initialCameraPosition;
        _cam.orthographicSize = _initialCameraOrthoSize;
    }

    private void Update()
    {
        if (!_isZooming)
            return;

        _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, 3, _zoomSpeed * Time.deltaTime);
        _cam.transform.position = Vector3.Lerp(_cam.transform.position, _targetPosition, _zoomSpeed * 2 * Time.deltaTime);
    }
}