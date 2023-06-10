using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

[RequireComponent(typeof(OutcomeAttackController))]
[HideMonoScript]
public class OutcomeAttackAffector : AttackAffector
{
    // Public (Properties) [START]
    public Vector3 Origin { get; set; }
    public Quaternion InitialRotation { get; set; }
    // Public (Properties) [END]

    // Public (Methods) [START]
    public override void PoolRetrievalAction(Poolable poolable)
    {
        base.PoolRetrievalAction(poolable);

        GetComponent<OutcomeAttackController>().PoolRetrievalAction(poolable);
    }
    public override void PoolInsertionAction(Poolable poolable)
    {
        base.PoolInsertionAction(poolable);

        Origin = Vector3.zero;
        InitialRotation = Quaternion.identity;

        GetComponent<OutcomeAttackController>().PoolInsertionAction(poolable);
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////