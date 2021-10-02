using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LevelManager : MonoBehaviour
{
    [TabGroup("Level Settings")]
    public int money = 0;
    [TabGroup("Level Settings")]
    public int happiness = 50;
    [TabGroup("Level Settings")]
    public int score = 0 ;
    [TabGroup("Level Settings")]
    public int tick = 0;

    [TabGroup("Spawn Settings")]
    public int maxBuildings = 20;
    [TabGroup("Spawn Settings")]
    public float spawnBuffer = 1.5f;
    [TabGroup("Spawn Settings")]
    public int spawnRate = 1;
    [TabGroup("Spawn Settings")]
    public int spawnOffset = 50;
    [TabGroup("Spawn Settings")]
    public int seed = 69;
    [TabGroup("Spawn Settings")]
    public AnimationCurve spawnLocationCurve;
    [TabGroup("Spawn Settings")]
    public AnimationCurve spawnFrequencyCurve;

    [TabGroup("Prefabs")]
    public GameObject buildingPrefab;
    [TabGroup("Prefabs")]
    public GameObject generatorPrefab;
    [TabGroup("Prefabs")]
    public GameObject pylonPrefab;

    private int nextTick = 1;
    private UIManager uiManager;

    private List<GameObject> buildings;
    private List<GameObject> generators;
    private List<GameObject> pylons;

    
    [Button(ButtonSizes.Large, ButtonStyle.Box, Expanded = true)]
    public void SpawnGenerator(int x, int z) {
        var go = Instantiate(generatorPrefab, new Vector3(x, 0, z), Quaternion.identity);
        generators.Add(go);
    }

    [Button(ButtonSizes.Large, ButtonStyle.Box, Expanded = true)]
    public void SpawnPylon(int x, int z) {
        var go = Instantiate(pylonPrefab, new Vector3(x, 0, z), Quaternion.identity);
        pylons.Add(go);
    }


    void Awake() {
        //Get all generators and buildings currently in the scene.
        buildings = new List<GameObject>();
        generators = new List<GameObject>();
        pylons = new List<GameObject>();
    }

    void Start() {
        uiManager = this.GetComponent<UIManager>();
        buildings = new List<GameObject>();
        generators = new List<GameObject>();
        pylons = new List<GameObject>();
        Random.seed = seed;
    }

    void Update() {
       uiManager.UpdateUI(money, happiness, score, tick);
       updatePowerUI();

       if (Time.time >= nextTick) {
           nextTick = Mathf.FloorToInt(Time.time) + 1;
           tick += 1;
           OnTick(tick, Time.deltaTime);
       } 
    }

    void OnTick(int tick, float deltaTime) {
        if (tick % spawnRate == 0 && buildings.Count < maxBuildings) {
            spawnBuilding();
        }
        if (tick % 5 == 0) {
            updateHappiness();
            updateMoney();
        }
    }

    void updatePowerUI() {
        uiManager.UpdatePowerUI(getPowerNeed(), getPowerDraw(), getPowerGenerated());
    }

    int getPowerDraw() {
        return buildings.FindAll(x => x.GetComponent<PowerConsumer>().ConnectedToPower() == true).Count;
    }

    int getPowerNeed() {
        float powerNeed = 0f;
        foreach (var building in buildings) {
            powerNeed += building.GetComponent<PowerConsumer>().PowerRequired;
        }
        return Mathf.FloorToInt(powerNeed);
    }

    int getPowerGenerated() {
        float powerGenerated = 0f;
        foreach (var gen in generators) {
            powerGenerated += gen.GetComponent<PowerGenerator>().GetOutput();
        }
        return Mathf.FloorToInt(powerGenerated);
    }

    void updateHappiness() {
        var unPoweredBuildings = buildings.FindAll(x => x.GetComponent<PowerConsumer>().ConnectedToPower() == false).Count;
        var unPoweredBuildingsCount = buildings.Count;
        happiness += buildings.Count;
        happiness -= unPoweredBuildingsCount;
    }

    void updateMoney() {
        money += buildings.Count;
        money += buildings.FindAll(x => x.GetComponent<PowerConsumer>().ConnectedToPower() == true).Count * 2;
        money -= generators.Count;
    }

    void spawnBuilding() {
        int buildingType = UnityEngine.Random.Range(0, 3);

        if (CurveWeighted(spawnFrequencyCurve) > 0.5f) {
            return;
        }

        float x = CurveWeighted(spawnLocationCurve) * spawnOffset;
        float z = CurveWeighted(spawnLocationCurve) * spawnOffset;

        validateCoordinates(ref x, ref z);

        var go = Instantiate(buildingPrefab, new Vector3(x, 0, z), Quaternion.identity);

        buildings.Add(go);
    }

    void validateCoordinates(ref float x, ref float z, int attempts = 0) {
        if (attempts > 1000) {
            return;
        }
        if (Physics.CheckSphere(new Vector3(x, 0, z), spawnBuffer)) {
            Debug.Log("Collision");
            x = CurveWeighted(spawnLocationCurve) * spawnOffset;
            z = CurveWeighted(spawnLocationCurve) * spawnOffset;
            validateCoordinates(ref x, ref z, attempts + 1);
        }
    }

    float CurveWeighted(AnimationCurve curve) {
        return curve.Evaluate(Random.value);
    }
}
