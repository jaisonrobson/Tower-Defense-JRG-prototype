using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

[HideMonoScript]
public class StatusAffector : Affector
{
    // Public (Variables) [START]
    public StatusAffectorSO statusAffectorSO;
    // Public (Variables) [END]

    // Public (Properties) [START]
    public float Duration { get; set; }
    public Agent AffectedAgent { get; set; }
    public Agent AffectorAgent { get; set; }
    // Public (Properties) [END]

    // Protected (Properties) [START]
    protected bool Finished { get; set; }
    // Protected (Properties) [END]

    // (Unity) Methods [START]
    protected virtual void Update()
    {
        HandleStatusExistanceByDuration();
        HandleStatusFinishing();
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        Duration = Time.time + statusAffectorSO.duration;
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
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
        AffectedAgent = null;
        AffectorAgent = null;
    }
    // Private (Methods) [END]

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

        if (AffectedAgent != null)
            AffectedAgent.RemovePoolInsertionAction(OnPool);
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////