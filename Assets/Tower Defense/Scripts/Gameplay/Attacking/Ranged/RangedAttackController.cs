using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;
using Core.Math;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(RangedAttackEnemyDetectionColliderController))]
[HideMonoScript]
public class RangedAttackController : AttackController
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
        HandleAttackMovement();
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void ResetVariables()
    {
        RangedAttackAffector raa = GetComponent<RangedAttackAffector>();

        StartTime = Time.fixedTime;
        Duration = Time.fixedTime + raa.Duration;
    }
    private void HandleAttackMovement()
    {
        if (Time.fixedTime > Duration)
            return;

        RangedAttackAffector raa = GetComponent<RangedAttackAffector>();

        switch (GetComponent<RangedAttackAffector>().travellingType)
        {
            case AttackTravellingTypeEnum.ARCH:
                transform.position = Slerp.EvaluateSlerpPointsVector3(raa.Origin, raa.Destination, 1, StartTime, raa.Duration);
                break;
            default:
                float travellingFractionTime = Time.fixedTime / Duration;
                
                transform.position = Vector3.Lerp(raa.Origin, raa.Destination, travellingFractionTime);
                break;
        }
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