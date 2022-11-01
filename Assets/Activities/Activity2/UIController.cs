using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private CoupledOscillationsSimulation sim;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private float amplitude = 1f;
    [SerializeField, Min(0)] private int numDecimalDigits = 1;
    [SerializeField] private bool pauseOnSliderChange = false;

    private int currentMode = 1;

    private void Start()
    {
        if (toggleGroup && sim) sim.EnterNormalMode(true, amplitude);
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
            sim.SetMass(RoundToDecimalPlace(mass, numDecimalDigits), pauseOnSliderChange);
            if (toggleGroup) sim.EnterNormalMode(currentMode == 1, amplitude);
        }
    }

    public void HandleK1Change(float k1)
    {
        if (sim)
        {
            sim.SetK1(RoundToDecimalPlace(k1, numDecimalDigits), pauseOnSliderChange);
            if (toggleGroup) sim.EnterNormalMode(currentMode == 1, amplitude);
        }
    }

    public void HandleK2Change(float k2)
    {
        if (sim)
        {
            sim.SetK2(RoundToDecimalPlace(k2, numDecimalDigits), pauseOnSliderChange);
            if (toggleGroup) sim.EnterNormalMode(currentMode == 1, amplitude);
        }
    }

    private float RoundToDecimalPlace(float value, int decimalPlace)
    {
        float factor = Mathf.Pow(10f, decimalPlace);
        return Mathf.Round(factor * value) / factor;
    }
}
