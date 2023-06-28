using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

[HideMonoScript]
public class DrownStatusAffector : StatusAffector
{
    // Private (Properties) [START]
    private float BC_AttackVelocity { get; set; }
    private float BC_Velocity { get; set; }
    // Private (Properties) [END]

    // Private (Methods) [START]
    private void ResetProperties()
    {
        BC_Velocity = 0f;
        BC_AttackVelocity = 0f;
    }
    // Private (Methods) [END]

    // Protected (Methods) [START]
    protected override void ExecuteTurnActions()
    {
        Target.OnReceiveDamage(Alignment, Damage, statusAffectorSO);
    }
    protected override void InitializeStatusActions()
    {
        BC_Velocity = Target.Velocity;
        BC_AttackVelocity = Target.AttackVelocity;

        Target.UpdateAgentVelocity(0f);
        Target.UpdateAgentAttackVelocity(0f);
    }
    protected override void FinishStatusActions()
    {
        Target.UpdateAgentVelocity(BC_Velocity);
        Target.UpdateAgentAttackVelocity(BC_AttackVelocity);

        ResetProperties();
    }
    // Protected (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////