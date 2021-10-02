using UnityEngine;
using System.Collections;
using MoreMountains.Feedbacks;

public class PowerConsumer : MonoBehaviour {
    [SerializeField] bool toggle;

    [SerializeField] float powerRequired;
    [SerializeField] bool hasPower;

    [SerializeField] Material onMaterial;
    [SerializeField] Material offMaterial;

    [SerializeField] MeshRenderer _light;

    [SerializeField] MMFeedbacks _flickerFeedback;

    void Update() {
        if (toggle) {
            hasPower = !hasPower;
            if (hasPower) {
                StartCoroutine(TurnOnCoroutine());
            } else {
                StartCoroutine(TurnOffCoroutine());
            }
            toggle = false;
        }
    }

    public float PowerRequired { get { return powerRequired; } }

    public void SetPower(bool hasPower) {
        if (hasPower) {
            StartCoroutine(TurnOnCoroutine());
        } else {
            StartCoroutine(TurnOffCoroutine());
        }
        this.hasPower = hasPower;
    }

    public bool HasPower { get { return hasPower; } }

    IEnumerator TurnOnCoroutine() {
        _flickerFeedback.PlayFeedbacks();
        yield return new WaitForSeconds(0.75f);
        _light.material = onMaterial;
    }

    IEnumerator TurnOffCoroutine() {
        _flickerFeedback.PlayFeedbacks();
        yield return new WaitForSeconds(0.75f);
        _light.material = offMaterial;
    }
}
