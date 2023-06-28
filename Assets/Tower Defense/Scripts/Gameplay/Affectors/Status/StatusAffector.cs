using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

[HideMonoScript]
public abstract class StatusAffector : Affector
{
    // Private (Variables) [START]
    private int actualTurn = 0;
    private float lastTimeExecutedTurn = 0f;
    // Private (Variables) [END]

    // Public (Variables) [START]
    public StatusAffectorSO statusAffectorSO;
    // Public (Variables) [END]

    // Public (Properties) [START]
    public float Duration { get; set; }
    public int TurnsInterval { get; set; }
    public float Damage { get; set; }
    // Public (Properties) [END]

    // Protected (Properties) [START]
    protected bool Finished { get; set; }
    // Protected (Properties) [END]

    // (Unity) Methods [START]
    protected virtual void Update()
    {
        HandleStatusExistanceByDuration();
        HandleStatusFinishing();
        HandleTurnsExecuting();
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        Duration = Time.time + statusAffectorSO.duration;
        Damage = (statusAffectorSO.damage / 100f) * Invoker.Damage;
        TurnsInterval = statusAffectorSO.duration / statusAffectorSO.turnsQuantity;

        Target.AddAffectingStatus(this);

        Target.AddPoolInsertionAction(OnPool);

        InitializeStatusActions();
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void HandleTurnsExecuting()
    {
        if (Time.time > (lastTimeExecutedTurn + actualTurn * TurnsInterval) && actualTurn < statusAffectorSO.turnsQuantity)
        {
            lastTimeExecutedTurn = Time.time;
            actualTurn++;

            ExecuteTurnActions();
        }
    }
    private void HandleStatusExistanceByDuration()
    {
        if (Time.time > Duration)
            Finished = true;
    }
    private void HandleStatusFinishing()
    {
        if (Finished)
            Poolable.TryPool(gameObject);
    }
    private void ResetVariables()
    {
        Finished = false;
        Duration = 0f;
        Damage = 0f;
        TurnsInterval = 0;
        actualTurn = 0;
        lastTimeExecutedTurn = 0f;
    }
    // Private (Methods) [END]

    // Protected (Methods) [START]
    protected abstract void ExecuteTurnActions();
    protected abstract void InitializeStatusActions();
    protected abstract void FinishStatusActions();
    // Protected (Methods) [END]

    // Public (Methods) [START]
    public void OnPool()
    {
        Poolable.TryPool(gameObject);
    }
    public override void PoolRetrievalAction(Poolable poolable)
    {
        base.PoolRetrievalAction(poolable);

        ResetVariables();
    }
    public override void PoolInsertionAction(Poolable poolable)
    {
        base.PoolInsertionAction(poolable);

        Target.RemoveAffectingStatus(this);

        FinishStatusActions();

        Target.RemovePoolInsertionAction(OnPool);
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////