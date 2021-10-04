using UnityEngine;
using UnityEngine.EventSystems;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using ModelShark;

// This script can be added on to power generators
// The Output and Range can be set
// This will alow allow you to power on / off the generator

[RequireComponent(typeof(PowerAnimator))]
[RequireComponent(typeof(Hoverable))]
public class PowerGenerator : MonoBehaviour, IPointerUpHandler {
    [SerializeField] float output;
    // public float range;
    [SerializeField] bool isOn = false;
    [SerializeField] MMLineRendererCircle lineRenderer;

    [Header("Debug")]
    [SerializeField] bool toggle;

    [SerializeField] MMFeedbacks _breakFeedback;
    [SerializeField] MMFeedbacks _repairFeedback;
    [SerializeField] PowerAnimator powerAnimator;
    [SerializeField] TooltipTrigger tooltipTrigger;
    [SerializeField] PowerRange powerRange;

    private LevelManager LevelManager;
    private bool hasExploaded = false;
    private int repairAttempts = 1;

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

    public int GetCost() {
        return 100;
    }

    public int GetRepairCost() {
        return (GetCost() / 2) * repairAttempts / 4;
    }

    public void OnPointerUp(PointerEventData _) {
        Switch(!isOn);
    }

    public void Explode() {
        _breakFeedback.PlayFeedbacks();
        hasExploaded = true;
        Switch(false);
    }

    public float GetRange() {
        return powerRange.range;
    }

    // Returns false if the animation is still underway
    public bool Switch(bool hasPower) {
        if (powerAnimator.IsAnimating()) { return false; }
        if (hasExploaded && hasPower) {
            _repairFeedback.PlayFeedbacks();
            hasExploaded = false;
            LevelManager.RepairGenerator(this);
            repairAttempts += 1;
        }
        powerAnimator.Switch(hasPower);
        isOn = hasPower;
        SetTooltip();
        return true;
    }

    public void SetOutput(float newOutput) {
        output = newOutput;
    }

    public float GetOutput() {
        return isOn ? output : 0;
    }

    void SetTooltip() {
        string text = "Output: " + GetOutput() + "\nRange: " + GetRange();
        if (hasExploaded) {
            text = "...>\n<<!-EXPLOSION_DETECTED->>\n" + GetRepairCost() + "Credits\n##REPAIR@NULL;";
        }
        tooltipTrigger.SetText("BodyText", text);
    }

    public void SetLevelManager(LevelManager levelManager) {
        LevelManager = levelManager;
    }
}
