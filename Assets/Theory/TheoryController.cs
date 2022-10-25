using UnityEngine;

public class TheoryController : MonoBehaviour
{
    [SerializeField] private CoupledOscillationsSimulation sim;
    [SerializeField] private Transform x1;
    [SerializeField] private Transform x2;

    [Header("References")]
    [SerializeField] private LengthScale lsLeftRef;
    [SerializeField] private LengthScale lsCenterRef;
    [SerializeField] private LengthScale lsRightRef;

    [Header("Length Scales")]
    [SerializeField] private LengthScale lsLeft;
    [SerializeField] private LengthScale lsCenter;
    [SerializeField] private LengthScale lsRight;

    private void Start()
    {
        if (sim)
        {
            SetReferenceLengthScales();
            sim.Pause();
        }
    }

    private void Update()
    {
        if (!sim) return;

        UpdateXMarkers();
        UpdateLengthScales();
    }

    private void UpdateXMarkers()
    {
        double[] x = sim.GetX1X2(true);

        if (x1) x1.localPosition = (float)x[0] * Vector3.right;
        if (x2) x2.localPosition = (float)x[1] * Vector3.right;
    }

    private void SetReferenceLengthScales()
    {
        if (lsLeftRef) lsLeftRef.SetXPositions(sim.GetSpring1Endpoints());
        if (lsCenterRef) lsCenterRef.SetXPositions(sim.GetSpring2Endpoints());
        if (lsRightRef) lsRightRef.SetXPositions(sim.GetSpring3Endpoints());
    }

    private void UpdateLengthScales()
    {
        if (lsLeft) lsLeft.SetXPositions(sim.GetSpring1Endpoints());
        if (lsCenter) lsCenter.SetXPositions(sim.GetSpring2Endpoints());
        if (lsRight) lsRight.SetXPositions(sim.GetSpring3Endpoints());
    }
}
