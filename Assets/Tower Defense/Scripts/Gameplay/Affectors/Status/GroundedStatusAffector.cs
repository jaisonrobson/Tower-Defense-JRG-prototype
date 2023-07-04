using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

[HideMonoScript]
public class GroundedStatusAffector : StatusAffector
{
    // Private (Properties) [START]
    private float BC_Velocity { get; set; }
    // Private (Properties) [END]

    // Private (Methods) [START]
    private void ResetProperties()
    {
        BC_Velocity = 0f;
    }
    // Private (Methods) [END]

    // Protected (Methods) [START]
    protected override void ExecuteTurnActions()
    {
    }
    protected override void InitializeStatusActions()
    {
        BC_Velocity = Target.Velocity;

        Target.UpdateAgentVelocity(0f);
    }
    protected override void FinishStatusActions()
    {
        Target.UpdateAgentVelocity(BC_Velocity);

        ResetProperties();
    }
    // Protected (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////