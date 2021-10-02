using UnityEngine;
using UnityEngine.EventSystems;
using ModelShark;

// This is a script that can be attached to pylons
// Clicking on the pylon toggles it on / off

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PowerAnimator))]
public class PowerDistributor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public bool isOn = false;
    public float range = 5f;

    [SerializeField] PowerAnimator powerAnimator;

    [SerializeField] TooltipTrigger _tooltipTrigger;

    [SerializeField] PowerRange powerRange;

    void Start() {
        SetTooltip();
    }

    public void OnPointerDown(PointerEventData _) { }

    public void OnPointerUp(PointerEventData _) {
        Switch(!isOn);
    }

    public bool Switch(bool hasPower) {
        if (powerAnimator.IsAnimating()) { return false; }
        powerAnimator.Switch(hasPower);
        isOn = hasPower;
        SetTooltip();
        return true;
    }

    void SetTooltip() {
        string text = "Status: " + (isOn ? "On" : "Off");
        _tooltipTrigger.SetText("BodyText", text);
    }
}