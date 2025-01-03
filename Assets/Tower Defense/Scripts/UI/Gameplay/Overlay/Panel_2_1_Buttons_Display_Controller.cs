using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class Panel_2_1_Buttons_Display_Controller : MonoBehaviour
{
    // Public (Variables) [START]
    [Required]
    public GameObject flagButton;
    [Required]
    public GameObject levelUpButton;
    [Required]
    public GameObject statisticsButton;
    [Required]
    public GameObject trashButton;
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
        bool canDisplayStatisticsButton = false;
        bool canDisplayThrashButton = false;

        if (SelectionManager.instance.IsAnythingSelected)
        {
            PlayableStructure ps = SelectionManager.instance.SelectedAgents.FirstOrDefault().GetComponent<PlayableStructure>();

            if (ps != null)
            {
                canDisplayFlagButtton = true;
                canDisplayLevelUpButton = ps.HasEvolution;
                canDisplayStatisticsButton = true;
                canDisplayThrashButton = true;
            }

            Agent agt = SelectionManager.instance.SelectedAgents.FirstOrDefault().GetComponent<Agent>();

            if (agt != null)
                canDisplayStatisticsButton = true;
        }

        flagButton.SetActive(canDisplayFlagButtton);
        levelUpButton.SetActive(canDisplayLevelUpButton);
        statisticsButton.SetActive(canDisplayStatisticsButton);
        trashButton.SetActive(canDisplayThrashButton);
    }
    // Private (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////