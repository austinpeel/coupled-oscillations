using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private CoupledOscillationsSimulation sim;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private float amplitude = 1f;

    private int currentMode = 1;

    private void Start()
    {
        if (sim) sim.EnterNormalMode(true, amplitude);
    }

    public void HandleModeToggle()
    {
        if (toggleGroup)
        {
            Toggle toggle = toggleGroup.GetFirstActiveToggle();
            if (!toggle.name.Contains(currentMode.ToString()))
            {
                currentMode = currentMode == 1 ? 2 : 1;
                if (sim)
                {
                    sim.EnterNormalMode(currentMode == 1, amplitude);
                }
            }
        }
    }

    public void HandleMassChange(float mass)
    {
        if (sim)
        {
            sim.SetMass(mass);
            sim.EnterNormalMode(currentMode == 1, amplitude);
        }
    }

    public void HandleK1Change(float k1)
    {
        if (sim)
        {
            sim.SetK1(k1);
            sim.EnterNormalMode(currentMode == 1, amplitude);
        }
    }

    public void HandleK2Change(float k2)
    {
        if (sim)
        {
            sim.SetK2(k2);
            sim.EnterNormalMode(currentMode == 1, amplitude);
        }
    }
}
