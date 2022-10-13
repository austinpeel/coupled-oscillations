using UnityEngine;

public class CoupledOscillationsSimulation : Simulation
{
    [Header("Simulation Objects")]
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
    // [SerializeField] private double k1 = 10;
    // [SerializeField] private double k2 = 2;
    [SerializeField] private float k1 = 10;
    [SerializeField] private float k2 = 2;

    private double[] x;
    private double[] xdot;
    private double[][] constants;

    private float elapsedTime = 0;

    private void Start()
    {
        double x1 = x1_init - x1_ref;
        double x2 = x2_init - x2_ref;

        // TODO check if masses have been assigned
        constants = new double[2][];
        constants[0] = new double[2] { -Mathf.Sqrt(k1 / mass1.mass), Mathf.Sqrt(k2 / mass1.mass) };
        constants[1] = new double[2] { -Mathf.Sqrt(k1 / mass2.mass), Mathf.Sqrt(k2 / mass2.mass) };

        x = new double[4] { x1, x2, 0, 0 };
        double[] a = ComputeAccelerations();
        xdot = new double[4] { 0, 0, a[0], a[1] };

        SetWallPositions();
        UpdateMassPositions();
        UpdateSpringPositions();
    }

    private void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;

        int numSubsteps = 10;
        double deltaTime = Time.fixedDeltaTime / numSubsteps;
        for (int i = 0; i < numSubsteps; i++)
        {
            TakeLeapfrogStep(deltaTime);
        }

        UpdateMassPositions();
        UpdateSpringPositions();
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    private void UpdateMassPositions()
    {
        if (mass1)
        {
            mass1.transform.position = (float)(x1_ref + x[0]) * Vector3.right;
        }

        if (mass2)
        {
            mass2.transform.position = (float)(x2_ref + x[1]) * Vector3.right;
        }
    }

    private void SetWallPositions()
    {
        if (wall1)
        {
            wall1.position = x1_wall * Vector3.right;
        }

        if (wall2)
        {
            wall2.position = x2_wall * Vector3.right;
        }
    }

    private void UpdateSpringPositions()
    {
        float x1 = (float)(x[0] + x1_ref);
        float x2 = (float)(x[1] + x2_ref);
        if (spring1)
        {
            spring1.SetEndpoints(x1_wall * Vector3.right, x1 * Vector3.right);
        }

        if (spring2)
        {
            spring2.SetEndpoints(x1 * Vector3.right, x2 * Vector3.right);
        }

        if (spring3)
        {
            spring3.SetEndpoints(x2 * Vector3.right, x2_wall * Vector3.right);
        }
    }

    private double[] ComputeAccelerations()
    {
        double[] a = new double[2];
        a[0] = constants[0][0] * x[0] + constants[0][1] * (x[1] - x[0]);
        a[1] = constants[1][0] * x[1] + constants[1][1] * (x[0] - x[1]);
        return a;
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
}
