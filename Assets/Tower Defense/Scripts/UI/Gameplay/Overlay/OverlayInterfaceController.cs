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
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void HandlePanel_2_1_Visibility()
    {
        if (PlayerCommandsManager.instance.IsTryingToCast)
        {
            OverlayInterfaceManager.instance.panel_2_1.SetActive(false);

            return;
        }

        if (SelectionManager.instance.IsAnythingSelected)
            OverlayInterfaceManager.instance.panel_2_1.SetActive(true);
        else
            OverlayInterfaceManager.instance.panel_2_1.SetActive(false);
    }
    // Private (Methods) [END]
}
