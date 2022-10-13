using TMPro;
using UnityEngine;
using System;

public class SpringCoupledMasses : MonoBehaviour
{
    [Header("Simulation Objects")]
    [SerializeField] private MassiveObject mass1;
    [SerializeField] private MassiveObject mass2;
    [SerializeField] private Spring springLeft;
    [SerializeField] private Spring springRight;
    [SerializeField] private Spring springCenter;
    [SerializeField] private Transform wall1;
    [SerializeField] private Transform wall2;

    [Header("Parameters")]
    [SerializeField] private double x1_init = -1;
    [SerializeField] private double x2_init = 1;
    [SerializeField] private double x1_ref = -2;
    [SerializeField] private double x2_ref = 2;
    [SerializeField] private double k1 = 10;
    [SerializeField] private double k2 = 2;

    private enum Solver { Euler, Leapfrog }
    [Header("Solver")]
    [SerializeField] private Solver solver = Solver.Leapfrog;
    [SerializeField] private int numSubsteps = 10;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI energyTMP;

    private double energy0;

    private double[] x;
    private double[] xdot;

    // private float x1;
    // private float x2;
    // private float v1;
    // private float v2;
    // private float a1;
    // private float a2;

    private delegate void SolverStep(double deltaTime);
    private SolverStep solverStep;

    private void Awake()
    {
        Initialize();

        // In case we want to change the appearance of the springs as a function of their constant
        if (springLeft) { springLeft.springConstant = (float)k1; }
        if (springRight) { springRight.springConstant = (float)k1; }
        if (springCenter) { springCenter.springConstant = (float)k2; }
    }

    private void OnDisable()
    {
        UpdateSprings();
    }

    private void Initialize()
    {
        if (mass1 && mass2)
        {
            double x1 = x1_init - x1_ref;
            double x2 = x2_init - x2_ref;
            double v1 = 0;
            double v2 = 0;
            double a1 = -Math.Sqrt(k1 / mass1.mass) * x1 + Math.Sqrt(k2 / mass1.mass) * (x2 - x1);
            double a2 = -Math.Sqrt(k1 / mass2.mass) * x2 + Math.Sqrt(k2 / mass2.mass) * (x1 - x2);

            x = new double[4] { x1, x2, v1, v2 };  // [x1, x2, v1, v2]
            xdot = new double[4] { v1, v2, a1, a2 };  // [v1, v2, a1, a2]

            energy0 = ComputeEnergy();

            UpdateSprings();
        }
    }

    private void FixedUpdate()
    {
        if (!(mass1 && mass2)) { return; }

        if (solver == Solver.Euler)
        {
            solverStep = TakeEulerStep;
        }
        else
        {
            solverStep = TakeLeapfrogStep;
        }

        double deltaTime = Time.fixedDeltaTime / numSubsteps;
        for (int i = 0; i < numSubsteps; i++)
        {
            solverStep(deltaTime);
        }

        if (energyTMP)
        {
            double energyRatio = energy0 / ComputeEnergy();
            energyTMP.text = "E = " + energyRatio.ToString("0.00");
        }

        UpdateSprings();
    }

    private void TakeEulerStep(double deltaTime)
    {
        // // Update positions with current velocities
        // x1 += v1 * deltaTime;
        // x2 += v2 * deltaTime;

        // // Compute new accelerations
        // a1 = -Mathf.Sqrt(k1 / mass1.mass) * x1 + Mathf.Sqrt(k2 / mass1.mass) * (x2 - x1);
        // a2 = -Mathf.Sqrt(k1 / mass2.mass) * x2 + Mathf.Sqrt(k2 / mass2.mass) * (x1 - x2);
        // v1 += a1 * deltaTime;
        // v2 += a2 * deltaTime;

        // Update positions with current velocities
        x[0] += xdot[0] * deltaTime;
        x[1] += xdot[1] * deltaTime;
        x[2] += xdot[2] * deltaTime;
        x[3] += xdot[3] * deltaTime;

        // Compute new accelerations
        xdot[0] = x[2];
        xdot[1] = x[3];
        xdot[2] = -Math.Sqrt(k1 / mass1.mass) * x[0] + Math.Sqrt(k2 / mass1.mass) * (x[1] - x[0]);
        xdot[3] = -Math.Sqrt(k1 / mass2.mass) * x[1] + Math.Sqrt(k2 / mass2.mass) * (x[0] - x[1]);

        // Move masses to new positions
        mass1.transform.position = (float)(x1_ref + x[0]) * Vector3.right;
        mass2.transform.position = (float)(x2_ref + x[1]) * Vector3.right;
    }

    private void TakeLeapfrogStep(double deltaTime)
    {
        // Update positions with current velocities and accelerations
        x[0] += deltaTime * (xdot[0] + 0.5 * xdot[2] * deltaTime);
        x[1] += deltaTime * (xdot[1] + 0.5 * xdot[3] * deltaTime);

        // Compute new accelerations and update velocities
        double a1new = -Math.Sqrt(k1 / mass1.mass) * x[0] + Math.Sqrt(k2 / mass1.mass) * (x[1] - x[0]);
        double a2new = -Math.Sqrt(k1 / mass2.mass) * x[1] + Math.Sqrt(k2 / mass2.mass) * (x[0] - x[1]);
        x[2] += 0.5 * (xdot[2] + a1new) * deltaTime;
        x[3] += 0.5 * (xdot[3] + a2new) * deltaTime;

        // Update accelerations
        xdot[0] = x[2];
        xdot[1] = x[3];
        xdot[2] = a1new;
        xdot[3] = a2new;

        // Move masses to new positions
        mass1.transform.position = (float)(x1_ref + x[0]) * Vector3.right;
        mass2.transform.position = (float)(x2_ref + x[1]) * Vector3.right;
    }

    private double ComputeEnergy()
    {
        double KE = mass1.mass * x[2] * x[2] + mass2.mass * x[3] * x[3];
        double PE = k1 * (x[0] * x[0] + x[1] * x[1]) + k2 * (x[1] - x[0]) * (x[1] - x[0]);
        return 0.5 * (KE + PE);
    }

    private void UpdateSprings()
    {
        if (springLeft && wall1)
        {
            springLeft.SetEndpoints(wall1.position, mass1.transform.position);
        }

        if (springCenter)
        {
            springCenter.SetEndpoints(mass1.transform.position, mass2.transform.position);
        }

        if (springRight && wall2)
        {
            springRight.SetEndpoints(mass2.transform.position, wall2.position);
        }
    }
}
