using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OverlayInterfaceManager))]
public class OverlayInterfaceController : MonoBehaviour
{
    // (Unity) Methods [START]
    private void Update()
    {
        HandlePanel_2_1_Visibility();
        HandlePanel_2_2_Visibility();
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void HandlePanel_2_1_Visibility()
    {
        HandlePanel_Any_Panel_Visibility(OverlayInterfaceManager.instance.panel_2_1);
    }
    private void HandlePanel_2_2_Visibility()
    {
        if (PlayerCommandsManager.instance.IsTryingToCast)
        {
            OverlayInterfaceManager.instance.panel_2_2.SetActive(false);

            return;
        }

        if (!SelectionManager.instance.IsAnythingSelected)
            OverlayInterfaceManager.instance.panel_2_2.SetActive(false);
    }
    private void HandlePanel_Any_Panel_Visibility(GameObject pPanel)
    {
        if (PlayerCommandsManager.instance.IsTryingToCast)
        {
            pPanel.SetActive(false);

            return;
        }

        if (SelectionManager.instance.IsAnythingSelected)
            pPanel.SetActive(true);
        else
            pPanel.SetActive(false);
    }
    // Private (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////