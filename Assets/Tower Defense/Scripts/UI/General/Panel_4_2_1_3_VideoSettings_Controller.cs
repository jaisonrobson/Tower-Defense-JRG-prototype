using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Sirenix.OdinInspector;
using TheraBytes.BetterUi;

[HideMonoScript]
public class Panel_4_2_1_3_VideoSettings_Controller : MonoBehaviour
{
    // Public (Variables) [START]
    [Required]
    [SceneObjectsOnly]
    public BetterDropdown resolutionDropdown;
    [Required]
    [SceneObjectsOnly]
    public BetterToggle fullscreenToggle;
    [Required]
    [SceneObjectsOnly]
    public BetterDropdown graphicsQualityDropdown;
    [Required]
    [SceneObjectsOnly]
    public BetterDropdown antiAliasingDropdown;
    [Required]
    [SceneObjectsOnly]
    public BetterDropdown textureQualityDropdown;
    // Public (Variables) [END]


    // Private (Variables) [START]
    private bool isRevertingSettings;
    private float timeUntilRevert;
    private Resolution[] resolutions;

    private bool beforeIsFullScreen;
    private Resolution beforeResolution;
    private int beforeGraphicsQuality;
    private int beforeTextureQuality;
    private int beforeAntiAliasing;

    private bool actualIsFullScreen;
    private Resolution actualResolution;
    private int actualGraphicsQuality;
    private int actualTextureQuality;
    private int actualAntiAliasing;
    // Private (Variables) [END]


    // Public (Properties) [START]
    public Resolution[] Resolutions { get { return resolutions; } }
    public bool ActualIsFullScreen { set { actualIsFullScreen = value; } }
    public Resolution ActualResolution { set { actualResolution = value; } }
    public int ActualGraphicsQuality { set { actualGraphicsQuality = value; } }
    public int ActualTextureQuality { set { actualTextureQuality = value; } }
    public int ActualAntiAliasing { set { actualAntiAliasing = value; } }
    // Public (Properties) [END]

    // (Unity) Methods [START]
    void Start()
    {
        InitializeVariables();
        InitializeUI();
    }
    private void Update()
    {
        HandleSettingsReversion();
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void InitializeVariables()
    {
        isRevertingSettings = false;
        timeUntilRevert = 0f;

        resolutions = Screen.resolutions;

        actualIsFullScreen = Screen.fullScreen;
        beforeIsFullScreen = Screen.fullScreen;

        actualResolution = Screen.currentResolution;
        beforeResolution = Screen.currentResolution;

        actualGraphicsQuality = QualitySettings.GetQualityLevel();
        beforeGraphicsQuality = QualitySettings.GetQualityLevel();

        actualTextureQuality = QualitySettings.masterTextureLimit;
        beforeTextureQuality = QualitySettings.masterTextureLimit;

        actualAntiAliasing = QualitySettings.antiAliasing;
        beforeAntiAliasing = QualitySettings.antiAliasing;
    }
    private void InitializeUI()
    {
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutions.ToList().ConvertAll<string>(r => r.width + " x " + r.height));

        fullscreenToggle.isOn = actualIsFullScreen;

        graphicsQualityDropdown.ClearOptions();
        graphicsQualityDropdown.AddOptions(QualitySettings.names.ToList());

        List<string> antiAliasingOptions = new List<string>() { "Disabled", "2x MSAA", "4x MSAA", "8x MSAA"};
        antiAliasingDropdown.ClearOptions();
        antiAliasingDropdown.AddOptions(antiAliasingOptions);

        List<string> textureQualityOptions = new List<string>() { "Full Resolution", "Half Resolution", "1/4 Resolution", "1/8 Resolution" };
        textureQualityDropdown.ClearOptions();
        textureQualityDropdown.AddOptions(textureQualityOptions);
    }
    private void HandleSettingsReversion()
    {
        if (isRevertingSettings && Time.time > timeUntilRevert)
        {
            isRevertingSettings = false;
            timeUntilRevert = 0f;

            ResetActualVariables();
            ApplyActualVariablesToUnity();
        }
    }
    private void ResetActualVariables()
    {
        actualIsFullScreen = beforeIsFullScreen;
        actualResolution = beforeResolution;
        actualGraphicsQuality = beforeGraphicsQuality;
        actualTextureQuality = beforeTextureQuality;
        actualAntiAliasing = beforeAntiAliasing;
    }
    private void ResetBeforeVariables()
    {
        beforeIsFullScreen = actualIsFullScreen;
        beforeResolution = actualResolution;
        beforeGraphicsQuality = actualGraphicsQuality;
        beforeTextureQuality = actualTextureQuality;
        beforeAntiAliasing = actualAntiAliasing;
    }
    private void ApplyActualVariablesToUnity()
    {
        Screen.fullScreen = actualIsFullScreen;

        Screen.SetResolution(actualResolution.width, actualResolution.height, actualIsFullScreen, actualResolution.refreshRate);

        QualitySettings.SetQualityLevel(actualGraphicsQuality);

        QualitySettings.masterTextureLimit = actualTextureQuality;

        QualitySettings.antiAliasing = actualAntiAliasing;
    }
    // Private (Methods) [END]

    // Public (Methods) [START]
    public void TryUpdateSettings()
    {
        isRevertingSettings = true;
        timeUntilRevert = Time.time + 10f;

        ApplyActualVariablesToUnity();
    }
    public void ConfirmSettingsUpdate()
    {
        isRevertingSettings = false;
        timeUntilRevert = 0f;

        ResetBeforeVariables();
    }
    /*
    public void UpdateIsFullScreen(bool pIsFullScreen) => actualIsFullScreen = pIsFullScreen;
    public void UpdateResolution(Resolution pResolution) => actualResolution = pResolution;
    public void UpdateGraphicsQuality(int pGraphicsQuality) => actualGraphicsQuality = pGraphicsQuality;
    public void UpdateTextureQuality(int pTextureQuality) => actualTextureQuality = pTextureQuality;
    public void UpdateAntiAliasing(int pAntiAliasing) => actualAntiAliasing = pAntiAliasing;
    */
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////