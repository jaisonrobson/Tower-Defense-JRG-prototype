using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Core.Patterns;

[HideMonoScript]
public class AgentUI : MonoBehaviour
{
    // Public (Variables) [START]
    public SizeEnum healthSliderSize;
    public SizeEnum textFloatingValueSize;
    // Public (Variables) [END]

    // Private (Variables) [START]
    private SliderController spawnedHealthSlider;
    private Agent agent;
    // Private (Variables) [END]    

    // (Unity) Methods [START]
    private void Start()
    {
        InitializeVariables();        
    }
    private void Update()
    {
        CreateHealthSlider();

        UpdateHealthSlider();
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void InitializeVariables()
    {
        agent = GetComponent<Agent>();
    }
    private void CreateHealthSlider()
    {
        if (agent.GetComponent<PlayableStructure>() != null && !agent.GetComponent<PlayableStructure>().IsPlaced)
            return;

        if (spawnedHealthSlider == null)
        {
            spawnedHealthSlider = Poolable.TryGetPoolable(WorldSpaceInterfaceManager.instance.GetSlider(healthSliderSize)).GetComponent<SliderController>();
            spawnedHealthSlider.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            spawnedHealthSlider.MinValue = 0f;
            spawnedHealthSlider.MaxValue = agent.MaxHealth;
            spawnedHealthSlider.Value = agent.MaxHealth;
            spawnedHealthSlider.Color = AlignmentManager.instance.GetAlignment(agent.Alignment).color;
            spawnedHealthSlider.SetTarget(transform);
            spawnedHealthSlider.SetTargetHeightOffset(agent.mainCollider.bounds.max.y);
        }
    }
    private void UpdateHealthSlider()
    {
        if (spawnedHealthSlider != null)
        {
            if (agent.ActualHealth == agent.MaxHealth)
                spawnedHealthSlider.HideSlider();
            else
            {
                spawnedHealthSlider.ShowSlider();

                spawnedHealthSlider.Value = agent.ActualHealth;
            }
        }   
    }
    // Private (Methods) [END]

    // Public (Methods) [START]
    public void GenerateFloatingText(float value)
    {
        FloatingTextController sft = Poolable.TryGetPoolable(WorldSpaceInterfaceManager.instance.GetTextFloatingValue(textFloatingValueSize)).GetComponent<FloatingTextController>();
        sft.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        sft.Value = value;
        sft.Color = AlignmentManager.instance.GetAlignment(agent.Alignment).color;
        sft.SetTarget(transform);
        sft.SetTargetHeightOffset(agent.mainCollider.bounds.max.y);
        sft.ForceMotionUpdate();
    }
    public void TryPool()
    {
        HealthSliderTryPool();
    }
    public void HealthSliderTryPool()
    {
        if (spawnedHealthSlider != null)
            Poolable.TryPool(spawnedHealthSlider.gameObject);

        spawnedHealthSlider = null;
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////