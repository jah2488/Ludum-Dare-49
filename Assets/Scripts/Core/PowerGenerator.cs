using UnityEngine;
using MoreMountains.Tools;

// This script can be added on to power generators
// The Output and Range can be set
// This will alow allow you to power on / off the generator

[RequireComponent(typeof(PowerAnimator))]
public class PowerGenerator : MonoBehaviour {
    [SerializeField] float output;
    [SerializeField] float range;
    [SerializeField] bool isOn = false;
    [SerializeField] MMLineRendererCircle lineRenderer;

    [Header("Debug")]
    [SerializeField] bool toggle;

    PowerAnimator powerAnimator;

    void Start() {
        powerAnimator = GetComponent<PowerAnimator>();
        Switch(true);
        UpdateRange(range);
    }

    void Update() {
        if (toggle && !powerAnimator.IsAnimating()) {
            isOn = !isOn;
            powerAnimator.Switch(isOn);
            toggle = false;
        }
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

    public void UpdateRange(float range) {
        this.range = range;
        lineRenderer.HorizontalRadius = lineRenderer.VerticalRadius = range;
        lineRenderer.DrawCircle();
    }
}
