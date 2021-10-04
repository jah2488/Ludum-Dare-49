using UnityEngine;
using UnityEngine.EventSystems;
using ModelShark;
using MoreMountains.Feedbacks;
using HighlightPlus;

// This is a script that can be attached to pylons
// Clicking on the pylon toggles it on / off

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PowerAnimator))]
public class PowerDistributor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
    public bool isOn = false;

    [SerializeField] PowerAnimator powerAnimator;

    [SerializeField] TooltipTrigger _tooltipTrigger;

    [SerializeField] PowerRange powerRange;

    [SerializeField] MMFeedbacks _breakFeedback;
    [SerializeField] MMFeedbacks _repairFeedback;

    [SerializeField] float powerSupply = 20f;
    //Not Used... Yet
    [SerializeField] float powerCapcity = 20f;

    private LevelManager LevelManager;
    private bool hovering = false;
    private bool isOverloaded = false;
    private int repairAttempts = 1;

    public int GetCost() {
        return 50;
    }

    public int GetRepairCost() {
        return (GetCost() / 2) * repairAttempts / 4;
    }

    void Start() {
        SetTooltip();
    }

    void Update() {
        TotalDraw(hovering);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        hovering = false;
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
        if (isOverloaded && hasPower) {
            _repairFeedback.PlayFeedbacks();
            isOverloaded = false;
            LevelManager.RepairPylon(this);
            repairAttempts += 1;
        }
        powerAnimator.Switch(hasPower);
        isOn = hasPower;
        SetTooltip();
        CheckForBuildingsInRange();
        return true;
    }

    void SetTooltip() {
        string text = "Status: " + (isOn ? "On" : "Off");
        if (LevelManager) {
            text += "\nPowering " + TotalDraw() + " units";
        }
        if (isOverloaded) {
            text = "...>\n<<!-Overloaded->>\n" + GetRepairCost() + "Credits\n##REPAIR@NULL;";
        }
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
        LevelManager.UpdateGameUI();
    }

    public int TotalDraw(bool highlight = false) {
        if (LevelManager == null) { 
            Debug.LogError("LevelManager is not assigned!");
            return -1; 
        }
        var total = 0;

        if (!isOn) { return total; }
        
        foreach (var building in LevelManager.buildings) {
            var buildingPosition = building.transform.position;
            var distance = Vector3.Distance(transform.position, buildingPosition);
            if (distance <= GetRange()) {
                total += 1;
                if (highlight) {
                    building.GetComponentInChildren<HighlightEffect>().highlighted = true;
                    building.GetComponentInChildren<HighlightEffect>().targetFX = true;
                } else {
                    building.GetComponentInChildren<HighlightEffect>().highlighted = false;
                    building.GetComponentInChildren<HighlightEffect>().targetFX = false;
                }
            }
        }
        return total;
    }

    public void Overload() {
        //Replace with Damaged Pylon?
        _breakFeedback.PlayFeedbacks();
        isOverloaded = true;
        Switch(false);
    }
}