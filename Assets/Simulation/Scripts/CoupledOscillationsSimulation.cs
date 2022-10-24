using UnityEngine;

public class CoupledOscillationsSimulation : Simulation
{
    [Header("Objects")]
    [SerializeField] private MassiveObject mass1;
    [SerializeField] private MassiveObject mass2;
    [SerializeField] private Spring spring1;
    [SerializeField] private Spring spring2;
    [SerializeField] private Spring spring3;
    [SerializeField] private Transform wall1;
    [SerializeField] private Transform wall2;

    [Header("Parameters")]
    [SerializeField] private double x1_init = -1;
    [SerializeField] private double x2_init = 1;
    [SerializeField] private double x1_ref = -2;
    [SerializeField] private double x2_ref = 2;
    [SerializeField] private float x1_wall = -5;
    [SerializeField] private float x2_wall = 5;

    private double[] x;
    private double[] xdot;
    private double[][] constants;

    private void Awake()
    {
        InitializeCouplingConstants();

        double x1 = x1_init - x1_ref;
        double x2 = x2_init - x2_ref;
        x = new double[4] { x1, x2, 0, 0 };
        double[] a = ComputeAccelerations();
        xdot = new double[4] { 0, 0, a[0], a[1] };

        SetWallPositions();
        UpdateMassPositions();
        UpdateSpringPositions();
    }

    private void FixedUpdate()
    {
        if (paused) { return; }

        int numSubsteps = 10;
        double deltaTime = Time.fixedDeltaTime / numSubsteps;
        for (int i = 0; i < numSubsteps; i++)
        {
            // TakeEulerStep(deltaTime);
            TakeLeapfrogStep(deltaTime);
        }

        UpdateMassPositions();
        UpdateSpringPositions();
    }

    public void UpdateMassPositions()
    {
        if (mass1) mass1.transform.position = (float)(x1_ref + x[0]) * Vector3.right;
        if (mass2) mass2.transform.position = (float)(x2_ref + x[1]) * Vector3.right;
    }

    private void SetWallPositions()
    {
        if (wall1) wall1.position = x1_wall * Vector3.right;
        if (wall2) wall2.position = x2_wall * Vector3.right;
    }

    public void UpdateSpringPositions()
    {
        float x1 = (float)(x[0] + x1_ref);
        float x2 = (float)(x[1] + x2_ref);

        float offsetWall1 = wall1 ? 0.5f * wall1.localScale.x : 0;
        float offsetWall2 = wall2 ? 0.5f * wall2.localScale.x : 0;
        float offsetMass1 = mass1 ? mass1.HalfScale : 0;
        float offsetMass2 = mass2 ? mass2.HalfScale : 0;

        if (spring1)
        {
            spring1.SetEndpoints((x1_wall + offsetWall1) * Vector3.right, (x1 - offsetMass1) * Vector3.right);
        }

        if (spring2)
        {
            spring2.SetEndpoints((x1 + offsetMass1) * Vector3.right, (x2 - offsetMass2) * Vector3.right);
        }

        if (spring3)
        {
            spring3.SetEndpoints((x2 + offsetMass2) * Vector3.right, (x2_wall - offsetWall2) * Vector3.right);
        }
    }

    private double[] ComputeAccelerations()
    {
        double[] a = new double[2];
        a[0] = constants[0][0] * x[0] + constants[0][1] * (x[1] - x[0]);
        a[1] = constants[1][0] * x[1] + constants[1][1] * (x[0] - x[1]);
        return a;
    }

    private void InitializeCouplingConstants()
    {
        constants = new double[2][];
        constants[0] = new double[2] { 1, 0 };
        constants[1] = new double[2] { 0, 1 };
        UpdateCouplingConstants();
    }

    private void UpdateCouplingConstants()
    {
        // TODO generalize for all 3 springs having different constants
        if (spring1 && spring2 && mass1 && mass2)
        {
            constants[0][0] = -spring1.springConstant / mass1.mass;
            constants[0][1] = spring2.springConstant / mass1.mass;
            constants[1][0] = -spring1.springConstant / mass2.mass;
            constants[1][1] = spring2.springConstant / mass2.mass;
        }
    }

    private void TakeLeapfrogStep(double deltaTime)
    {
        // Update positions with current velocities and accelerations
        x[0] += deltaTime * (xdot[0] + 0.5 * xdot[2] * deltaTime);
        x[1] += deltaTime * (xdot[1] + 0.5 * xdot[3] * deltaTime);

        // Compute new accelerations and update velocities
        double[] aNew = ComputeAccelerations();
        x[2] += 0.5 * (xdot[2] + aNew[0]) * deltaTime;
        x[3] += 0.5 * (xdot[3] + aNew[1]) * deltaTime;

        // Update accelerations
        xdot[0] = x[2];
        xdot[1] = x[3];
        xdot[2] = aNew[0];
        xdot[3] = aNew[1];
    }

    public void UpdateX(float x1, float x2)
    {
        x[0] = x1;
        x[1] = x2;
        x[2] = 0;
        x[3] = 0;
    }

    public void UpdateXDot()
    {
        double[] a = ComputeAccelerations();
        xdot[0] = x[2];
        xdot[1] = x[3];
        xdot[2] = a[0];
        xdot[3] = a[1];
    }

    public double[] GetX1X2()
    {
        return new double[2] { x[0], x[1] };
    }

    public void EnterNormalMode(bool first, float amplitude)
    {
        float sign = first ? 1 : -1;
        UpdateX(-amplitude, -sign * amplitude);
        UpdateXDot();
        UpdateMassPositions();
        UpdateSpringPositions();
    }

    public void SetMass(float value)
    {
        if (mass1 && mass2)
        {
            mass1.SetMass(value);
            mass2.SetMass(value);
            UpdateCouplingConstants();
        }
    }

    public void SetK1(float value)
    {
        if (spring1 && spring3)
        {
            spring1.springConstant = value;
            spring3.springConstant = value;
            UpdateCouplingConstants();
        }
    }

    public void SetK2(float value)
    {
        if (spring2)
        {
            spring2.springConstant = value;
            UpdateCouplingConstants();
        }
    }
}
