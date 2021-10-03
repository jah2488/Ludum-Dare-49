using UnityEngine;
using UnityEngine.EventSystems;
using ModelShark;

// This is a script that can be attached to pylons
// Clicking on the pylon toggles it on / off

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PowerAnimator))]
public class PowerDistributor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public bool isOn = false;

    [SerializeField] PowerAnimator powerAnimator;

    [SerializeField] TooltipTrigger _tooltipTrigger;

    [SerializeField] PowerRange powerRange;

    [SerializeField] float powerSupply = 20f;
    //Not Used... Yet
    [SerializeField] float powerCapcity = 20f;

    private LevelManager LevelManager;

    void Start() {
        SetTooltip();
    }

    public void OnPointerDown(PointerEventData _) { }

    public void OnPointerUp(PointerEventData _) {
        Switch(!isOn);
    }

    public float GetRange() {
        return powerRange.range;
    }

    private bool Switch(bool hasPower) {
        if (powerAnimator.IsAnimating()) { return false; }
        powerAnimator.Switch(hasPower);
        isOn = hasPower;
        SetTooltip();
        CheckForBuildingsInRange();
        return true;
    }

    void SetTooltip() {
        string text = "Status: " + (isOn ? "On" : "Off");
        _tooltipTrigger.SetText("BodyText", text);
    }

    public float GetPowerSupply() {
        return powerSupply;
    }

    public void SetLevelManager(LevelManager levelManager) {
        LevelManager = levelManager;
    }

    public void AddPower(float power) {
        powerSupply += power;
        if (powerSupply > powerCapcity) {
            powerSupply = powerCapcity;
        }
        CheckIsPowered();
    }

    public void RemovePower(float power) {
        powerSupply -= power;
        if (powerSupply <= 0) {
            powerSupply = 0;
        }
        CheckIsPowered();
    }

    void CheckIsPowered() {
        if (powerSupply > 0) {
            Switch(true);
        } else {
            Switch(false);
        }
    }

    void CheckForBuildingsInRange() {
        if (LevelManager == null) { return; }

        foreach (var building in LevelManager.buildings) {
            var buildingPosition = building.transform.position;
            var distance = Vector3.Distance(transform.position, buildingPosition);
            if (distance <= GetRange()) {
                var buildingScript = building.GetComponent<PowerConsumer>();
                if (isOn) {
                    buildingScript.AddPower(powerSupply);
                } else {
                    buildingScript.RemovePower(powerSupply);
                }
            }
        }
    }
}