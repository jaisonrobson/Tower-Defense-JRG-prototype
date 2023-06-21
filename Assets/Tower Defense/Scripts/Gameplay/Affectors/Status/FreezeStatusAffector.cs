using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

[HideMonoScript]
public class FreezeStatusAffector : StatusAffector
{
    // Private (Properties) [START]
    private float BC_AttackVelocity { get; set; }
    private float AC_AttackVelocity { get; set; }
    private float BC_Velocity { get; set; }
    private float AC_Velocity { get; set; }
    // Private (Properties) [END]

    // Private (Methods) [START]
    private void ResetProperties()
    {
        BC_AttackVelocity = 0f;
        AC_AttackVelocity = 0f;
        BC_Velocity = 0f;
        AC_Velocity = 0f;
    }
    // Private (Methods) [END]

    // Public (Methods) [START]
    public void UpdateAgentStats()
    {
        BC_AttackVelocity = AffectedAgent.AttackVelocity;
        BC_Velocity = AffectedAgent.Velocity;
        AC_AttackVelocity = BC_AttackVelocity * Mathf.Abs((statusAffectorSO.influence / 100) - 1);
        AC_Velocity = BC_Velocity * Mathf.Abs((statusAffectorSO.influence / 100) - 1);

        AffectedAgent.UpdateAgentVelocity(AC_Velocity);
        AffectedAgent.UpdateAgentAttackVelocity(AC_AttackVelocity);
    }

    public void ResetAgentStats()
    {
        AffectedAgent.UpdateAgentVelocity(AffectedAgent.Velocity + (BC_Velocity - AC_Velocity));
        AffectedAgent.UpdateAgentAttackVelocity(AffectedAgent.AttackVelocity + (BC_AttackVelocity - AC_AttackVelocity));

        ResetProperties();
    }
    public override void PoolRetrievalAction(Poolable poolable)
    {
        base.PoolRetrievalAction(poolable);
    }

    public override void PoolInsertionAction(Poolable poolable)
    {
        base.PoolInsertionAction(poolable);

        ResetAgentStats();
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////