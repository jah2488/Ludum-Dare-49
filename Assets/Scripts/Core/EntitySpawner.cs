using UnityEngine;
using UnityEngine.EventSystems;

public class EntitySpawner : MonoBehaviour {
    [SerializeField] GameObject generatorPreview;
    [SerializeField] GameObject generator;

    [SerializeField] GameObject distributorPreview;
    [SerializeField] GameObject distributor;

    public UnityEngine.Events.UnityEvent<GameObject> onGeneratorSpawned;
    public UnityEngine.Events.UnityEvent<GameObject> onDistributorSpawned;

    GameObject currentPreview;
    GameObject currentEntity;

    Camera _camera;

    void Start() {
        _camera = Camera.main;
    }

    void Update() {
        if (currentPreview == null) { return; }
        Cursor.visible = false;
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity)) {
            Debug.DrawLine (castPoint.origin, hit.point, Color.red);
            currentPreview.transform.position = new Vector3(hit.point.x, currentPreview.transform.position.y, hit.point.z);

            if (hit.point.y > 0.6f || hit.collider.gameObject.tag == "NoSpawn") {
                var renderers = currentPreview.gameObject.GetComponentsInChildren<Renderer>();
                foreach(var r in renderers) {
                    r.material.color = Color.red;
                }
            } else {
                var renderers = currentPreview.gameObject.GetComponentsInChildren<Renderer>();
                foreach(var r in renderers) {
                    r.material.color = Color.white;
                }
                if (Input.GetMouseButtonDown(0)) {
                    SpawnEntity();
                }
            }
        }
    }

    public void ShowGeneratorPreview() {
        currentEntity = generator;
        currentPreview = Instantiate(generatorPreview);
    }

    public void SpawnDistributorPreview() {
        currentEntity = distributor;
        currentPreview = Instantiate(distributorPreview);
    }

    void SpawnEntity() {
        GameObject entity = Instantiate(currentEntity);
        entity.transform.position = currentPreview.transform.position;
        GameObject.Destroy(currentPreview);
        currentPreview = null;
        currentEntity = null;
        Cursor.visible = true;
        Emit(entity);
    }

    void Emit(GameObject entity) {
        if (entity.GetComponent<PowerGenerator>() != null) {
            onGeneratorSpawned?.Invoke(entity);
        } else {
            onDistributorSpawned?.Invoke(entity);
        }
    }
}
