using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    private Camera _camera;
    private EventSystem _eventSystem;

    private bool isRotating;
    private Vector3 rotatingPos;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _eventSystem = EventSystem.current;
    }

    public void AdjustCamera(int width, int height)
    {
        var x = Mathf.Max(width, height);

        _camera.orthographicSize = 0.4f * x + 0.5f;

        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, 0.5f, 50f);
    }

    private void Update()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            _camera.orthographicSize += -scroll * Time.deltaTime * 75 * (_camera.orthographicSize / 4);

            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, 0.5f, 50f);
        }

        if (Input.GetMouseButtonDown(0) && !isRotating && _eventSystem.currentSelectedGameObject == null)
        {
            // Start rotation
            rotatingPos = Input.mousePosition;
            isRotating = true;
        }
        else if (Input.GetMouseButtonUp(0) && isRotating)
        {
            // Stop rotation
            isRotating = false;
        }
        else if (isRotating)
        {
            // Rotate
            var mousePos = Input.mousePosition;
            var offset = mousePos.x - rotatingPos.x;

            rotatingPos = mousePos;

            transform.RotateAround(Vector3.zero, Vector3.up, offset * Time.deltaTime * 5);
        }
    }
}