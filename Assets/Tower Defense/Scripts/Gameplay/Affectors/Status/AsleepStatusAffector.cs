using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

[HideMonoScript]
public class AsleepStatusAffector : StatusAffector
{
    // Private (Variables) [START]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private int attacksReceived = 0;
    // Private (Variables) [END]

    // Private (Properties) [START]
    private float BC_AttackVelocity { get; set; }
    private float BC_Velocity { get; set; }
    // Private (Properties) [END]

    // (Unity) Methods [START]
    protected override void Update()
    {
        base.Update();

        HandleAgentAwakening();
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void ResetProperties()
    {
        BC_Velocity = 0f;
        BC_AttackVelocity = 0f;
        attacksReceived = 0;
    }
    private void HandleAgentAwakening()
    {
        if (attacksReceived >= statusAffectorSO.specialCondition)
        {
            Finished = true;
        }
    }
    // Private (Methods) [END]

    // Protected (Methods) [START]
    protected override void ExecuteTurnActions()
    {
    }
    protected override void InitializeStatusActions()
    {
        BC_Velocity = Target.Velocity;
        BC_AttackVelocity = Target.AttackVelocity;

        Target.UpdateAgentVelocity(0f);
        Target.UpdateAgentAttackVelocity(0f);
        Target.onReceiveDamageAction += IncreaseAttacksReceived;
    }
    protected override void FinishStatusActions()
    {
        Target.UpdateAgentVelocity(BC_Velocity);
        Target.UpdateAgentAttackVelocity(BC_AttackVelocity);
        Target.onReceiveDamageAction -= IncreaseAttacksReceived;

        ResetProperties();
    }
    // Protected (Methods) [END]


    // Public (Methods) [START]
    public void IncreaseAttacksReceived() => attacksReceived++;
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////