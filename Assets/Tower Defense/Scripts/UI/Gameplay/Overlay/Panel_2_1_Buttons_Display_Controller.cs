using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Panel_2_1_Buttons_Display_Controller : MonoBehaviour
{
    // Public (Variables) [START]
    public GameObject flagButton;
    public GameObject levelUpButton;
    // Public (Variables) [END]

    // (Unity) Methods [START]
    private void Update()
    {
        HandleButtonsDisplay();
    }
    // (Unity) Methods [END]


    // Private (Methods) [START]
    private void HandleButtonsDisplay()
    {
        bool canDisplayFlagButtton = false;
        bool canDisplayLevelUpButton = false;

        if (SelectionManager.instance.IsAnythingSelected)
        {
            PlayableStructure ps = SelectionManager.instance.SelectedAgents.FirstOrDefault().GetComponent<PlayableStructure>();

            if (ps != null)
            {
                canDisplayFlagButtton = true;
                canDisplayLevelUpButton = true;
            }
        }

        flagButton.SetActive(canDisplayFlagButtton);
        levelUpButton.SetActive(canDisplayLevelUpButton);
    }
    // Private (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////