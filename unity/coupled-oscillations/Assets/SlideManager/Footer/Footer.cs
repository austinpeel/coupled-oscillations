using UnityEngine;
using System.Runtime.InteropServices;

public class Footer : MonoBehaviour
{
    [SerializeField] private AppState appState;
    [SerializeField] private GameObject enterButton;
    [SerializeField] private GameObject exitButton;

    private bool appIsFullscreen;

    [DllImport("__Internal")]
    private static extern void EnterFullscreen();

    [DllImport("__Internal")]
    private static extern void ExitFullscreen();

    private void Start()
    {
        if (appState) appIsFullscreen = appState.appIsFullscreen;
        SetButtonVisibility();
    }

    public void HandleEnterFullscreen()
    {
        appIsFullscreen = true;
        if (appState) appState.appIsFullscreen = appIsFullscreen;
        SetButtonVisibility();

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        EnterFullscreen();
#endif
    }

    public void HandleExitFullscreen()
    {
        appIsFullscreen = false;
        if (appState) appState.appIsFullscreen = appIsFullscreen;
        SetButtonVisibility();

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        ExitFullscreen();
#endif
    }

    public void SetButtonVisibility()
    {
        if (appIsFullscreen)
        {
            enterButton.SetActive(false);
            exitButton.SetActive(true);
        }
        else
        {
            enterButton.SetActive(true);
            exitButton.SetActive(false);
        }
    }
}
