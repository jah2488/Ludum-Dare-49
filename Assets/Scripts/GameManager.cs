using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using System.Collections;


public class GameManager : MonoBehaviour {
    public static GameManager i { get; private set; }

    [SerializeField] MMFader fader;
    public int currentLevel = 0;

    void Awake() {
        if (i == null) { i = this; } else { Destroy(gameObject); }
    }

    void Start() {
        fader.enabled = true;
    }

    public void StartNewGame() {
        currentLevel = 1;
        LoadLevel(currentLevel);
    }

    public void LoadLevel(int i) {
        currentLevel = i;
        FadeIn();
        StartCoroutine(LoadScene(1));
    }

    IEnumerator LoadScene(int id, float wait = 1f) {
        yield return new WaitForSeconds(wait);
        SceneManager.LoadScene(id);
        FadeOut();
    }

    public void FadeIn() {
        MMFadeInEvent.Trigger(0.5f, new MMTweenType(MMTween.MMTweenCurve.LinearTween));
    }

    public void FadeOut() {
        MMFadeOutEvent.Trigger(0.5f, new MMTweenType(MMTween.MMTweenCurve.LinearTween));
    }
}
