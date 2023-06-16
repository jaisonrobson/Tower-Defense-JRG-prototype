using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

[HideMonoScript]
public class StatusAffector : Affector
{
    // Public (Properties) [START]
    public float Duration { get; set; }
    // Public (Properties) [END]

    // Protected (Properties) [START]
    protected bool Finished { get; set; }
    // Protected (Properties) [END]


    // Private (Methods) [START]
    private void ResetVariables()
    {
        Finished = false;
        Duration = 0f;
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