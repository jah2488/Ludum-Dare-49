using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct WorldBounds {
    public int x;
    public int z;
    public int width;
    public int height;
}

public class LevelManager : MonoBehaviour {
    [TabGroup("Level Settings")]
    public int money = 0;
    [TabGroup("Level Settings")]
    public int happiness = 50;
    [TabGroup("Level Settings")]
    public int score = 0;
    [TabGroup("Level Settings")]
    public int tick = 0;

    [TabGroup("Spawn Settings")]
    [SceneObjectsOnly]
    public GameObject spawnObject;
    [TabGroup("Spawn Settings")]
    [SceneObjectsOnly]
    public GameObject cameraContainer;
    [TabGroup("Spawn Settings")]
    public WorldBounds spawnBounds;
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
    private bool explosionStarted;
    private bool overloadStarted;

    public List<GameObject> buildings;
    public List<GameObject> generators;
    public List<GameObject> pylons;



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

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(spawnBounds.x + spawnBounds.width / 2, 0, spawnBounds.z + spawnBounds.height / 2), new Vector3(spawnBounds.width, 1, spawnBounds.height));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(
            (spawnBounds.x + spawnBounds.width / 2) + spawnBuffer, 0,
            (spawnBounds.z + spawnBounds.height / 2) + spawnBuffer),
            new Vector3(spawnBounds.width - spawnBuffer / 2, 1, spawnBounds.height - spawnBuffer / 2));
    }

    public void OnGeneratorSpawned(GameObject go) {
        var gen = go.GetComponent<PowerGenerator>();
        foreach (var pylon in pylons) {
            var pylonScript = pylon.GetComponent<PowerDistributor>();
            var pylonPosition = pylon.transform.position;
            var generatorPosition = go.transform.position;
            var distance = Vector3.Distance(pylonPosition, generatorPosition);
            if (distance < gen.GetRange()) {
                pylonScript.AddPower(gen.GetOutput());
            }
        }
        generators.Add(go);
        GameUIManager.i.SetPower(getPowerGenerated(), getPowerDraw());
        GameUIManager.i.RemoveMoney(gen.GetCost());
        money -= gen.GetCost();
    }

    public void OnPylonSpawned(GameObject go) {
        var pylon = go.GetComponent<PowerDistributor>();
        foreach (var g in generators) {
            var gen = g.GetComponent<PowerGenerator>();
            var genRange = gen.GetRange();
            var distance = Vector3.Distance(pylon.transform.position, gen.transform.position);
            if (distance < genRange) {
                pylon.AddPower(gen.GetOutput());
            }
        }

        if (buildings.Count > 0) {
            var pylonRange = pylon.GetRange();
            var pylonPosition = go.transform.position;
            foreach (var building in buildings) {
                var buildingPosition = building.transform.position;
                var distance = Vector3.Distance(pylonPosition, buildingPosition);
                if (distance <= pylonRange) {
                    var buildingScript = building.GetComponent<PowerConsumer>();
                    if (pylon.isOn) {
                        buildingScript.AddPower(pylon.GetPowerSupply());
                    }
                }
            }
        }

        go.GetComponent<PowerDistributor>().SetLevelManager(this);
        pylons.Add(go);
        GameUIManager.i.SetPower(getPowerGenerated(), getPowerDraw());
        GameUIManager.i.RemoveMoney(pylon.GetCost());
        money -= pylon.GetCost();
    }

    public void RepairPylon(PowerDistributor pd) {
        GameUIManager.i.RemoveMoney(pd.GetRepairCost());
        money -= pd.GetRepairCost();
    }

    public void RepairGenerator(PowerGenerator pd) {
        GameUIManager.i.RemoveMoney(pd.GetRepairCost());
        money -= pd.GetRepairCost();
    }


   public void UpdateGameUI() {
        GameUIManager.i.SetPower(getPowerGenerated(), getPowerDraw());
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
        Random.seed = Random.Range(0, 100);

        if (spawnObject) {
            spawnObject.transform.position = new Vector3(spawnBounds.x + spawnBounds.width / 2, spawnObject.transform.position.y, spawnBounds.z + spawnBounds.height / 2);
        }
        if (cameraContainer) {
            cameraContainer.transform.position = new Vector3(spawnBounds.x + spawnBounds.width / 2, cameraContainer.transform.position.y, spawnBounds.z + spawnBounds.height / 2);
        }

        GameUIManager.i.SetPower(0, 0);
    }

    void Update() {
        uiManager.UpdateUI(money, happiness, score, tick);
        updatePowerUI();

        if (Time.time >= nextTick) {
            nextTick = Mathf.FloorToInt(Time.time) + 1; 
            tick += 1;
            OnTick(tick, Time.deltaTime);
        }

        CheckForOverProduction();
        CheckForOverConsumption();
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

    void CheckForOverConsumption() {
        if (getPowerDraw() > getPowerGenerated() && !overloadStarted) {
            overloadStarted = true;
            StartCoroutine(StartOverloadTimer(5f));
            Debug.Log("Overconsuming!");
        }
    }

    IEnumerator StartOverloadTimer(float wait) {
        yield return new WaitForSeconds(wait);
        PowerDistributor overloadedPylon = pylons[0].GetComponent<PowerDistributor>();
        foreach(var p in pylons) {
            overloadedPylon = pylons[0].GetComponent<PowerDistributor>();
            var pylon = p.GetComponent<PowerDistributor>();
            if (overloadedPylon.TotalDraw() < pylon.TotalDraw()) {
                overloadedPylon = pylon;
            }
        }
        overloadedPylon.Overload();
        overloadStarted = false;
        yield break;
    }

    IEnumerator StartExplosionTimer(float wait) {
        yield return new WaitForSeconds(wait);
        while(getPowerGenerated() > getPowerDraw() * 1.3f) {
            Debug.Log("Still Over producing");
            if (Random.value > 0.5f) {
                Debug.Log("go boom");
                if (getPowerGenerated() > getPowerDraw() * 1.3f) {
                    var gens = generators.FindAll(x => x.GetComponent<PowerGenerator>().GetOutput() > 0);
                    int randomIndex = Random.Range(0, gens.Count);
                    var go = generators[randomIndex];
                    go.GetComponent<PowerGenerator>().Explode();
                }
            }
            explosionStarted = false;
            yield break;
        }
    }

    void CheckForOverProduction() {
        if (generators.FindAll(x => x.GetComponent<PowerGenerator>().GetOutput() > 0).Count < 2) { return; }
        if (getPowerGenerated() > getPowerDraw() * 1.3f && !explosionStarted) {
            explosionStarted = true;
            StartCoroutine(StartExplosionTimer(4f));
            Debug.Log("Over production");
        }
    }

    void updatePowerUI() {
        uiManager.UpdatePowerUI(getPowerNeed(), getPowerDraw(), getPowerGenerated());
    }

    int getPowerDraw() {
        float powerDraw = 0f;
        foreach (var building in buildings.FindAll(x => x.GetComponent<PowerConsumer>().ConnectedToPower() == true)) {
            powerDraw += building.GetComponent<PowerConsumer>().PowerRequired;
        }
        return Mathf.RoundToInt(powerDraw);
 
    }

    int getPowerNeed() {
        float powerNeed = 0f;
        foreach (var building in buildings) {
            powerNeed += building.GetComponent<PowerConsumer>().PowerRequired;
        }
        return Mathf.RoundToInt(powerNeed);
    }

    int getPowerGenerated() {
        float powerGenerated = 0f;
        foreach (var gen in generators) {
            powerGenerated += gen.GetComponent<PowerGenerator>().GetOutput();
        }
        return Mathf.RoundToInt(powerGenerated);
    }

    void updateHappiness() {
        var WINGAME_LEVEL = 3;
        var LOSEGAME_LEVEL = 4;
        var unPoweredBuildings = buildings.FindAll(x => x.GetComponent<PowerConsumer>().ConnectedToPower() == false).Count;
        var unPoweredBuildingsCount = buildings.Count;
        happiness += (buildings.Count - unPoweredBuildingsCount) * 3;
        happiness -= unPoweredBuildingsCount;
        if (money < 0) {
            happiness -= money * -1 / 100;
            Debug.Log("Happiness-- " + money * -1 / 100);
        }

        if (happiness >= 100) {
            happiness = 100;
        }

        GameUIManager.i.SetHappiness(happiness, happiness < 51);

        if (happiness >= 100) {
            GameUIManager.i.GameOver(true);
        }

        if (happiness < 0) {
            GameUIManager.i.GameOver(false);
        }
    }

    void updateMoney() {
        var newMoney = 0;
        newMoney += buildings.Count;
        newMoney += buildings.FindAll(x => x.GetComponent<PowerConsumer>().ConnectedToPower() == true).Count * 2;
        newMoney -= generators.Count;
        GameUIManager.i.AddMoney(newMoney);
        money += newMoney;
    } 

    void spawnBuilding() {
        int buildingType = UnityEngine.Random.Range(0, 5);

        if (CurveWeighted(spawnFrequencyCurve) > 0.5f) {
            return;
        }

        float x = Mathf.FloorToInt(MapValue(CurveWeighted(spawnLocationCurve), 0, 1, spawnBounds.x - spawnBuffer, spawnBounds.width - spawnBuffer));
        float z = Mathf.FloorToInt(MapValue(CurveWeighted(spawnLocationCurve), 0, 1, spawnBounds.z - spawnBuffer, spawnBounds.height - spawnBuffer));

        validateCoordinates(ref x, ref z);

        var go = Instantiate(buildingPrefab, new Vector3(x, 0, z), Quaternion.identity);
        go.GetComponent<PowerConsumer>().SetLevelManager(this);

        foreach (var pylon in pylons) {
            var pylonScript = pylon.GetComponent<PowerDistributor>();
            if (pylonScript.isOn) {
                var pylonPosition = pylon.transform.position;
                var buildingPosition = go.transform.position;
                var distance = Vector3.Distance(pylonPosition, buildingPosition);
                if (distance < pylonScript.GetRange()) {
                    go.GetComponent<PowerConsumer>().AddPower(pylonScript.GetPowerSupply());
                }
            }
        }

        foreach(var r in go.GetComponentsInChildren<Renderer>()) {
            //r.material.color = new Color[]{Color.blue, Color.red, Color.green, Color.cyan, Color.gray}[buildingType];
        }

        go.transform.rotation = Quaternion.Euler(0, Random.Range(0, 2) * 90, 0);
        buildings.Add(go);
    }

    void validateCoordinates(ref float x, ref float z, int attempts = 0) {
        if (attempts > 100) {
            return;
        }
        var bitMask = 1 << 3;
        var layerMask = ~bitMask;
        if (Physics.CheckSphere(new Vector3(x, 0, z), spawnBuffer, layerMask)) {
            x = MapValue(CurveWeighted(spawnLocationCurve), 0, 1, spawnBounds.x - spawnBuffer, spawnBounds.width - spawnBuffer);
            z = MapValue(CurveWeighted(spawnLocationCurve), 0, 1, spawnBounds.z - spawnBuffer, spawnBounds.height - spawnBuffer);
            validateCoordinates(ref x, ref z, attempts + 1);
        }
    }

    float CurveWeighted(AnimationCurve curve) {
        return curve.Evaluate(Random.value);
    }

    float MapValue(float value, float fromSource, float toSource, float fromTarget, float toTarget) {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}
