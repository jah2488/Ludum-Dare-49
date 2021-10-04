using UnityEngine;
using System.Collections;
using MoreMountains.Feedbacks;
using ModelShark;

// This script can be placed on house game objects
// It will consume power set in `powerRequired`
// SetPower can be called (somehow) to know if there is enough power

public class PowerConsumer : MonoBehaviour {
    [SerializeField] float powerRequired;
    [SerializeField] float powerSupplied;
    [SerializeField] bool hasPower;

    [SerializeField] Material onMaterial;
    [SerializeField] Material offMaterial;

    [SerializeField] MeshRenderer _light;

    [SerializeField] MMFeedbacks _flickerFeedback;

    [Header("Debug")]
    [SerializeField] bool toggle;

    [SerializeField] TooltipTrigger _tooltipTrigger;

    private LevelManager LevelManager;
    private int poweredCycles = 0;

    void Start() {
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

    public void SetLevelManager(LevelManager levelManager) {
        LevelManager = levelManager;
    }

    public bool ConnectedToPower() {
        return hasPower;
    }

    public float PowerRequired { get { return powerRequired; } }

    public void SetPowerRequired(float power) {
        powerRequired = power;
    }

    public void AddPower(float power) {
        powerSupplied += power;
        CheckIsPowered();
    }

    public void RemovePower(float power) {
        powerSupplied -= power;
        CheckIsPowered();
    }

    public bool HasPower { get { return hasPower; } }

    private void SetPower(bool hasPower) {
        if (hasPower) {
            StartCoroutine(TurnOnCoroutine());
        } else {
            StartCoroutine(TurnOffCoroutine());
        }
        this.hasPower = hasPower;
        poweredCycles++;
        LevelManager.UpdateGameUI();
    }

    void CheckIsPowered() {
        if (powerSupplied >= powerRequired) {
            SetPower(true);
        } else {
            SetPower(false);
        }
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
        text += "\nSupplied: " + powerSupplied;
        text += "\nStability: " + (hasPower ? "+2" : "-1");
        text += "\n------";
        if (hasPower && poweredCycles < 5) {
            text += "'Finally power!'";
        } else {
            text += "\n'HELP! We need some power over here!'";
            text += "\n'I hope there is a pylon and a generator on the way here!'";
        }

        if (!hasPower && poweredCycles > 2) {
            text += "\n'I'm not sure if we have enough power here...'";
            text += "\n'Maybe we should look for a pylon that isn't overloaded?'";
            text += "\n'Maybe we should look for a generator that isn't on fire!?'";
        }

        if (poweredCycles > 5) {
            text += "\nWhat are they doing?";
            text += "\nI don't think they know how to do this at all!";
        }

        if (LevelManager && LevelManager.money < 0) {
            text += "\n'I really don't want us going in to debt over this project!'";
        }
        _tooltipTrigger.SetText("BodyText", text);
    }
}
