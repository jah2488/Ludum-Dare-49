using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerAnimator : MonoBehaviour {
    [SerializeField] bool toggle;
    [SerializeField] bool isOn;

    [SerializeField] float waitTime;
    [SerializeField] Material onMaterial;
    [SerializeField] Material offMaterial;
    [SerializeField] List<MeshRenderer> lights;

    void Update() {
        if (toggle) {
            isOn = !isOn;
            Switch(isOn);
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

    IEnumerator TurnOnCoroutine() {
        foreach (MeshRenderer light in lights) {
            light.material = onMaterial;
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator TurnOffCoroutine() {
        for (int i = lights.Count - 1; i >= 0; i--) {
            lights[i].material = offMaterial;
            yield return new WaitForSeconds(waitTime);
        }
    }
}
