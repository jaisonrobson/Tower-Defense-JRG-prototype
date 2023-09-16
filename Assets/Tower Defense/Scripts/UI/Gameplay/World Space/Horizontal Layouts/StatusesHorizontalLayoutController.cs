using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Sirenix.OdinInspector;
using TheraBytes.BetterUi;

public struct StatusHorizontalLayoutElement
{
    public StatusEnum status;
    public GameObject element;
}

[HideMonoScript]
public class StatusesHorizontalLayoutController : HorizontalLayoutController
{
    // Public (Variables) [START]
    [HideInEditorMode]
    public Agent agent;
    // Public (Variables) [END]

    // Private (Variables) [START]
    private List<StatusHorizontalLayoutElement> statusElements;
    private StatusSO[] statusesSOs;
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

        HandleStatusesVisibility();
    }
    // (Unity) Methods [END]


    // Private (Methods) [START]
    private void InitializeVariables()
    {
        UpdateSpawnedElementsList();
        RefreshLayoutSize();
        InitializeStatusElementsList();
    }
    private void InitializeStatusElementsList()
    {
        statusesSOs = Resources.LoadAll<StatusSO>("SO's/Statuses");

        statusElements = new List<StatusHorizontalLayoutElement>();
        statusElements.AddRange(Enumerable.Range(0, Enum.GetValues(typeof(StatusEnum)).Length).Select(element => {
            StatusHorizontalLayoutElement shle = new()
            {
                status = (StatusEnum)element,
                element = spawnedElements.ElementAt(element)
            };

            return shle;
        }));
    }
    private void UpdateSpawnedElementsList()
    {
        int quantityToInsert = Mathf.Abs(spawnedElements.Count - Enum.GetValues(typeof(StatusEnum)).Length);

        spawnedElements.AddRange(Enumerable.Range(0, quantityToInsert).Select(element => {
            GameObject newElement = Instantiate(horizontalLayoutElement);

            newElement.transform.SetParent(transform);

            RecalculateLayoutElementSizeValues(newElement);

            return newElement;
        }));

        spawnedElements.ForEach(el => el.SetActive(false));
    }
    private void HandleStatusesVisibility()
    {
        if (agent != null)
        {
            agent.NotAffectingStatuses.ForEach(status => {
                statusElements.Where(se => se.status == status)?.First().element.gameObject.SetActive(false);
            });

            agent.AffectingStatuses.ForEach(status => {
                StatusSO selectedStatusSO = statusesSOs.Where(sso => sso.status == status).First();

                GameObject element = statusElements.Where(se => se.status == status)?.First().element;

                element.SetActive(true);

                RecalculateLayoutElementSizeValues(element);

                element.GetComponent<BetterImage>().sprite = selectedStatusSO.image;
            });
        }
    }
    // Private (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////