using UnityEngine;
using MoreMountains.Tools;

public class PowerRange : MonoBehaviour {
    public float range;
    [SerializeField] MMLineRendererCircle lineRenderer;

    void Start() {
        UpdateRange();
    }

    public void UpdateRange() {
        lineRenderer.HorizontalRadius = lineRenderer.VerticalRadius = range;
        lineRenderer.DrawCircle();
    }
}