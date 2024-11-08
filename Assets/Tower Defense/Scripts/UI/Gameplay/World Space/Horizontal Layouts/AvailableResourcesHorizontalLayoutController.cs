using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Sirenix.OdinInspector;
using TheraBytes.BetterUi;

[Serializable]
public struct AvailableResourceHorizontalLayoutElement
{
    public ResourceEnum resource;
    public GameObject element;
}

[HideMonoScript]
public class AvailableResourcesHorizontalLayoutController : HorizontalLayoutController
{
    // Public (Variables) [START]
    [HideInEditorMode]
    public Agent agent;
    // Public (Variables) [END]

    // Private (Variables) [START]
    private List<AvailableResourceHorizontalLayoutElement> avaResElements;
    private ResourceSO[] resourcesSOs;
    // Private (Variables) [END]

    // (Unity) Methods [START]
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        InitializeVariables();
    }

    protected override void Update()
    {
        base.Update();

        HandleAvailableResourcesVisibility();
    }
    // (Unity) Methods [END]


    // Private (Methods) [START]
    private void InitializeVariables()
    {
        RefreshLayoutSize();
        InitializeAvailableResourcesElementsList();
    }
    private void InitializeAvailableResourcesElementsList()
    {
        resourcesSOs = Resources.LoadAll<ResourceSO>("SO's/Resources");

        avaResElements = new List<AvailableResourceHorizontalLayoutElement>();
        avaResElements.AddRange(Enumerable.Range(0, Enum.GetValues(typeof(ResourceEnum)).Length).Select(element => {
            AddElement(element);

            AvailableResourceHorizontalLayoutElement arhle = new()
            {
                resource = (ResourceEnum)element,
                element = spawnedElements.Find(se => se.id == element).element
            };

            return arhle;
        }));
    }
    private void HandleAvailableResourcesVisibility()
    {
        if (agent != null)
        {

            //Playable Structures
            if (agent.IsStructure && agent.GetComponent<StructureFsmAi>().IsPlayable && agent.GetComponent<PlayableStructure>().IsPlaced)
            {
                if (!agent.CanEvolve)
                    avaResElements.Where(element => element.resource == ResourceEnum.EVOLUTION).First().element.gameObject.SetActive(false);
                else
                {
                    ResourceSO selectedResourceSO = resourcesSOs.Where(sso => sso.type == ResourceEnum.EVOLUTION).First();

                    GameObject element = avaResElements.Where(element => element.resource == ResourceEnum.EVOLUTION).First().element;

                    element.SetActive(true);

                    RecalculateLayoutElementSizeValues(element);

                    element.GetComponent<BetterImage>().sprite = selectedResourceSO.image;
                }
            }
        }
    }
    // Private (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////