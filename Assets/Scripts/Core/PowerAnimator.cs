using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is a helper class for power generators and distributors that
// lights them up and dims them down

public class PowerAnimator : MonoBehaviour {
    [SerializeField] bool isOn;

    [SerializeField] float waitTime;
    [SerializeField] Material onMaterial;
    [SerializeField] Material offMaterial;
    [SerializeField] List<MeshRenderer> lights;
    [SerializeField] GameObject pointLight;

    [Header("Debug")]
    [SerializeField] bool toggle;

    bool animating = false;

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

    public bool IsAnimating() {
        return animating;
    }

    IEnumerator TurnOnCoroutine() {
        animating = true;
        foreach (MeshRenderer light in lights) {
            light.material = onMaterial;
            yield return new WaitForSeconds(waitTime);
        }
        pointLight?.SetActive(true);
        animating = false;
    }

    IEnumerator TurnOffCoroutine() {
        animating = true;
        pointLight?.SetActive(false);
        for (int i = lights.Count - 1; i >= 0; i--) {
            lights[i].material = offMaterial;
            yield return new WaitForSeconds(waitTime);
        }
        animating = false;
    }
}
