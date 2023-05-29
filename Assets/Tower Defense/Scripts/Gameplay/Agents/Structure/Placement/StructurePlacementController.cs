using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Core.Patterns;
using Core.Physics;

[HideMonoScript]
public class StructurePlacementController : Singleton<StructurePlacementController>
{
    // Public (Variables) [START]
    [Required]
    public LayerMask groundLayer;
    [Required]
    public LayerMask structureAreaLayer;
    // Public (Variables) [END]

    // Private (Variables) [START]
    private Camera mainCamera;
    private GameObject currentPlacingStructure;
    private List<PlayableAgentSO> levelStructures = new List<PlayableAgentSO>();
    // Private (Variables) [END]

    // Public (Properties) [START]
    public bool IsPlacing { get { return currentPlacingStructure != null; } }
    // Public (Properties) [END]

    // (Unity) Methods [START]
    void Start()
    {
        mainCamera = Camera.main;

        InitializeLevelStructures();
    }

    void FixedUpdate()
    {
        HandleStructureWorldPositioning();
        HandleInput();
    }

    void OnDrawGizmos()
    {
        if (currentPlacingStructure != null)
        {
            Vector3 pos = currentPlacingStructure.transform.position;
            pos.y += 1f;

            Gizmos.DrawRay(pos, Vector3.down);
        }
    }
    // (Unity) Methods [END]

    // Private Methods [START]
    private void InitializeLevelStructures()
    {
        PlayableAgentSO[] pAgents = MapManager.instance.map.playableAgents;

        foreach (PlayableAgentSO pAgt in pAgents)
        {
            if (pAgt.agent.type == AgentTypeEnum.STRUCTURE)
            {
                levelStructures.Add(pAgt);
            }
        }
    }

    private void HandleInput()
    {
        HandleGhostInstantiation();
        HandlePlacementCancelling();
        HandleStructurePlacement();
    }

    private void HandleStructureWorldPositioning()
    {
        if (currentPlacingStructure != null && currentPlacingStructure.activeInHierarchy)
        {
            RaycastHit hit = Raycasting.ScreenPointToRay(groundLayer);

            if (!Raycasting.IsHitEmpty(hit))
            {
                currentPlacingStructure.transform.position = hit.point;
            }
        }
    }

    private void HandleStructurePlacement()
    {
        if (currentPlacingStructure != null && currentPlacingStructure.activeInHierarchy)
        {
            //Placement
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 structureGhostPosition = currentPlacingStructure.transform.position;
                structureGhostPosition.y += 1000f;

                if (Physics.Raycast(structureGhostPosition, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, structureAreaLayer, QueryTriggerInteraction.Collide))
                {
                    if (hitInfo.collider.gameObject.GetComponent<PlacementArea>().alignment == MapManager.instance.map.playerAlignment.alignment)
                    {
                        if (currentPlacingStructure.GetComponentInChildren<StructurePlacementCollisionManager>().CanPlaceStructure())
                        {
                            currentPlacingStructure.GetComponent<Agent>().Alignment = MapManager.instance.map.playerAlignment.alignment;
                            currentPlacingStructure.GetComponent<PlayableStructure>().PlaceStructure();
                            currentPlacingStructure = null;
                        }
                    }
                }
            }
        }
    }
    private void HandlePlacementCancelling()
    {
        if (currentPlacingStructure != null)
        {
            if (Input.GetKey(KeyCode.Escape) || !currentPlacingStructure.activeInHierarchy)
            {
                Poolable.TryPool(currentPlacingStructure);
                currentPlacingStructure = null;
            }
        }
    }

    private void HandleGhostInstantiation()
    {
        if (currentPlacingStructure == null)
        {
            for (int i = 0; i < levelStructures.Count; i++)
            {
                if (Input.GetKey(i.ToString()))
                {
                    InstantiateGhost(levelStructures.ElementAt(i).agent.prefab);
                }
            }
        }
    }

    private void InstantiateGhost(GameObject structure)
    {
        currentPlacingStructure = Poolable.TryGetPoolable(structure);
    }

    // Private Methods [END]
}
