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
    [FoldoutGroup("Quantitative UI Information")]
    [Required]
    public BetterSlider experienceSlider;
    [FoldoutGroup("Quantitative UI Information")]
    [Required]
    public BetterText experienceText;
    [FoldoutGroup("Quantitative UI Information")]
    [Required]
    public BetterSlider healthSlider;
    [FoldoutGroup("Quantitative UI Information")]
    [Required]
    public BetterText healthText;
    [FoldoutGroup("Quantitative UI Information")]
    [Required]
    public BetterText damageText;
    [FoldoutGroup("Quantitative UI Information")]
    [Required]
    public BetterText attackVelocityText;
    [FoldoutGroup("Quantitative UI Information")]
    [Required]
    public BetterText attackRangeText;
    [FoldoutGroup("Quantitative UI Information")]
    [Required]
    public BetterText velocityText;
    [FoldoutGroup("Quantitative UI Information")]
    [Required]
    public BetterText visibilityText;
    [FoldoutGroup("Quantitative UI Information")]
    [Required]
    public BetterText evasionText;
    [FoldoutGroup("Quantitative UI Information")]
    [Required]
    public BetterText spawnQuantityText;
    [FoldoutGroup("UI Items")]
    [Required]
    public GameObject experienceItem;
    [FoldoutGroup("UI Items")]
    [Required]
    public GameObject healthItem;
    [FoldoutGroup("UI Items")]
    [Required]
    public GameObject damageItem;
    [FoldoutGroup("UI Items")]
    [Required]
    public GameObject attackVelocityItem;
    [FoldoutGroup("UI Items")]
    [Required]
    public GameObject attackRangeItem;
    [FoldoutGroup("UI Items")]
    [Required]
    public GameObject velocityItem;
    [FoldoutGroup("UI Items")]
    [Required]
    public GameObject visibilityItem;
    [FoldoutGroup("UI Items")]
    [Required]
    public GameObject evasionItem;
    [FoldoutGroup("UI Items")]
    [Required]
    public GameObject spawnQuantityItem;
    // Public (Variables) [END]


    // (Unity) Methods [START]
    private void Update()
    {
        HandleStatisticsUpdate();
        HandleStatisticsVisibility();
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

            spawnQuantityText.text = agt.GetAgent().subspawns.Aggregate(0, (int acc, SubSpawnSO value) => acc += value.maxAlive, result => result.ToString());
        }
    }
    private void HandleStatisticsVisibility()
    {
        Agent agt = SelectionManager.instance.SelectedAgents.FirstOrDefault()?.GetComponent<Agent>();

        if (agt != null)
        {
            AgentFsmAi afai = agt.gameObject.GetComponent<AgentFsmAi>();

            switch (agt.GetAgent().type)
            {
                case AgentTypeEnum.CREATURE:
                    experienceItem.SetActive(true);
                    healthItem.SetActive(true);

                    damageItem.SetActive(afai.IsAggressive);
                    attackVelocityItem.SetActive(afai.IsAggressive);
                    attackRangeItem.SetActive(afai.IsAggressive);
                    velocityItem.SetActive(true);
                    visibilityItem.SetActive(true);
                    evasionItem.SetActive(true);
                    spawnQuantityItem.SetActive(agt.SubSpawns.Count > 0);
                    break;
                case AgentTypeEnum.STRUCTURE:
                    experienceItem.SetActive(true);
                    healthItem.SetActive(true);

                    damageItem.SetActive(afai.IsAggressive);
                    attackVelocityItem.SetActive(afai.IsAggressive);
                    attackRangeItem.SetActive(afai.IsAggressive);
                    velocityItem.SetActive(false);
                    visibilityItem.SetActive(true);
                    evasionItem.SetActive(false);
                    spawnQuantityItem.SetActive(agt.SubSpawns.Count > 0);
                    break;
            }
        }
    }
    // Private (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////