using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float elapsedTime;
    private bool timerIsRunning;

    private void Update()
    {
        if (timerIsRunning)
        {
            elapsedTime += Time.deltaTime;
        }
    }
}
