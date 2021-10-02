using UnityEngine;
using MoreMountains.Tools;

public class PowerGenerator : MonoBehaviour {
    [SerializeField] float output;
    [SerializeField] float range;
    [SerializeField] bool isOn;
    [SerializeField] bool toggle;
    [SerializeField] MMLineRendererCircle lineRenderer;

    PowerAnimator powerAnimator;

    void Start() {
        powerAnimator = GetComponent<PowerAnimator>();
        UpdateRange(range);
    }

    void Update() {
        if (toggle) {
            isOn = !isOn;
            powerAnimator.Switch(isOn);
            toggle = false;
        }
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
