using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using MoreMountains.Feedbacks;
using UnityEngine.UI;
using System.Collections;

public class GameUIManager : MonoBehaviour {
    // Singleton
    public static GameUIManager i { get; private set; }
    void Awake() {
        if (i == null) { i = this; } else { Destroy(gameObject); }
    }

    [SerializeField] GameObject menu;
    [SerializeField] GameObject gameOver;
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] EntitySpawner entitySpawner;

    void Update() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            menu.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void MainMenu() {
        Time.timeScale = 1;
        GameManager.i.MainMenu();
    }

    public void Cancel() {
        menu.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void GameOver(bool win = true) {
        Time.timeScale = 0;
        if (win) { winText.text = "You Win!"; } else { winText.text = "Sorry, You Lose :("; }
        gameOver.gameObject.SetActive(true);
    }

    public void SpawnPylon() {
        entitySpawner.SpawnDistributorPreview();
    }

    public void SpawnGenerator() {
        entitySpawner.ShowGeneratorPreview();
    }

    // Money UI
    [TabGroup("Money Objects")]
    [SerializeField] TextMeshProUGUI moneyText;
    [TabGroup("Money Objects")]
    [SerializeField] TextMeshProUGUI moneyAddedText;
    [TabGroup("Money Objects")]
    [SerializeField] TextMeshProUGUI moneyRemovedText;
    [TabGroup("Money Objects")]
    [SerializeField] MMFeedbacks _addMoneyFeedback;
    [TabGroup("Money Objects")]
    [SerializeField] MMFeedbacks _removeMoneyFeedback;

    public void AddMoney(int amount) {
        moneyText.text = (int.Parse(moneyText.text) + amount).ToString();
        moneyAddedText.text = "+" + amount.ToString();
        _addMoneyFeedback.PlayFeedbacks();
    }

    public void RemoveMoney(int amount) {
        moneyText.text = (int.Parse(moneyText.text) - amount).ToString();
        moneyRemovedText.text = "-" + amount.ToString();
        _removeMoneyFeedback.PlayFeedbacks();
    }

    [Button(ButtonSizes.Large, ButtonStyle.Box, Expanded = true)]
    public void TestAddMoney() {
        AddMoney(50);
    }

    [Button(ButtonSizes.Large, ButtonStyle.Box, Expanded = true)]
    public void TestRemoveMoney() {
        RemoveMoney(50);
    }


    // Power UI
    [TabGroup("Power Objects")]
    [SerializeField] TextMeshProUGUI powerGenerationText;
    [TabGroup("Power Objects")]
    [SerializeField] TextMeshProUGUI powerConsumptionText;
    [TabGroup("Power Objects")]
    [SerializeField] Slider powerSlider;
    [TabGroup("Power Objects")]
    [SerializeField] Slider consumptionSlider;
    [TabGroup("Power Objects")]
    [SerializeField] MMWiggle powerGenerationWiggle;
    [TabGroup("Power Objects")]
    [SerializeField] MMWiggle powerConsumptionrWiggle;
    [TabGroup("Power Objects")]
    [SerializeField] Image productionFillImage;
    [TabGroup("Power Objects")]
    [SerializeField] Image consumptionFillImage;
    [TabGroup("Power Objects")]
    [SerializeField] Sprite normalImage;
    [TabGroup("Power Objects")]
    [SerializeField] Sprite dangerImage;

    public void SetPower(int production, int consumption) {
        powerGenerationText.text = production.ToString();
        powerConsumptionText.text = consumption.ToString();
        if (production + consumption == 0) {
            StartCoroutine(UpdateSlider(powerSlider, 0));
            StartCoroutine(UpdateSlider(consumptionSlider, 0));
        } else {
            StartCoroutine(UpdateSlider(powerSlider, (float)production / (production + consumption)));
            StartCoroutine(UpdateSlider(consumptionSlider, (float)consumption / (production + consumption)));
        }
        powerGenerationWiggle.enabled = Overproduction(production, consumption);
        powerConsumptionrWiggle.enabled = Overconsumption(production, consumption);
        if (Overproduction(production, consumption)) {
            productionFillImage.sprite = dangerImage;
        } else {
            productionFillImage.sprite = normalImage;
        }

        if (Overconsumption(production, consumption)) {
            consumptionFillImage.sprite = dangerImage;
        } else {
            consumptionFillImage.sprite = normalImage;
        }
    }

    [Button(ButtonSizes.Large, ButtonStyle.Box, Expanded = true)]
    public void TestSetPower(int production, int consumption) {
        SetPower(production, consumption);
    }

    bool Overproduction(int production, int consumption) {
        return production > (consumption * 1.3f);
    }

    bool Overconsumption(int production, int consumption) {
        return consumption > (production);
    }


    // Happiness UI
    [TabGroup("Happiness Objects")]
    [SerializeField] TextMeshProUGUI happinessText;
    [TabGroup("Happiness Objects")]
    [SerializeField] MMWiggle happinessWiggle;
    [TabGroup("Happiness Objects")]
    [SerializeField] Slider happinessSlider;
    [TabGroup("Happiness Objects")]
    [SerializeField] Image happinessFillImage;
    [TabGroup("Happiness Objects")]
    [SerializeField] Sprite normalHappinessImage;
    [TabGroup("Happiness Objects")]
    [SerializeField] Sprite dangerHappinessImage;

    public void SetHappiness(int amount, bool inDanger = false) {
        happinessText.text = amount.ToString() + "%";
        if (inDanger) {
            happinessFillImage.sprite = dangerHappinessImage;
        } else {
            happinessFillImage.sprite = normalHappinessImage;
        }
        happinessWiggle.enabled = inDanger;
        StartCoroutine(UpdateSlider(happinessSlider, (float)amount / 100));
    }

    [Button(ButtonSizes.Large, ButtonStyle.Box, Expanded = true)]
    public void TestSetHappiness(int amount, bool inDanger) {
        SetHappiness(amount, inDanger);
    }


    // If we update the UI every frame, we will have to remove this coroutine
    IEnumerator UpdateSlider(Slider slider, float pct) {
        if (float.IsNaN(pct)) { pct = 0; }
        float start = slider.value;
        float elapsed = 0f;
        while (elapsed < 0.5f) {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(start, pct, elapsed / 0.5f);
            yield return null;
        }
        slider.value = pct;
    }
}
