using UnityEngine;
using UnityEngine.EventSystems;
using ModelShark;

// This is a script that can be attached to pylons
// Clicking on the pylon toggles it on / off

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PowerAnimator))]
public class PowerDistributor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    [SerializeField] bool isOn = false;

    PowerAnimator powerAnimator;

    TooltipTrigger _tooltipTrigger;

    void Start() {
        powerAnimator = GetComponent<PowerAnimator>();
        _tooltipTrigger = GetComponent<TooltipTrigger>();
        SetTooltip();
        Switch(true);
    }

    public void OnPointerDown(PointerEventData _) { }

    public void OnPointerUp(PointerEventData _) {
        Switch(!isOn);
    }

    public bool Switch(bool hasPower) {
        if (powerAnimator.IsAnimating()) { return false; }
        powerAnimator.Switch(hasPower);
        isOn = hasPower;
        return true;
    }

    void SetTooltip() {
        string text = "Status: " + (isOn ? "On" : "Off");
        _tooltipTrigger.SetText("BodyText", text);
    }
}