using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TheraBytes.BetterUi;

[HideMonoScript]
public class Panel_2_2_Statistics_Controller : MonoBehaviour
{
    // Public (Variables) [START]
    [Required]
    public BetterSlider experienceSlider;
    [Required]
    public BetterText experienceText;
    [Required]
    public BetterSlider healthSlider;
    [Required]
    public BetterText healthText;
    [Required]
    public BetterText damageText;
    [Required]
    public BetterText attackVelocityText;
    [Required]
    public BetterText attackRangeText;
    [Required]
    public BetterText velocityText;
    [Required]
    public BetterText visibilityText;
    [Required]
    public BetterText evasionText;
    // Public (Variables) [END]


    // (Unity) Methods [START]
    private void Update()
    {
        HandleStatisticsUpdate();
    }
    // (Unity) Methods [END]


    // Private (Methods) [START]
    private void HandleStatisticsUpdate()
    {
        Agent agt = SelectionManager.instance.SelectedAgents.FirstOrDefault()?.GetComponent<Agent>();

        if (agt != null)
        {
            experienceSlider.minValue = 0f;
            experienceSlider.value = agt.ActualExperience;
            experienceSlider.maxValue = agt.ExperienceToEvolve;
            experienceText.text = string.Format("{0:0.##}", agt.ActualExperience) + " / " + string.Format("{0:0.##}", agt.ExperienceToEvolve);

            healthSlider.minValue = 0f;
            healthSlider.value = agt.ActualHealth;
            healthSlider.maxValue = agt.MaxHealth;
            healthText.text = agt.ActualHealth.ToString("0.#") + " / " + agt.MaxHealth.ToString("0.#");

            damageText.text = agt.Damage.ToString("0.#");

            attackVelocityText.text = agt.AttackVelocity.ToString("0.#");

            attackRangeText.text = agt.AttackRange.ToString("0.#");

            velocityText.text = agt.Velocity.ToString("0.#");

            visibilityText.text = agt.VisibilityArea.ToString("0.#");

            evasionText.text = agt.Evasion.ToString();
        }
    }
    // Private (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////