using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

[RequireComponent(typeof(GameController))]
[HideMonoScript]
public class GameManager : Singleton<GameManager>
{
    // Private (Variables) [START]
    private bool isRunning;
    private bool isPaused;
    // Private (Variables) [END]

    // Public (Properties) [START]
    public bool IsRunning { get { return isRunning; } }
    public bool IsPaused { get { return isPaused; } }
    public bool IsRunningAndNotPaused { get { return isRunning && !isPaused; } }
    // Public (Properties) [END]

    // (Unity) Methods [START]
    private void Start()
    {
        ResetVariables();
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void ResetVariables()
    {
        isRunning = false;
        isPaused = false;
    }
    // Private (Methods) [END]

    // Public (Methods) [START]
    public void StartGame()
    {
        WaveController.instance.StartWaves();

        isRunning = true;
    }
    public void EndGame() => isRunning = false;
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////