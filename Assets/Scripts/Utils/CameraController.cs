using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour {

    [SerializeField] CinemachineVirtualCamera cam;
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;
    [SerializeField] float panSpeed;

    void Update() {
        float zoomLevel = cam.m_Lens.OrthographicSize;
        // WASD Panning
        if (Input.GetKey(KeyCode.W)) {
            transform.Translate(new Vector3(-1, 0, -1) * Time.deltaTime * zoomLevel * panSpeed);
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.Translate(new Vector3(1, 0, 1) * Time.deltaTime * zoomLevel * panSpeed);
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.Translate(new Vector3(1, 0, -1) * Time.deltaTime * zoomLevel * panSpeed);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(new Vector3(-1, 0, 1) * Time.deltaTime * zoomLevel * panSpeed);
        }

        // Scrollwheel Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0) {
            cam.m_Lens.OrthographicSize -= scroll * 10;
            if (cam.m_Lens.OrthographicSize < minZoom) {
                cam.m_Lens.OrthographicSize = minZoom;
            }
            if (cam.m_Lens.OrthographicSize > maxZoom) {
                cam.m_Lens.OrthographicSize = maxZoom;
            }
        }
    }
}
