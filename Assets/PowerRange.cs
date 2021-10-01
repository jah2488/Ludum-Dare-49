using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PowerRange : MonoBehaviour
{
    public float deliveryRange = 10f;
    public float pullRange = 20f;
    public int segments = 10;

    private float power;
    private LineRenderer line;
    private LineRenderer pullLine;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();

        line.SetVertexCount(segments + 1);
        line.useWorldSpace = false;
        CreatePoints(deliveryRange, line);
    }

    void CreatePoints(float range, LineRenderer line)
    {
        float x;
        float y;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin (Mathf.Deg2Rad * angle) * deliveryRange;

            y = Mathf.Cos (Mathf.Deg2Rad * angle) * deliveryRange;

            line.SetPosition (i,new Vector3(y,0,x) );

            angle += (360f / segments);
        }
    }

    [Button]
    public void UpdateCircle() {
        CreatePoints(deliveryRange, line);
    }
}
