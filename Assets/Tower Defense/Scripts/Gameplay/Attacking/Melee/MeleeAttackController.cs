using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeleeAttackEnemyDetectionColliderController))]
[HideMonoScript]
public class MeleeAttackController : AttackController
{
    // Private (Properties) [START]
    private float StartTime { get; set; }
    private float Duration { get; set; }
    // Private (Properties) [END]

    // (Unity) Methods [START]
    private void OnEnable()
    {
        ResetVariables();
    }
    private void FixedUpdate()
    {
        HandleAttackPositioning();
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void ResetVariables()
    {
        MeleeAttackAffector maa = GetComponent<MeleeAttackAffector>();

        StartTime = Time.fixedTime;
        Duration = Time.fixedTime + maa.Duration;
    }
    private void HandleAttackPositioning()
    {
        if (Time.fixedTime > Duration)
            return;

        MeleeAttackAffector maa = GetComponent<MeleeAttackAffector>();

        transform.position = maa.Origin;
        transform.rotation = maa.InitialRotation;
    }
    // Private (Methods) [END]

    // Public (Methods) [START]
    public override void PoolRetrievalAction(Poolable poolable)
    {
        base.PoolRetrievalAction(poolable);

        ResetVariables();
    }
    public override void PoolInsertionAction(Poolable poolable)
    {
        base.PoolInsertionAction(poolable);
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////