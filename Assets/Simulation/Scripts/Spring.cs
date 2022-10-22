using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    public enum Type { Simple, ZigZag, Coiled }
    public Type type = Type.Simple;
    [SerializeField] private Color color = Color.blue;
    public float springConstant = 1f;  // [N / m]
    public float radius = 1f;  // m

    [Header("Endpoints")]
    public Vector3 point1 = Vector3.left;
    public Vector3 point2 = Vector3.right;

    // private void OnValidate()
    // {
    //     lineRenderer?.GetComponent<LineRenderer>();

    //     if (lineRenderer == null) { return; }

    //     float width = 0.05f;
    //     if (type == Type.ZigZag)
    //     {
    //         width = 0.2f;
    //     }
    //     lineRenderer.startWidth = width;
    //     lineRenderer.endWidth = width;

    //     lineRenderer.startColor = color;
    //     lineRenderer.endColor = color;
    // }

    public void SetEndpoints(Vector3 point1, Vector3 point2)
    {
        this.point1 = point1;
        this.point2 = point2;
    }

    public void Redraw()
    {
        if (lineRenderer == null) { return; }

        Debug.Log("Redrawing");

        SetEndpoints(point1, point2);

        float width = 0.06f * Mathf.Log10(1 + Mathf.Max(1, springConstant));
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        switch (type)
        {
            case Type.Simple:
                lineRenderer.positionCount = 2;
                lineRenderer.numCornerVertices = 0;
                lineRenderer.SetPositions(new Vector3[] { point1, point2 });
                break;
            case Type.ZigZag:
                lineRenderer.positionCount = 8;
                lineRenderer.numCornerVertices = 5;
                Vector3 displacement = point2 - point1;
                Vector3 xHat = displacement.normalized;
                Vector3 yHat = Vector3.Cross(Vector3.Cross(xHat, Vector3.up), xHat);
                float delta = displacement.magnitude / 6;
                Vector3[] positions = new Vector3[8];
                positions[0] = point1;
                positions[1] = point1 + 1 * delta * xHat;
                positions[2] = point1 + 1.5f * delta * xHat + radius * yHat;
                positions[3] = point1 + 2.5f * delta * xHat - radius * yHat;
                positions[4] = point1 + 3.5f * delta * xHat + radius * yHat;
                positions[5] = point1 + 4.5f * delta * xHat - radius * yHat;
                positions[6] = point1 + 5 * delta * xHat;
                positions[7] = point2;
                lineRenderer.SetPositions(positions);
                break;
            default:
                break;
        }
    }
}
