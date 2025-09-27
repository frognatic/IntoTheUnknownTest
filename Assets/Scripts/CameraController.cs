using UnityEngine;

namespace IntoTheUnknownTest
{
    public class CameraController : MonoBehaviour
    {
        [Header("Move Settings")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _moveSmoothing = 10f; 

        [Header("Bounds")]
        [SerializeField] private Vector2 _xBounds = new Vector2(-10f, 10f);
        [SerializeField] private Vector2 _yBounds = new Vector2(-10f, 10f);
        [SerializeField] private Vector2 _zoomBounds = new Vector2(2f, 10f);

        [Header("Zoom Settings")]
        [SerializeField] private float _zoomStep = 0.5f;
        [SerializeField] private float _smoothTime = 0.2f;
        
        private Camera _cam;
        private CameraControls _cameraControls;
        private Vector3 _targetPosition;
        private float _targetZoom;
        private float _zoomVelocity;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _cameraControls = new CameraControls();

            _targetPosition = transform.position;
            _targetZoom = _cam.orthographicSize;
        }

        private void OnEnable()
        {
            _cameraControls.Camera.Enable();
        }

        private void OnDisable()
        {
            _cameraControls.Camera.Disable();
        }

        private void LateUpdate()
        {
            HandleMovement();
            HandleZoom();
        }

        private void HandleMovement()
        {
            Vector2 moveInput = _cameraControls.Camera.Move.ReadValue<Vector2>();
            Vector3 moveDelta = new Vector3(moveInput.x, moveInput.y, 0) * (_moveSpeed * Time.deltaTime);
        
            _targetPosition += moveDelta;

            _targetPosition.x = Mathf.Clamp(_targetPosition.x, _xBounds.x, _xBounds.y);
            _targetPosition.y = Mathf.Clamp(_targetPosition.y, _yBounds.x, _yBounds.y);

            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _moveSmoothing);
        }
        
        private void HandleZoom()
        {
            float scroll = _cameraControls.Camera.Zoom.ReadValue<float>();
            
            _targetZoom -= scroll * _zoomStep;
            _targetZoom = Mathf.Clamp(_targetZoom, _zoomBounds.x, _zoomBounds.y);
            
            _cam.orthographicSize = Mathf.SmoothDamp(_cam.orthographicSize, _targetZoom, ref _zoomVelocity, _smoothTime);
        }
    }
}
