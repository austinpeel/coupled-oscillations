using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timeDisplay;
    [SerializeField] private PlayButton playButton;

    private float elapsedTime;
    private bool timerIsRunning;

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeDisplay(elapsedTime);
        }
    }

    public void TogglePlayPause()
    {
        if (timerIsRunning)
        {
            PauseTimer();
        }
        else
        {
            StartTimer();
        }
    }

    public void StartTimer()
    {
        timerIsRunning = true;
        playButton?.Play();
    }

    public void PauseTimer()
    {
        timerIsRunning = false;
        playButton?.Pause();
    }

    public void ResetTimer()
    {
        PauseTimer();
        elapsedTime = 0;
        UpdateTimeDisplay(elapsedTime);
    }

    private void UpdateTimeDisplay(float time)
    {
        if (timeDisplay)
        {
            timeDisplay.text = time.ToString("00.00");
        }
    }
}
