using UnityEngine;
using MoreMountains.Tools;
using ModelShark;

// This script can be added on to power generators
// The Output and Range can be set
// This will alow allow you to power on / off the generator

[RequireComponent(typeof(PowerAnimator))]
[RequireComponent(typeof(Hoverable))]
public class PowerGenerator : MonoBehaviour {
    [SerializeField] float output;
    // public float range;
    [SerializeField] bool isOn = false;
    [SerializeField] MMLineRendererCircle lineRenderer;

    [Header("Debug")]
    [SerializeField] bool toggle;

    [SerializeField] PowerAnimator powerAnimator;
    [SerializeField] TooltipTrigger tooltipTrigger;
    [SerializeField] PowerRange powerRange;


    void Start() {
        SetTooltip();
        Switch(true);
    }

    void Update() {
        if (toggle && !powerAnimator.IsAnimating()) {
            isOn = !isOn;
            powerAnimator.Switch(isOn);
            toggle = false;
        }
    }

    public float GetRange() {
        return powerRange.range;
    }

    // Returns false if the animation is still underway
    public bool Switch(bool hasPower) {
        if (powerAnimator.IsAnimating()) { return false; }
        powerAnimator.Switch(hasPower);
        isOn = hasPower;
        return true;
    }

    public void SetOutput(float newOutput) {
        output = newOutput;
    }

    public float GetOutput() {
        return isOn ? output : 0;
    }

    void SetTooltip() {
        string text = "Output: " + output + "\nRange: " + GetRange();
        tooltipTrigger.SetText("BodyText", text);
    }
}
