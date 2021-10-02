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
    PowerAnimator powerAnimator;

    TooltipTrigger _tooltipTrigger;

    void Start() {
        powerAnimator = GetComponent<PowerAnimator>();
        _tooltipTrigger = GetComponent<TooltipTrigger>();
        SetTooltip();
    }

    public void OnPointerDown(PointerEventData _) { }

    public void OnPointerUp(PointerEventData _) {
        Switch(!isOn);
    }

    public bool Switch(bool hasPower) {
        if (GetPowerAnimator().IsAnimating()) { return false; }
        GetPowerAnimator().Switch(hasPower);
        isOn = hasPower;
        return true;
    }

    PowerAnimator GetPowerAnimator() {
        if (powerAnimator) {
            return powerAnimator;
        } else {
            var pa = GetComponent<PowerAnimator>();
            powerAnimator = pa;
            return GetComponent<PowerAnimator>();
        }
    }

    void SetTooltip() {
        string text = "Status: " + (isOn ? "On" : "Off");
        _tooltipTrigger.SetText("BodyText", text);
    }
}