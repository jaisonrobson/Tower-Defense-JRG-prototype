using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

[Serializable]
public struct OutcomeCollisionConfiguration
{
    [Required]
    [AssetsOnly]
    public OutcomeAttackEnemyDetectionColliderController controller;
    [Range(0, 60)]
    [PropertyTooltip("Time in seconds this outcome will be executed before the script start")]
    public float timeInSequence;
    [ReadOnly]
    public bool activated;

    public OutcomeCollisionConfiguration(OutcomeAttackEnemyDetectionColliderController pController)
    {
        controller = pController;
        timeInSequence = 0f;
        activated = false;
    }
}

[RequireComponent(typeof(Poolable))]
[HideMonoScript]
public class OutcomeAttackController : MonoBehaviour, IPoolable
{
    // Public (Variables) [START]
    [OnInspectorInit("MaintainOutcomeCollisions")]
    public List<OutcomeCollisionConfiguration> outcomeCollisions;
    // Public (Variables) [END]

    // Private (Variables) [START]
    [ReadOnly]
    [ShowInInspector]
    private bool isRunning = false;
    private float startTime = 0f;
    // Private (Variables) [END]

    // Public (Properties) [START]
    [ReadOnly]
    [ShowInInspector]
    public AlignmentEnum Alignment { get; protected set; }
    [ReadOnly]
    [ShowInInspector]
    public LayerMask AffectedsMask { get; protected set; }
    [ReadOnly]
    [ShowInInspector]
    public AttackSO Attack { get; protected set; }
    [ReadOnly]
    [ShowInInspector]
    public float Damage { get; protected set; }
    // Public (Properties) [END]

    // (Unity) Methods [START]
    private void OnEnable()
    {
        ResetVariables();
    }
    private void Update()
    {
        HandleOutcomeSequencesActivation();
        HandleOutcomeEnding();
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void ResetVariables()
    {
        isRunning = false;
        outcomeCollisions.ForEach(oc => oc.activated = false);
        transform.position = Vector3.zero;
        Alignment = AlignmentEnum.GENERIC;
        AffectedsMask = 0;
        Attack = null;
        Damage = 0;
    }
    private void HandleOutcomeSequencesActivation()
    {
        if (isRunning)
        {
            float sequenceTiming = Time.time - startTime;

            outcomeCollisions.ForEach(oc =>
            {
                if (sequenceTiming > oc.timeInSequence)
                    if (!oc.activated)
                    {
                        oc.activated = true;

                        oc.controller.StartOutcomeCollider();
                    }
            });
        }
    }
    private void HandleOutcomeEnding()
    {
        if (isRunning && outcomeCollisions.All(oc => oc.activated && oc.controller.Finished))
            Poolable.TryPool(gameObject);
    }
    // Private (Methods) [END]

    // Public (Methods) [START]
    public void StartOutcome(Vector3 pPosition, AlignmentEnum pAlignment, LayerMask pAffectedsMask, AttackSO pAttack, float pDamage)
    {
        //REFACTOR OUTCOME TO HAVE AN AFFECTOR INSTEAD OF RECEIVING ALL VARIABLES DIRECTLY BY METHOD
        transform.position = pPosition;
        Alignment = pAlignment;
        AffectedsMask = pAffectedsMask;
        Attack = pAttack;
        Damage = pDamage;
        startTime = Time.time;
        isRunning = true;
    }
    public virtual void PoolRetrievalAction(Poolable poolable)
    {
        ResetVariables();

        outcomeCollisions.ForEach(oc => oc.controller.PoolRetrievalAction(poolable));
    }

    public virtual void PoolInsertionAction(Poolable poolable)
    {
        ResetVariables();

        outcomeCollisions.ForEach(oc => oc.controller.PoolInsertionAction(poolable));
    }
    // Public (Methods) [END]

    // (Validation) Methods [START]
    private void MaintainOutcomeCollisions()
    {
        if (outcomeCollisions == null)
            outcomeCollisions = new List<OutcomeCollisionConfiguration>();

        GetComponentsInChildren<OutcomeAttackEnemyDetectionColliderController>(true)
            .ToList()
            .ForEach(occ => {
                if (!outcomeCollisions.Any(oc => oc.controller.GetInstanceID() == occ.GetInstanceID()))
                    outcomeCollisions.Add(new OutcomeCollisionConfiguration(occ));
            });
    }
    // (Validation) Methods [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////