using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text powerNeed;
    public Text powerDraw;
    public Text powerGeneration;
    public Text money;
    public Text happiness;
    public Text time;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI(int m, int hap, int sc, int tick) {
        money.text = "Money: " + m;
        happiness.text = "Happiness: " + hap;
        time.text = "Time: " + tick;
    }

    public void UpdatePowerUI(int pn, int pd, int pg) {
        powerNeed.text = "Power Need: " + pn.ToString();
        powerDraw.text = "Power Draw: " + pd.ToString();
        powerGeneration.text = "Power Gen: " + pg.ToString();
    }
}
