using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparent : MonoBehaviour {

    [SerializeField] float value;

    void Start() {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers) {
            Material material = renderer.material;
            Color color = new Color(material.color.r, material.color.g, material.color.b, value);
            renderer.material.color = color;
        };
    }
}
