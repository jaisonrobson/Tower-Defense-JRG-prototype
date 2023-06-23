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
        Target.AddAffectingStatus(this);

        BC_AttackVelocity = Target.AttackVelocity;
        BC_Velocity = Target.Velocity;
        AC_AttackVelocity = BC_AttackVelocity * Mathf.Abs((statusAffectorSO.influence / 100f) - 1f);
        AC_Velocity = BC_Velocity * Mathf.Abs((statusAffectorSO.influence / 100f) - 1f);

        Target.UpdateAgentVelocity(AC_Velocity);
        Target.UpdateAgentAttackVelocity(AC_AttackVelocity);
    }

    public void ResetAgentStats()
    {
        Target.UpdateAgentVelocity(Target.Velocity + (BC_Velocity - AC_Velocity));
        Target.UpdateAgentAttackVelocity(Target.AttackVelocity + (BC_AttackVelocity - AC_AttackVelocity));

        Target.RemoveAffectingStatus(this);

        ResetProperties();
    }
    public override void PoolRetrievalAction(Poolable poolable)
    {
        base.PoolRetrievalAction(poolable);
    }

    public override void PoolInsertionAction(Poolable poolable)
    {
        ResetAgentStats();

        base.PoolInsertionAction(poolable);
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////