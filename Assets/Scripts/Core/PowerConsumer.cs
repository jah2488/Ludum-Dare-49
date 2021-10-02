using UnityEngine;
using System.Collections;
using MoreMountains.Feedbacks;
using ModelShark;

// This script can be placed on house game objects
// It will consume power set in `powerRequired`
// SetPower can be called (somehow) to know if there is enough power

public class PowerConsumer : MonoBehaviour {
    [SerializeField] float powerRequired;
    [SerializeField] bool hasPower;

    [SerializeField] Material onMaterial;
    [SerializeField] Material offMaterial;

    [SerializeField] MeshRenderer _light;

    [SerializeField] MMFeedbacks _flickerFeedback;

    [Header("Debug")]
    [SerializeField] bool toggle;

    TooltipTrigger _tooltipTrigger;

    void Start() {
        _tooltipTrigger = GetComponent<TooltipTrigger>();
        SetTooltip();
    }

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

    public bool ConnectedToPower() {
        return hasPower;
    }

    public float PowerRequired { get { return powerRequired; } }

    public void SetPowerRequired(float power) {
        powerRequired = power;
    }

    public bool HasPower { get { return hasPower; } }

    public void SetPower(bool hasPower) {
        if (hasPower) {
            StartCoroutine(TurnOnCoroutine());
        } else {
            StartCoroutine(TurnOffCoroutine());
        }
        this.hasPower = hasPower;
    }

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

    void SetTooltip() {
        string text = "Demand: " + powerRequired;
        _tooltipTrigger.SetText("BodyText", text);
    }
}
