using UnityEngine;

public class XAxis : MonoBehaviour
{
    [SerializeField] private CoupledOscillationsSimulation sim;
    [SerializeField] private Transform x1;
    [SerializeField] private Transform x2;

    private void Update()
    {
        if (!sim) return;

        UpdateXMarkers();
    }

    private void UpdateXMarkers()
    {
        double[] x = sim.GetX1X2(true);

        if (x1) x1.localPosition = (float)x[0] * Vector3.right;
        if (x2) x2.localPosition = (float)x[1] * Vector3.right;
    }
}
