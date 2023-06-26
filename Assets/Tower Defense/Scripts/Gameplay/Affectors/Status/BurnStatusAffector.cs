using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

[HideMonoScript]
public class BurnStatusAffector : StatusAffector
{
    // Private (Properties) [START]
    private int actualTurn = 0;
    private float lastTimeDamaged = 0f;
    // Private (Properties) [END]

    // (Unity) Methods [START]
    protected override void OnEnable()
    {
        base.OnEnable();

        UpdateAgentStats();
    }
    protected override void Update()
    {
        base.Update();

        HandleTurnsDamaging();
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void HandleTurnsDamaging()
    {
        if (Time.time > (lastTimeDamaged + actualTurn * TurnsInterval) && actualTurn < statusAffectorSO.turnsQuantity)
        {
            lastTimeDamaged = Time.time;
            actualTurn++;

            Target.OnReceiveDamage(Alignment, Damage, statusAffectorSO);
        }
    }
    private void ResetProperties()
    {
        actualTurn = 0;
        lastTimeDamaged = 0f;
    }
    // Private (Methods) [END]

    // Public (Methods) [START]
    public void UpdateAgentStats()
    {
        Target.AddAffectingStatus(this);
    }

    public void ResetAgentStats()
    {
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