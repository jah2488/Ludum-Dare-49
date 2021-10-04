using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIManager : MonoBehaviour {
    // Singleton
    public static MainUIManager i { get; private set; }
    void Awake() {
        if (i == null) { i = this; } else { Destroy(gameObject); }
    }

    public void StartNewGamme() {
        GameManager.i.StartNewGame();
    }

    public void Quit() {
        GameManager.i.Quit();
    }
}
