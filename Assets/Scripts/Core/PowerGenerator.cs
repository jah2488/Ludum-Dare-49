using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerGenerator : MonoBehaviour {
    [SerializeField] float output;
    [SerializeField] bool isOn;
    [SerializeField] bool toggle;

    [SerializeField] Material onMaterial;
    [SerializeField] Material offMaterial;
    [SerializeField] List<MeshRenderer> lights;

    void Update() {
        if (toggle) {
            isOn = !isOn;
            if (isOn) {
                StartCoroutine(TurnOnCoroutine());
            } else {
                StartCoroutine(TurnOffCoroutine());
            }
            toggle = false;
        }
    }

    public void Switch(bool status) {
        if (status) {
            StartCoroutine(TurnOnCoroutine());
        } else {
            StartCoroutine(TurnOffCoroutine());
        }
        isOn = status;
    }

    public float GetOutput() {
        return isOn ? output : 0;
    }

    IEnumerator TurnOnCoroutine() {
        foreach (MeshRenderer light in lights) {
            light.material = onMaterial;
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator TurnOffCoroutine() {
        for (int i = lights.Count - 1; i >= 0; i--) {
            lights[i].material = offMaterial;
            yield return new WaitForSeconds(0.3f);
        }
    }
}
