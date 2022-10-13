using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    public enum Type { Simple, Realistic }
    [SerializeField] private Type type = Type.Simple;
    [SerializeField] private Color color = Color.blue;
    public float springConstant = 1f;  // [N / m]

    private void OnValidate()
    {
        lineRenderer?.GetComponent<LineRenderer>();

        if (lineRenderer == null) { return; }

        float width = 0.05f;
        if (type == Type.Realistic)
        {
            width = 0.2f;
        }
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void SetEndpoints(Vector3 point1, Vector3 point2)
    {
        if (lineRenderer == null) { return; }

        lineRenderer.SetPositions(new Vector3[] { point1, point2 });
    }
}
