using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Core.Patterns;

public class PlayableStructure : Structure
{
    [BoxGroup("Structure Identity")]
    [Required]
    public GameObject structure;
    [BoxGroup("Structure Identity")]
    [Required]
    public GameObject ghost;
    [BoxGroup("Structure Identity")]
    [Required]
    [AssetsOnly]
    public GameObject flag;
    // Public (Variables) [END]

    // Private (Variables) [START]
    [HideInEditorMode]
    private bool isPlaced = false;
    private Transform goalFlag;
    // Private (Variables) [END]

    // Public (Properties) [START]
    public Transform GoalFlag { get { return goalFlag; } }
    // Public (Properties) [END]

    // (Unity) Methods [START]
    protected override void Start()
    {
        base.Start();

        InitializeVariables();
    }
    protected override void Update()
    {
        base.Update();

        HandleStructurePlacement();
    }
    // (Unity) Methods [END]

    // Private Methods [START]
    private void HandleStructurePlacement()
    {
        if (isPlaced)
        {
            if (!structure.gameObject.activeSelf)
            {
                ghost.SetActive(false);

                structure.SetActive(true);

                InitializeGoalFlagPosition();
            }
        }
        else
        {
            if (!ghost.gameObject.activeSelf)
            {
                structure.SetActive(false);

                ghost.SetActive(true);
            }
        }
    }
    private void ResetStructurePlacement()
    {
        isPlaced = false;
        structure.SetActive(false);
        ghost.SetActive(true);
    }
    private void InitializeVariables()
    {
        InitializeGoalFlag();
    }
    private void InitializeGoalFlag()
    {
        goalFlag = Instantiate(flag).transform;
        goalFlag.SetParent(this.transform, false);        
    }
    private void InitializeGoalFlagPosition()
    {
        goalFlag.SetPositionAndRotation(GoalFlagInitialPosition(), Quaternion.identity);
    }
    private Vector3 GoalFlagInitialPosition()
    {
        return new Vector3(mainCollider.bounds.min.x - (mainCollider.bounds.extents.x * 0.5f), goalFlag.localScale.y / 2, mainCollider.bounds.min.z - (mainCollider.bounds.extents.z * 0.5f));
    }
    // Private Methods [END]

    // Public (Methods) [START]
    public bool GetIsStructurePlaced() { return isPlaced; }
    public void PlaceStructure() { isPlaced = true; }
    public override void PoolRetrievalAction(Poolable poolable)
    {
        ResetStructurePlacement();

        base.PoolRetrievalAction(poolable);
    }
    public override void PoolInsertionAction(Poolable poolable)
    {
        GetComponentInChildren<StructurePlacementCollisionManager>(true).PoolInsertionAction();

        ResetStructurePlacement();

        base.PoolInsertionAction(poolable);
    }
    public void SetFlagPosition(Vector3 worldPosition)
    {
        Vector3 newWorldPosition = worldPosition;

        float yPosition = worldPosition.y + GoalFlag.localScale.y / 2;

        Collider areaCollision = GetComponentInChildren<AgentEnemyColliderManager>().GetComponent<Collider>();

        newWorldPosition = areaCollision.ClosestPoint(newWorldPosition);

        newWorldPosition.y = yPosition;

        newWorldPosition = mainCollider.bounds.Contains(newWorldPosition) ? GoalFlagInitialPosition() : newWorldPosition;

        GoalFlag.position = newWorldPosition;
    }
    // Public (Methods) [END]
}
